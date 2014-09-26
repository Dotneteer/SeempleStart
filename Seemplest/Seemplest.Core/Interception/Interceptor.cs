using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Seemplest.Core.Exceptions;

namespace Seemplest.Core.Interception
{
    /// <summary>
    /// This class is responsible for creating intercepted objects.
    /// </summary>
    public static class Interceptor
    {
        // --- Filed name constants
        private const string TARGET_FIELD = "_target";
        private const string ASPECTS_FIELD = "_aspects";

        // --- Stores the interceptors
        private static readonly Dictionary<Type, InterceptedTypeDescriptor> s_Interceptors =
            new Dictionary<Type, InterceptedTypeDescriptor>();

        // --- Caches the types traversed during discovery
        private static readonly ConcurrentDictionary<Type, List<AspectAttributeBase>> s_Types =
            new ConcurrentDictionary<Type, List<AspectAttributeBase>>();

        // --- Caches the methods traversed during discovery
        private static readonly ConcurrentDictionary<MethodKey, List<AspectAttributeBase>> s_Methods =
            new ConcurrentDictionary<MethodKey, List<AspectAttributeBase>>();

        private static AssemblyBuilder s_AssemblyBuilder;
        private static ModuleBuilder s_ModuleBuilder;
        private static FieldBuilder s_TargetField;
        private static FieldBuilder s_AspectField;

        /// <summary>
        /// Clears the cached information from the memory
        /// </summary>
        public static void ResetCache()
        {
            s_Interceptors.Clear();
            s_Types.Clear();
            s_Methods.Clear();
            s_AssemblyBuilder = null;
            s_ModuleBuilder = null;
        }

        /// <summary>
        /// Creates an intercepted object for the specified target using the provided aspect chain.
        /// </summary>
        /// <typeparam name="TService">Type of service object</typeparam>
        /// <param name="target">Target object to create an intercepted object for</param>
        /// <param name="aspects">Aspects to be used for the service object's method calls</param>
        /// <returns>Intercepted service object</returns>
        public static TService GetInterceptedObject<TService>(TService target, AspectChain aspects)
        {
            return (TService)GetInterceptedObject(typeof(TService), target, aspects);
        }

        /// <summary>
        /// Creates an intercepted object for the specified target using the provided aspect chain.
        /// </summary>
        /// <param name="serviceType">Type of service object</param>
        /// <param name="target">Target object to create an intercepted object for</param>
        /// <param name="aspects">Aspects to be used for the service object's method calls</param>
        /// <returns>Intercepted service object</returns>
        public static object GetInterceptedObject(Type serviceType, object target, AspectChain aspects)
        {
            if (serviceType == null) throw new ArgumentNullException("serviceType");

            // --- Check the cache
            InterceptedTypeDescriptor interceptor;
            if (!s_Interceptors.TryGetValue(serviceType, out interceptor))
            {
                lock (s_Interceptors)
                {
                    // --- Validate again to prevent a parallel type creation
                    if (!s_Interceptors.TryGetValue(serviceType, out interceptor))
                    {
                        // --- Validate input parameters
                        if (target == null) throw new ArgumentNullException("target");
                        if (!serviceType.IsInterface || !serviceType.IsInstanceOfType(target))
                        {
                            throw new InvalidOperationException("Service type must be an interface" +
                                                                " type and the target object must implement that interface.");
                        }

                        // --- No generics is accepted
                        if (serviceType.IsGenericType ||
                            serviceType.GetMethods().Any(met => met.IsGenericMethod) ||
                            serviceType.GetInterfaces().Any(intf => intf.IsGenericType
                                                                    || intf.GetMethods().Any(met => met.IsGenericMethod)))
                        {
                            throw new InvalidOperationException(
                                "Service type must not have generic parameters or methods.");
                        }

                        // --- We do not have any wrapper types for the specified service type
                        // --- Generate the interceptor type and save it in the cache
                        interceptor = new InterceptedTypeDescriptor
                        {
                            InterceptedType = serviceType,
                            Methods = GetInterfaceMethods(serviceType)
                        };
                        GenerateWrappedType(interceptor, serviceType);
                        s_Interceptors[serviceType] = interceptor;
                    }
                }
            }

            // --- Instantiate the wrapped type
            var wrappedInstance = Activator.CreateInstance(
                interceptor.WrapperType, 
                new[] { target, aspects });
            return wrappedInstance;
        }

        /// <summary>
        /// Executes the method call on the target object and wraps it into the execution of the
        /// specified object chain.
        /// </summary>
        /// <param name="target">Target object</param>
        /// <param name="arguments">Method call arguments</param>
        /// <param name="targetMethod"></param>
        /// <param name="aspects">Aspect chain to execute</param>
        /// <param name="methodInfo"></param>
        /// <returns>Result of the method execution</returns>
        public static IMethodResultDescriptor ExecuteWrappedCall<TService>(TService target, List<MethodArgument> arguments, 
            MethodInfo methodInfo, Func<IMethodCallDescriptor, TService, IMethodResultDescriptor> targetMethod, 
            AspectChain aspects)
        {
            // --- Use the aspects of the object, if that supports IMethodAspect
            if (aspects == null)
            {
                var typeAspects = GetAspects(methodInfo, target.GetType());
                if (typeAspects.Count > 0) aspects = new AspectChain(typeAspects);
            }

            var targetAspect = target as IMethodAspect;
            if (targetAspect != null)
            {
                var aspectList = new List<IMethodAspect> { targetAspect };
                if (aspects != null) aspectList.AddRange(aspects);
                aspects = new AspectChain(aspectList);
            }

            // --- Obtain the method descriptor
            var args = new MethodCallDescriptor(target, methodInfo, arguments);
            var onEntryResult = OnEntry(aspects, args);
            if (onEntryResult == null)
            {
                // --- Call original method body
                IMethodResultDescriptor result;
                try
                {
                    result = targetMethod(args, target);
                }
                catch (Exception ex)
                {
                    result = new MethodResultDescriptor(ex);
                }

                // --- Call OnExit
                OnExit(aspects, args, result);
                if (result.Exception == null)
                {
                    onEntryResult = OnSuccess(aspects, args, result);
                }
                else
                {
                    var newEx = OnException(aspects, args, result.Exception);
                    if (newEx == null)
                    {
                        throw new AspectInfrastructureException("OnException returned null.", null);
                    }
                    throw newEx;
                }
            }
            return onEntryResult;
        }

        /// <summary>
        /// Invokes the OnEntry method on all elements of the aspect chain.
        /// </summary>
        /// <param name="aspects">Aspect chain</param>
        /// <param name="args">Method arguments descriptor</param>
        /// <returns>Result of the execution</returns>
        // ReSharper disable ParameterTypeCanBeEnumerable.Local
        private static IMethodResultDescriptor OnEntry(AspectChain aspects, IMethodCallDescriptor args)
        // ReSharper restore ParameterTypeCanBeEnumerable.Local
        {
            try
            {
                if (aspects == null) return null;
                IMethodResultDescriptor result = null;
                foreach (var aspect in aspects)
                {
                    result = aspect.OnEntry(args, null);
                    if (result != null) break;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new AspectInfrastructureException(
                    "Aspect infrastructure exception caught in OnEntry", ex);
            }
        }

        /// <summary>
        /// Invokes the OnExit method on all elements of the aspect chain.
        /// </summary>
        /// <param name="aspects">Aspect chain</param>
        /// <param name="args">Method arguments descriptor</param>
        /// <param name="result">Result of the method call</param>
        /// <returns>Result of the execution</returns>
        private static void OnExit(AspectChain aspects, IMethodCallDescriptor args, 
            IMethodResultDescriptor result)
        {
            try
            {
                if (aspects == null) return;
                foreach (var aspect in aspects.Reverse)
                {
                    aspect.OnExit(args, result);
                }
            }
            catch (Exception ex)
            {
                throw new AspectInfrastructureException(
                    "Aspect infrastructure exception caught in OnExit", ex);
            }
        }

        /// <summary>
        /// Invokes the OnSuccess method on all elements of the aspect chain.
        /// </summary>
        /// <param name="aspects">Aspect chain</param>
        /// <param name="args">Method arguments descriptor</param>
        /// <param name="result">Result of the method call</param>
        /// <returns>Result of the execution</returns>
        private static IMethodResultDescriptor OnSuccess(AspectChain aspects, IMethodCallDescriptor args,
            IMethodResultDescriptor result)
        {
            try
            {
                if (aspects != null)
                {
                    foreach (var aspect in aspects.Reverse)
                    {
                        aspect.OnSuccess(args, result);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new AspectInfrastructureException(
                    "Aspect infrastructure exception caught in OnSuccess", ex);
            }
        }

        /// <summary>
        /// Invokes the OnException method on all elements of the aspect chain.
        /// </summary>
        /// <param name="aspects">Aspect chain</param>
        /// <param name="args">Method arguments descriptor</param>
        /// <param name="ex">Exception instance</param>
        /// <returns>Result of the execution</returns>
        private static Exception OnException(AspectChain aspects, IMethodCallDescriptor args,
            Exception ex)
        {
            try
            {
                return aspects == null
                           ? ex
                           : aspects.Reverse.Aggregate(ex,
                                 (current, aspect) => aspect.OnException(args, current));
            }
            catch (Exception newEx)
            {
                throw new AspectInfrastructureException(
                    "Aspect infrastructure exception caught in OnSuccess", newEx);
            }
        }

        /// <summary>
        /// Generates an intercepted type for the specified type
        /// </summary>
        /// <param name="descriptor">Intercepted type descriptor to generate the dynamic code into</param>
        /// <param name="interfaceType">Type of interface this type is generated for</param>
        /// <returns>Descriptor instance</returns>
        private static void GenerateWrappedType(InterceptedTypeDescriptor descriptor, Type interfaceType)
        {
            // --- Create the dynamic assembly and the dynamic type holding the wrapper
            EnsureModuleBuilder();

            // --- Create the helper type for method information
            var methodHelperType = CreateMethodHelperType(interfaceType);
            var invokerHelperType = CreateInvokerHelperType(interfaceType);
            var wrapper = CreateWrapperType(interfaceType, methodHelperType, invokerHelperType);

            // --- Add a new type to the generated assembly
            descriptor.WrapperType = wrapper;
        }

        // --- This class will generate a static class with MethodInfo fields:
        // public static MethodHelper_0000_IFoo
        // {
        //     public MethodInfo Op1_Method;
        //     ...
        //     public MethodInfo OpN_Method;
        //
        //     static MethodHelper_0000_IFoo()
        //     {
        //         var types = typeof(IFoo).GetMethods;
        //         Op1_Method = types[0];
        //         ...
        //         OpN_Method = types[N-1];
        //     }
        // }
        private static Type CreateMethodHelperType(Type interfaceType)
        {
            // --- Create the static helper type
            var typeName = "MethodHelper_" + interfaceType.GetHashCode() + "_" + interfaceType.FullName;
            var typeBuilder = s_ModuleBuilder.DefineType(
                typeName,
                TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit 
                | TypeAttributes.AutoClass | TypeAttributes.AnsiClass,
                typeof(object),
                Type.EmptyTypes);

            // --- Create the static constructor -- it will initialzie members
            var cctorDef = typeBuilder.DefineConstructor(
                MethodAttributes.Private | MethodAttributes.Static,
                CallingConventions.Standard,
                Type.EmptyTypes);

            var ccIlGen = cctorDef.GetILGenerator();
            var types = ccIlGen.DeclareLocal(typeof (MethodInfo[]));            // --- Declare the 'types' array

            // --- Iterate through each interface methods ans store their method information
            var interfaces = GetInterfaceChain(interfaceType);
            foreach (var intf in interfaces)
            {
                ccIlGen.Emit(OpCodes.Ldtoken, intf);                    // --- Push the interface type's token
                ccIlGen.Emit(OpCodes.Call, GetTypeFromHandle_Method);   // --- Get the interface type's type information
                ccIlGen.Emit(OpCodes.Call, GetMethods_Method);          // --- Call GetMethods on interface type
                ccIlGen.Emit(OpCodes.Stloc_0);                          // --- Store methods to 'types'
                var methods = intf.GetMethods();
                for (var index = 0; index < methods.Length; index++)
                {
                    var method = methods[index];
                    var fieldName = GetMethodHelperName(method);
                    var fieldDef = typeBuilder.DefineField(fieldName, typeof(MethodInfo),
                        FieldAttributes.Public | FieldAttributes.Static);
                    ccIlGen.Emit(OpCodes.Ldloc_S, types);       // --- Push 'types' address
                    ccIlGen.Emit(OpCodes.Ldc_I4_S, index);      // --- Push 'index'
                    ccIlGen.Emit(OpCodes.Ldelem_Ref);           // --- Retrieve types[index]
                    ccIlGen.Emit(OpCodes.Stsfld, fieldDef);      // --- Store it to the new static field
                }
            }
            ccIlGen.Emit(OpCodes.Ret);                      // --- Return from mehtod

            // --- Complete the type
            return typeBuilder.CreateType();
        }

        // --- This class will generate a static class with 
        // --- Func<IMethodCallDescriptor, IFoo, IMethodResultDescriptor> methods:
        // public static InvokeHelper_0000_IFoo
        // {
        //     static InvokeHelper_0000_IFoo()
        //     {
        //     }
        //
        //     IMethodResultDescriptor Op1(IMethodCallDescriptor args, IFoo target)
        //     {
        //         ...
        //     }
        // }
        private static Type CreateInvokerHelperType(Type interfaceType)
        {
            // --- Create the static helper type
            var typeName = "InvokeHelper_" + interfaceType.GetHashCode() + "_" + interfaceType.FullName;
            var typeBuilder = s_ModuleBuilder.DefineType(
                typeName,
                TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit
                | TypeAttributes.AutoClass | TypeAttributes.AnsiClass,
                typeof(object),
                Type.EmptyTypes);

            // --- Create the static constructor -- it will initialzie members
            var cctorDef = typeBuilder.DefineConstructor(
                MethodAttributes.Private | MethodAttributes.Static,
                CallingConventions.Standard,
                Type.EmptyTypes);
            var ccIlGen = cctorDef.GetILGenerator();
            ccIlGen.Emit(OpCodes.Ret);

            // --- Iterate through all interface methods
            var methods = GetInterfaceMethods(interfaceType);
            foreach (var method in methods)
            {
                var methodName = GetMethodHelperName(method);
                var methodDef = typeBuilder.DefineMethod(
                    methodName,
                    MethodAttributes.Public | MethodAttributes.Static,
                    CallingConventions.Standard,
                    typeof (IMethodResultDescriptor),
                    new[] {typeof (IMethodCallDescriptor), method.DeclaringType});
                var mIlGen = methodDef.GetILGenerator();

                // --- Generate the call according to the interface method parameters
                GenerateInvokeMethod(mIlGen, method);
            }

            // --- Complete the type
            return typeBuilder.CreateType();
        }

        /// <summary>
        /// Creates a method that invokes the target interface method
        /// </summary>
        /// <param name="mIlGen">Method body IL generator</param>
        /// <param name="method">Interface method information</param>
        private static void GenerateInvokeMethod(ILGenerator mIlGen, MethodInfo method)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            // --- Obtain method parameters
            var parameters = method.GetParameters();

            // --- Generate a local variable for output arguments
            var outputArgs = mIlGen.DeclareLocal(typeof (List<MethodArgument>));
            mIlGen.Emit(OpCodes.Newobj, typeof(List<MethodArgument>).GetConstructor(Type.EmptyTypes));
            mIlGen.Emit(OpCodes.Stloc_S, outputArgs);

            // --- Rest counters
            var inArgIndex = 0;
            var outArgIndex = 0;

            // --- Iterate throug parameters
            var descriptors = new ParameterDescriptor[parameters.Length];
            for (var index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index];
                descriptors[index] = new ParameterDescriptor
                    {
                        Name = parameter.Name,
                        Type = parameter.ParameterType,
                        LocalVariable = null,
                        InArgIndex = -1,
                        OutArgIndex = -1
                    };

                // --- Generate reference and output variables
                if (parameter.ParameterType.IsByRef || parameter.IsOut)
                {
                    // --- This argument must be an output argument
                    descriptors[index].OutArgIndex = outArgIndex++;

                    // --- Generate a local variable
                    var paramType = GetNonReferencedType(parameter.ParameterType);
                    descriptors[index].Type = paramType;
                    descriptors[index].LocalVariable = mIlGen.DeclareLocal(paramType);
                    if (!parameter.IsOut)
                    {
                        // --- Argument passed by reference
                        descriptors[index].InArgIndex = inArgIndex++;

                        // --- Initialize the local variable with the corresponding input argument
                        PushArgumentValueByIndex(mIlGen, descriptors[index].InArgIndex, paramType);
                        mIlGen.Emit(OpCodes.Stloc_S, descriptors[index].LocalVariable);
                    }
                }
                else
                {
                    // --- Simple input argument
                    descriptors[index].InArgIndex = inArgIndex++;
                }
            }

            // --- Its time to call the method on the target
            mIlGen.Emit(OpCodes.Ldarg_1); // --- Push target

            // --- Push parameters to the stack
            for (var index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index];
                if (parameter.ParameterType.IsByRef || parameter.IsOut)
                {
                    mIlGen.Emit(OpCodes.Ldloca_S, descriptors[index].LocalVariable);
                }
                else
                {
                    PushArgumentValueByIndex(mIlGen, descriptors[index].InArgIndex, parameter.ParameterType);
                }
            }

            // --- Invoke the method
            mIlGen.Emit(OpCodes.Callvirt, method);
            LocalBuilder returnValue = null;

            // --- Store the result, if there is any
            if (method.ReturnType != typeof (void))
            {
                returnValue = mIlGen.DeclareLocal(method.ReturnType);
                mIlGen.Emit(OpCodes.Stloc_S, returnValue);
            }

            // --- Extract output arguments
            foreach (var paramDescr in descriptors.Where(p => p.OutArgIndex >= 0))
            {
                mIlGen.Emit(OpCodes.Ldloc_S, outputArgs);
                mIlGen.Emit(OpCodes.Ldstr, paramDescr.Name);
                mIlGen.Emit(OpCodes.Ldloc_S, paramDescr.LocalVariable);
                if (paramDescr.Type.IsValueType)
                {
                    mIlGen.Emit(OpCodes.Box, paramDescr.Type);
                }
                mIlGen.Emit(OpCodes.Newobj, typeof(MethodArgument)
                    .GetConstructor(new [] { typeof(string), typeof(object) }));
                mIlGen.Emit(OpCodes.Callvirt, typeof(List<MethodArgument>).GetMethod("Add"));
            }

            // --- Create the response object
            if (returnValue == null)
            {
                // --- No return value
                mIlGen.Emit(OpCodes.Ldc_I4_0);
                mIlGen.Emit(OpCodes.Ldnull);
            }
            else
            {
                // --- There is a return value
                mIlGen.Emit(OpCodes.Ldc_I4_1);
                mIlGen.Emit(OpCodes.Ldloc_S, returnValue);
                if (method.ReturnType.IsValueType)
                {
                    // --- Do not forget to box value types
                    mIlGen.Emit(OpCodes.Box, method.ReturnType);
                }
            }
            mIlGen.Emit(OpCodes.Ldloc_S, outputArgs);
            mIlGen.Emit(OpCodes.Newobj, typeof(MethodResultDescriptor).GetConstructor(
                new[] { typeof(bool), typeof(object), typeof(List<MethodArgument>) }));
            mIlGen.Emit(OpCodes.Ret);
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Generates code that pushes the 'Value' property of the method argument 
        /// (the first parameter of the invoker method) with the specified index
        /// </summary>
        /// <param name="mIlGen"></param>
        /// <param name="inArgIndex"></param>
        /// <param name="paramType"></param>
        private static void PushArgumentValueByIndex(ILGenerator mIlGen, int inArgIndex, Type paramType)
        {
            mIlGen.Emit(OpCodes.Ldarg_0);
            mIlGen.Emit(OpCodes.Ldc_I4_S, inArgIndex);
            mIlGen.Emit(OpCodes.Callvirt, typeof(IMethodCallDescriptor).GetMethod("GetArgument"));
            mIlGen.Emit(OpCodes.Callvirt, typeof(MethodArgument).GetProperty("Value").GetGetMethod());
            mIlGen.Emit(paramType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, paramType);
        }

        /// <summary>
        /// Creates a wrapper type for the specified interface, using the passed 
        /// (dynamically generated) method and invoker helper types
        /// </summary>
        /// <param name="interfaceType">Interface type</param>
        /// <param name="methodHelperType">Method helper static class</param>
        /// <param name="invokerHelperType">Invoker helper static class</param>
        /// <returns></returns>
        private static Type CreateWrapperType(Type interfaceType, Type methodHelperType, Type invokerHelperType)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            // --- Create the type definition
            var wrapperTypeName = "Wrapper_" + interfaceType.GetHashCode() + "_" + interfaceType.FullName;
            var typeBuilder = s_ModuleBuilder.DefineType(wrapperTypeName,
                TypeAttributes.Public | TypeAttributes.BeforeFieldInit | TypeAttributes.Serializable,
                typeof(object),
                new[] { interfaceType });
            CreateWrapperFieldsAndConstructors(typeBuilder, interfaceType);

            // --- Create method bodies
            foreach (var method in GetInterfaceMethods(interfaceType))
            {
                // --- Define the method 
                var paramTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
                var methodDef = typeBuilder.DefineMethod(
                    method.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                    method.ReturnType == typeof(void) ? null : method.ReturnType,
                    paramTypes);

                // --- Emit method body
                var mIlGen = methodDef.GetILGenerator();

                // --- Create a variable for method arguments
                var inputArgs = mIlGen.DeclareLocal(typeof(List<MethodArgument>));
                mIlGen.Emit(OpCodes.Newobj, typeof(List<MethodArgument>).GetConstructor(Type.EmptyTypes));
                mIlGen.Emit(OpCodes.Stloc_S, inputArgs);

                // --- Fill up the input arguments
                var parameters = method.GetParameters();
                for (var index = 0; index < parameters.Length; index++)
                {
                    var param = parameters[index];
                    if (param.IsOut) continue;
                    mIlGen.Emit(OpCodes.Ldloc_S, inputArgs);
                    mIlGen.Emit(OpCodes.Ldstr, param.Name);
                    mIlGen.Emit(OpCodes.Ldarg_S, index + 1);
                    if (param.ParameterType.IsByRef)
                    {
                        var paramType = GetNonReferencedType(param.ParameterType);
                        if (paramType.IsValueType)
                        {
                            mIlGen.Emit(OpCodes.Ldobj, paramType);
                            mIlGen.Emit(OpCodes.Box, paramType);
                        }
                        else
                        {
                            mIlGen.Emit(OpCodes.Ldind_Ref);
                        }
                    }
                    else if (param.ParameterType.IsValueType)
                    {
                        mIlGen.Emit(OpCodes.Box, param.ParameterType);
                    }
                    mIlGen.Emit(OpCodes.Newobj, typeof(MethodArgument)
                                                    .GetConstructor(new[] { typeof(string), typeof(object) }));
                    mIlGen.Emit(OpCodes.Callvirt, typeof(List<MethodArgument>).GetMethod("Add"));
                }

                // --- Call the interceptor method with the invoker
                mIlGen.Emit(OpCodes.Ldarg_0);
                mIlGen.Emit(OpCodes.Ldfld, s_TargetField);  // --- Push '_target'
                mIlGen.Emit(OpCodes.Ldloc_S, inputArgs);    // --- Push 'args'
                var opName = GetMethodHelperName(method);
                mIlGen.Emit(OpCodes.Ldsfld,                 // --- Push the helper method information to the stack
                            methodHelperType.GetField(opName, BindingFlags.Public | BindingFlags.Static));
                mIlGen.Emit(OpCodes.Ldnull);                // --- Push the invoker method to the stack
                mIlGen.Emit(OpCodes.Ldftn, invokerHelperType.GetMethod(opName, new[] { typeof(IMethodCallDescriptor), interfaceType }));
                var genericFunc3 = typeof(Func<,,>);
                var func3 = genericFunc3.MakeGenericType(new[] { typeof(IMethodCallDescriptor), interfaceType, typeof(IMethodResultDescriptor) });
                mIlGen.Emit(OpCodes.Newobj, func3.GetConstructors()[0]);
                mIlGen.Emit(OpCodes.Ldarg_0);
                mIlGen.Emit(OpCodes.Ldfld, s_AspectField);
                var execWrapperGeneric = typeof(Interceptor).GetMethod("ExecuteWrappedCall");
                var execWrapperConstructed = execWrapperGeneric.MakeGenericMethod(new[] { interfaceType });
                mIlGen.Emit(OpCodes.Call, execWrapperConstructed);
                var response = mIlGen.DeclareLocal(typeof(IMethodResultDescriptor));
                mIlGen.Emit(OpCodes.Stloc_S, response);

                // --- Set the value of output variables
                var outArgIndex = 0;
                for (var index = 0; index < parameters.Length; index++)
                {
                    var param = parameters[index];
                    var paramType = param.ParameterType;
                    if (!param.IsOut && !paramType.IsByRef) continue;

                    paramType = GetNonReferencedType(paramType);
                    mIlGen.Emit(OpCodes.Ldarg_S, index + 1); // --- Push argument address to the stack
                    mIlGen.Emit(OpCodes.Ldloc_S, response);
                    mIlGen.Emit(OpCodes.Ldc_I4_S, outArgIndex);
                    mIlGen.Emit(OpCodes.Callvirt, typeof(IMethodResultDescriptor).GetMethod("GetOutputArgument"));
                    mIlGen.Emit(OpCodes.Callvirt, typeof(MethodArgument).GetProperty("Value").GetGetMethod());
                    if (paramType.IsValueType)
                    {
                        mIlGen.Emit(OpCodes.Unbox_Any, paramType);
                        mIlGen.Emit(OpCodes.Stobj, paramType);
                    }
                    else
                    {
                        mIlGen.Emit(OpCodes.Castclass, paramType);
                        mIlGen.Emit(OpCodes.Stind_Ref);
                    }
                    outArgIndex++;
                }

                // --- Set the return value
                var returnType = method.ReturnType;
                if (returnType != typeof (void))
                {
                    mIlGen.Emit(OpCodes.Ldloc_S, response);
                    mIlGen.Emit(OpCodes.Callvirt, typeof(IMethodResultDescriptor).GetProperty("ReturnValue").GetGetMethod());
                    mIlGen.Emit(returnType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, returnType);
                }

                // --- Return back from the operation
                mIlGen.Emit(OpCodes.Ret);
            }

            // --- Complete the type
            return typeBuilder.CreateType();
            // ReSharper restore AssignNullToNotNullAttribute
        }

        private static void CreateWrapperFieldsAndConstructors(TypeBuilder builder, Type interfaceType)
        {
            // --- Define fields
            s_TargetField = builder.DefineField(TARGET_FIELD, interfaceType,
                FieldAttributes.Private);
            s_AspectField = builder.DefineField(ASPECTS_FIELD, typeof(AspectChain),
                FieldAttributes.Private);

            // --- Define instance constructor
            var ctorDef = builder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new[] { interfaceType, typeof(AspectChain) });

            // --- Emit constructor body
            // ReSharper disable AssignNullToNotNullAttribute
            var ctorIlGen = ctorDef.GetILGenerator();
            ctorIlGen.Emit(OpCodes.Ldarg_0);
            ctorIlGen.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            ctorIlGen.Emit(OpCodes.Ldarg_0);
            ctorIlGen.Emit(OpCodes.Ldarg_1);
            ctorIlGen.Emit(OpCodes.Stfld, s_TargetField);
            ctorIlGen.Emit(OpCodes.Ldarg_0);
            ctorIlGen.Emit(OpCodes.Ldarg_2);
            ctorIlGen.Emit(OpCodes.Stfld, s_AspectField);
            ctorIlGen.Emit(OpCodes.Ret);
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Ensures that this class has an AssemblyBuilder object to use
        /// </summary>
        private static void EnsureModuleBuilder()
        {
            if (s_AssemblyBuilder != null) return;
            var name = new AssemblyName("InterceptorAssembly");
            s_AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave);
            s_ModuleBuilder = s_AssemblyBuilder.DefineDynamicModule(name.Name, "SampleAssembly.dll");
        }

        /// <summary>
        /// Gets a non-referenced type from a referenced type
        /// </summary>
        /// <param name="referencedType"></param>
        /// <returns></returns>
        private static Type GetNonReferencedType(Type referencedType)
        {
            var nonRefType = referencedType.FullName;
            nonRefType = nonRefType.Substring(0, nonRefType.Length - 1);
            return Type.GetType(nonRefType);
        }

        /// <summary>
        /// Gets the attributes related to aspects in regard to the <paramref name="methodInfo"/>
        /// input parameter.
        /// </summary>
        /// <param name="methodInfo">
        /// The descriptor of the method to obtain aspect information for
        /// </param>
        /// <param name="activatorType">Optional type that activates an aspect</param>
        /// <returns>
        /// A <see cref="List{MethodAspectedAttribute}"/> instance that contains all aspect
        /// attributes. The attributes are in order of their priority, so item with index 0 is
        /// to be called first. This method merges assembly, type and method level attributes
        /// according to merging rules, handling aspect order and overriding.
        /// </returns>
        private static List<AspectAttributeBase> GetAspects(MethodBase methodInfo, Type activatorType)
        {
            // --- Check cache for type attributes
            List<AspectAttributeBase> attrs;
            var methodKey = new MethodKey(methodInfo.Name, activatorType);
            if (s_Methods.TryGetValue(methodKey, out attrs)) return attrs;

            // --- Obtain AspectAttributeBase instances decorating the method and
            // --- combine them with aspect attributes belonging to types.
            attrs = new List<AspectAttributeBase>(GetTypeAspects(activatorType));
            var parameters = methodInfo.GetParameters();
            var paramTypes = new Type[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                paramTypes[i] = parameters[i].ParameterType;
            }
            // ReSharper disable PossibleNullReferenceException
            var serviceMethod = activatorType.GetMethod(methodInfo.Name,
                // ReSharper restore PossibleNullReferenceException
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
                CallingConventions.Any, paramTypes, null);
            if (serviceMethod != null)
            {
                // --- Method is available in the interface
                var methodAttrs = serviceMethod
                    .GetCustomAttributes(typeof(AspectAttributeBase), true)
                    .Cast<AspectAttributeBase>()
                    .ToList();
                methodAttrs = ResolveCompositeAspects(methodAttrs);
                CombineAspects(attrs, methodAttrs);
            }
            s_Methods[methodKey] = attrs;
            return attrs;
        }

        /// <summary>
        /// Helper method to obtain aspect information based on <paramref name="declaringType"/>.
        /// </summary>
        /// <param name="declaringType">The type to collect aspect attributes for.</param>
        /// <returns>
        /// A <see cref="IEnumerable{MethodAspectedAttribute}"/> instance that contains type aspect
        /// attributes. The attributes are in order of their priority, so item with index 0 is
        /// to be called first. This method merges assembly and type level attributes
        /// according to merging rules, handling aspect order and overriding.
        /// </returns>
        private static IEnumerable<AspectAttributeBase> GetTypeAspects(Type declaringType)
        {
            // --- Check cache for type attributes
            List<AspectAttributeBase> attrs;
            if (s_Types.TryGetValue(declaringType, out attrs)) return attrs;

            // --- Obtain OperationAspectAttribute instances decorating the type and
            // --- combine them with aspect attributes belonging to the assembly.
            attrs = new List<AspectAttributeBase>();
            var typeAttrs = declaringType
                .GetCustomAttributes(typeof(AspectAttributeBase), true)
                .Cast<AspectAttributeBase>()
                .ToList();
            typeAttrs = ResolveCompositeAspects(typeAttrs);
            CombineAspects(attrs, typeAttrs);
            s_Types[declaringType] = attrs;
            return attrs;
        }

        /// <summary>
        /// This method resolves the <see cref="CompositeAspectAttributeBase"/> instances
        /// in the specified list of attributes. Each attribute decorating the composite
        /// attributed is added to the result list.
        /// </summary>
        /// <param name="origAttrs">Original attributes</param>
        /// <returns>List of attributes after resolving the composite </returns>
        private static List<AspectAttributeBase> ResolveCompositeAspects(
            IEnumerable<AspectAttributeBase> origAttrs)
        {
            var result = new List<AspectAttributeBase>();
            foreach (var item in origAttrs)
            {
                if (item is CompositeAspectAttributeBase)
                {
                    // --- Go through the OperationAspect attributes decorating the composite attribute
                    result
                        .AddRange(item.GetType()
                        .GetCustomAttributes(typeof(AspectAttributeBase), false)
                        .Cast<AspectAttributeBase>());
                }
                else
                {
                    // --- Single attribute
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// This method combines two lists of aspects according to merging and ordering rules.
        /// </summary>
        /// <param name="origAttrs">Original list of aspect attributes</param>
        /// <param name="newAttrs">
        /// New list of aspect attributes to combine with the original
        /// </param>
        private static void CombineAspects(List<AspectAttributeBase> origAttrs,
            IEnumerable<AspectAttributeBase> newAttrs)
        {
            // --- Mark overridden attributes for deletion
            var toDelete = new List<AspectAttributeBase>();
            // ReSharper disable PossibleMultipleEnumeration
            foreach (var attr in newAttrs.Where(attr => attr.Override))
            {
                // ReSharper disable AccessToModifiedClosure
                toDelete.AddRange(origAttrs.Where(origAttr => origAttr.GetType() == attr.GetType()));
                // ReSharper restore AccessToModifiedClosure
            }

            // --- Delete overridden attributes
            foreach (var item in toDelete)
                origAttrs.Remove(item);

            // --- Add new attributes
            origAttrs.AddRange(newAttrs.OrderBy(attr => attr.Order));
            // ReSharper restore PossibleMultipleEnumeration
        }

        // ReSharper disable InconsistentNaming
        
        private static MethodInfo GetTypeFromHandle_Method
        {
            get { return typeof (Type).GetMethod("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static); }
        }

        private static MethodInfo GetMethods_Method
        {
            get { return typeof(Type).GetMethod("GetMethods", Type.EmptyTypes); }
        }

        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Gets all methods of the specified interface
        /// </summary>
        /// <param name="type">Interface type</param>
        /// <returns>Methods implemented by directly or indirectly</returns>
        private static List<MethodInfo> GetInterfaceMethods(Type type)
        {
            var methods = new List<MethodInfo>(type.GetMethods());
            var interfaces = type.GetInterfaces();
            foreach (var intf in interfaces)
            {
                methods.AddRange(intf.GetMethods());
            }
            return methods;
        }

        /// <summary>
        /// Gets all interfaces implemented by the specific type
        /// </summary>
        /// <param name="type">Type to get the inherited interfaces for</param>
        /// <returns>List of interfaces</returns>
        private static IEnumerable<Type> GetInterfaceChain(Type type)
        {
            var result = new List<Type> {type};
            result.AddRange(type.GetInterfaces());
            return result;
        }

        /// <summary>
        /// Gets the name of the method to be used in helper types
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static string GetMethodHelperName(MethodInfo method)
        {
            // ReSharper disable once PossibleNullReferenceException
            return string.Format("{0}_{1}_{2}",
                method.DeclaringType.FullName,
                method.Name,
                method.GetHashCode());
        }

        /// <summary>
        /// Used temporarily for invoker method generation
        /// </summary>
        private struct ParameterDescriptor
        {
            public string Name;
            public Type Type;
            public LocalBuilder LocalVariable;
            public int InArgIndex;
            public int OutArgIndex;
        }

        /// <summary>
        /// This helper class describes a method key in the cache.
        /// </summary>
        private class MethodKey : Tuple<string, Type>
        {
            public MethodKey(string name, Type type)
                : base(name, type) { }
        }
    }
}