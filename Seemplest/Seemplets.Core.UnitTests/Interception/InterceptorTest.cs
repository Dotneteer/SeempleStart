using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seemplest.Core.Exceptions;
using Seemplest.Core.Interception;
using Seemplest.Core.UnitTests.DependencyInjection;
using SoftwareApproach.TestingExtensions;

namespace Seemplest.Core.UnitTests.Interception
{
    [TestClass]
    public class InterceptorTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Interceptor.ResetCache();
            Aspect1.Reset();
            Aspect2.Reset();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WrongTypeFails1()
        {
            // --- Act
            Interceptor.GetInterceptedObject<IWrongInterface1<int>>(new WrongInterface1<int>(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WrongTypeFails2()
        {
            // --- Act
            Interceptor.GetInterceptedObject<IWrongInterface2>(new WrongInterface2(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WrongTypeFails3()
        {
            // --- Act
            Interceptor.GetInterceptedObject<IWrongInterface3>(new WrongInterface3(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WrongTypeFails4()
        {
            // --- Act
            Interceptor.GetInterceptedObject<IWrongInterface4>(new WrongInterface4(), null);
        }

        [TestMethod]
        public void InterceptorClassGenerated()
        {
            // --- Arrange
            var foo = new Foo();

            // --- Act
            var wrappedType = Interceptor.GetInterceptedObject<IFoo>(foo, null);
            var wrappedType1 = Interceptor.GetInterceptedObject<ISampleService>(new SampleService(), null);

            // --- Assert
            wrappedType.ShouldNotBeNull();
            wrappedType.Operation1(0);
            wrappedType.Operation2(1, 2).ShouldEqual(0);
            var refGuid = Guid.NewGuid();
            wrappedType.Operation7(ref refGuid);
            string outVal1;
            int refVal1 = 2;
            wrappedType.Operation3(1, out outVal1, ref refVal1);
            int outVal;
            var val = "2";
            wrappedType.Operation4(1, ref val, out outVal);
            wrappedType1.ShouldNotBeNull();
        }

        [TestMethod]
        public void GetInterceptedObjectIsThreadSafe()
        {
            // --- Act
            Parallel.Invoke(
                () => Interceptor.GetInterceptedObject<ISampleService>(new SampleService(), null), 
                () => Interceptor.GetInterceptedObject<ISampleService>(new SampleService(), null));
        }

        [TestMethod]
        public void Operation1WorksWithSimpleAspect()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect1());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            wrapped.Operation1(123);

            // --- Assert
            Aspect1.OnEntryVisited.ShouldBeTrue();
            Aspect1.OnExitVisited.ShouldBeTrue();
            Aspect1.OnSuccessVisited.ShouldBeTrue();
            Aspect1.OnExceptionVisited.ShouldBeFalse();
        }

        [TestMethod]
        public void Operation1WorksWithSimpleAspectWrapped()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect1());

            // --- Act
            var wrapped = Interceptor.GetInterceptedObject<IFoo>(target, aspects);
            wrapped.Operation1(123);

            // --- Assert
            Aspect1.OnEntryVisited.ShouldBeTrue();
            Aspect1.OnExitVisited.ShouldBeTrue();
            Aspect1.OnSuccessVisited.ShouldBeTrue();
            Aspect1.OnExceptionVisited.ShouldBeFalse();
        }

        [TestMethod]
        public void Operation1ExceptionWorksWithSimpleAspect()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect1());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            var exceptionCaught = false;
            try
            {
                wrapped.Operation1(-1);
            }
            catch (Exception ex)
            {
                exceptionCaught = ex is InvalidOperationException;
            }

            // --- Assert
            exceptionCaught.ShouldBeTrue();
            Aspect1.OnEntryVisited.ShouldBeTrue();
            Aspect1.OnExitVisited.ShouldBeTrue();
            Aspect1.OnSuccessVisited.ShouldBeFalse();
            Aspect1.OnExceptionVisited.ShouldBeTrue();
        }

        [TestMethod]
        public void Operation1ExceptionWorksWithSimpleAspectWrapped()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect1());

            // --- Act
            var wrapped = Interceptor.GetInterceptedObject<IFoo>(target, aspects);
            var exceptionCaught = false;
            try
            {
                wrapped.Operation1(-1);
            }
            catch (Exception ex)
            {
                exceptionCaught = ex is InvalidOperationException;
            }

            // --- Assert
            exceptionCaught.ShouldBeTrue();
            Aspect1.OnEntryVisited.ShouldBeTrue();
            Aspect1.OnExitVisited.ShouldBeTrue();
            Aspect1.OnSuccessVisited.ShouldBeFalse();
            Aspect1.OnExceptionVisited.ShouldBeTrue();
        }

        [TestMethod]
        public void Operation2WorksWithSimpleAspect()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect1());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            var result = wrapped.Operation2(8, 3);

            // --- Assert
            result.ShouldEqual(2);
            Aspect1.OnEntryVisited.ShouldBeTrue();
            Aspect1.OnExitVisited.ShouldBeTrue();
            Aspect1.OnSuccessVisited.ShouldBeTrue();
            Aspect1.OnExceptionVisited.ShouldBeFalse();
        }

        [TestMethod]
        public void Operation2WorksWithBodySkipper()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new BodySkipperAspect());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            var result = wrapped.Operation2(8, 3);

            // --- Assert
            result.ShouldEqual(2);
            BodySkipperAspect.OnEntryVisited.ShouldBeTrue();
            BodySkipperAspect.OnExitVisited.ShouldBeFalse();
            BodySkipperAspect.OnSuccessVisited.ShouldBeFalse();
            BodySkipperAspect.OnExceptionVisited.ShouldBeFalse();
        }

        [TestMethod]
        public void Operation2WorksWithNullAspects()
        {
            // --- Arrange
            var target = new Foo();

            // --- Act
            var wrapped = new WrappedFoo(target, null);
            var exceptionCaught = false;
            try
            {
                wrapped.Operation2(8, 0);
            }
            catch (Exception ex)
            {
                exceptionCaught = ex is DivideByZeroException;
            }

            // --- Assert
            exceptionCaught.ShouldBeTrue();
        }

        [TestMethod]
        public void Operation2WorksWithSimpleAspectWrapped()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect1());

            // --- Act
            var wrapped = Interceptor.GetInterceptedObject<IFoo>(target, aspects); 
            var result = wrapped.Operation2(8, 3);

            // --- Assert
            result.ShouldEqual(2);
            Aspect1.OnEntryVisited.ShouldBeTrue();
            Aspect1.OnExitVisited.ShouldBeTrue();
            Aspect1.OnSuccessVisited.ShouldBeTrue();
            Aspect1.OnExceptionVisited.ShouldBeFalse();
        }

        [TestMethod]
        public void Operation2ExceptionWorksWithSimpleAspect()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect1());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            var exceptionCaught = false;
            try
            {
                wrapped.Operation2(8, 0);
            }
            catch (Exception ex)
            {
                exceptionCaught = ex is DivideByZeroException;
            }

            // --- Assert
            exceptionCaught.ShouldBeTrue();
            Aspect1.OnEntryVisited.ShouldBeTrue();
            Aspect1.OnExitVisited.ShouldBeTrue();
            Aspect1.OnSuccessVisited.ShouldBeFalse();
            Aspect1.OnExceptionVisited.ShouldBeTrue();
        }

        [TestMethod]
        public void Operation2ExceptionWorksWithSimpleAspectWrapped()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect1());

            // --- Act
            var wrapped = Interceptor.GetInterceptedObject<IFoo>(target, aspects);
            var exceptionCaught = false;
            try
            {
                wrapped.Operation2(8, 0);
            }
            catch (Exception ex)
            {
                exceptionCaught = ex is DivideByZeroException;
            }

            // --- Assert
            exceptionCaught.ShouldBeTrue();
            Aspect1.OnEntryVisited.ShouldBeTrue();
            Aspect1.OnExitVisited.ShouldBeTrue();
            Aspect1.OnSuccessVisited.ShouldBeFalse();
            Aspect1.OnExceptionVisited.ShouldBeTrue();
        }

        [TestMethod]
        public void Operation3WorksWithSimpleAspect()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect1());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            string outArg;
            int refArg = 3;
            var result = wrapped.Operation3(8, out outArg, ref refArg);

            // --- Assert
            result.ShouldEqual("Done.");
            outArg.ShouldEqual("Hello 2");
            refArg.ShouldEqual(4);
            Aspect1.OnEntryVisited.ShouldBeTrue();
            Aspect1.OnExitVisited.ShouldBeTrue();
            Aspect1.OnSuccessVisited.ShouldBeTrue();
            Aspect1.OnExceptionVisited.ShouldBeFalse();
        }

        [TestMethod]
        public void Operation3WorksWithSimpleAspectWrapped()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect1());

            // --- Act
            var wrapped = Interceptor.GetInterceptedObject<IFoo>(target, aspects);
            string outArg;
            int refArg = 3;
            var result = wrapped.Operation3(8, out outArg, ref refArg);

            // --- Assert
            result.ShouldEqual("Done.");
            outArg.ShouldEqual("Hello 2");
            refArg.ShouldEqual(4);
            Aspect1.OnEntryVisited.ShouldBeTrue();
            Aspect1.OnExitVisited.ShouldBeTrue();
            Aspect1.OnSuccessVisited.ShouldBeTrue();
            Aspect1.OnExceptionVisited.ShouldBeFalse();
        }

        [TestMethod]
        public void Operation3ExceptionWorksWithSimpleAspect()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect1());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            var exceptionCaught = false;
            try
            {
                string outArg;
                int refArg = 0;
                wrapped.Operation3(8, out outArg, ref refArg);
            }
            catch (Exception ex)
            {
                exceptionCaught = ex is DivideByZeroException;
            }

            // --- Assert
            exceptionCaught.ShouldBeTrue();
            Aspect1.OnEntryVisited.ShouldBeTrue();
            Aspect1.OnExitVisited.ShouldBeTrue();
            Aspect1.OnSuccessVisited.ShouldBeFalse();
            Aspect1.OnExceptionVisited.ShouldBeTrue();
        }

        [TestMethod]
        public void Operation4WorksWithSimpleAspect()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect1());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            var refArg = "3";
            int outArg;
            wrapped.Operation4(8, ref refArg, out outArg);

            // --- Assert
            refArg.ShouldEqual("3Hello 8");
            outArg.ShouldEqual(1);
            Aspect1.OnEntryVisited.ShouldBeTrue();
            Aspect1.OnExitVisited.ShouldBeTrue();
            Aspect1.OnSuccessVisited.ShouldBeTrue();
            Aspect1.OnExceptionVisited.ShouldBeFalse();
        }

        [TestMethod]
        public void Operation5WorksWithSimpleAspect()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect1());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            wrapped.Operation5();

            // --- Assert
            Aspect1.OnEntryVisited.ShouldBeTrue();
            Aspect1.OnExitVisited.ShouldBeTrue();
            Aspect1.OnSuccessVisited.ShouldBeTrue();
            Aspect1.OnExceptionVisited.ShouldBeFalse();
        }

        [TestMethod]
        public void Operation6WorksWithSimpleAspect()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect1());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            var result = wrapped.Operation6();

            // --- Assert
            result.ShouldEqual("Done.");
            Aspect1.OnEntryVisited.ShouldBeTrue();
            Aspect1.OnExitVisited.ShouldBeTrue();
            Aspect1.OnSuccessVisited.ShouldBeTrue();
            Aspect1.OnExceptionVisited.ShouldBeFalse();
        }

        [TestMethod]
        public void Operation1WorksWithCompoundAspect()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect2(), new Aspect3());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            wrapped.Operation1(123);

            // --- Assert
            Aspect2.OnEntryVisited.ShouldEqual("12");
            Aspect2.OnExitVisited.ShouldEqual("21");
            Aspect2.OnSuccessVisited.ShouldEqual("21");
            Aspect2.OnExceptionVisited.ShouldEqual("");
        }

        [TestMethod]
        public void Operation1ExceptionWorksWithCompoundAspect()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect2(), new Aspect3());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            var exceptionCaught = false;
            try
            {
                wrapped.Operation1(-1);
            }
            catch (Exception ex)
            {
                exceptionCaught = ex is InvalidOperationException;
            }

            // --- Assert
            exceptionCaught.ShouldBeTrue();
            Aspect2.OnEntryVisited.ShouldEqual("12");
            Aspect2.OnExitVisited.ShouldEqual("21");
            Aspect2.OnSuccessVisited.ShouldEqual("");
            Aspect2.OnExceptionVisited.ShouldEqual("21");
        }

        [TestMethod]
        public void Operation2WorksWithCompoundAspect()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect2(), new Aspect3());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            var result = wrapped.Operation2(8, 3);

            // --- Assert
            result.ShouldEqual(2);
            Aspect2.OnEntryVisited.ShouldEqual("12");
            Aspect2.OnExitVisited.ShouldEqual("21");
            Aspect2.OnSuccessVisited.ShouldEqual("21");
            Aspect2.OnExceptionVisited.ShouldEqual("");
        }

        [TestMethod]
        public void Operation2ExceptionWorksWithCompoundAspect()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect2(), new Aspect3());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            var exceptionCaught = false;
            try
            {
                wrapped.Operation2(8, 0);
            }
            catch (Exception ex)
            {
                exceptionCaught = ex is DivideByZeroException;
            }

            // --- Assert
            exceptionCaught.ShouldBeTrue();
            Aspect2.OnEntryVisited.ShouldEqual("12");
            Aspect2.OnExitVisited.ShouldEqual("21");
            Aspect2.OnSuccessVisited.ShouldEqual("");
            Aspect2.OnExceptionVisited.ShouldEqual("21");
        }

        [TestMethod]
        public void Operation3WorksWithCompoudAspect()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect2(), new Aspect3());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            string outArg;
            int refArg = 3;
            var result = wrapped.Operation3(8, out outArg, ref refArg);

            // --- Assert
            result.ShouldEqual("Done.");
            outArg.ShouldEqual("Hello 2");
            refArg.ShouldEqual(4);
            Aspect2.OnEntryVisited.ShouldEqual("12");
            Aspect2.OnExitVisited.ShouldEqual("21");
            Aspect2.OnSuccessVisited.ShouldEqual("21");
            Aspect2.OnExceptionVisited.ShouldEqual("");
        }

        [TestMethod]
        public void Operation3ExceptionWorksWithCompoundAspect()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect2(), new Aspect3());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            var exceptionCaught = false;
            try
            {
                string outArg;
                int refArg = 0;
                wrapped.Operation3(8, out outArg, ref refArg);
            }
            catch (Exception ex)
            {
                exceptionCaught = ex is DivideByZeroException;
            }

            // --- Assert
            exceptionCaught.ShouldBeTrue();
            Aspect2.OnEntryVisited.ShouldEqual("12");
            Aspect2.OnExitVisited.ShouldEqual("21");
            Aspect2.OnSuccessVisited.ShouldEqual("");
            Aspect2.OnExceptionVisited.ShouldEqual("21");
        }

        [TestMethod]
        public void Operation4WorksWithCompoundAspect()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect2(), new Aspect3());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            var refArg = "3";
            int outArg;
            wrapped.Operation4(8, ref refArg, out outArg);

            // --- Assert
            refArg.ShouldEqual("3Hello 8");
            outArg.ShouldEqual(1);
            Aspect2.OnEntryVisited.ShouldEqual("12");
            Aspect2.OnExitVisited.ShouldEqual("21");
            Aspect2.OnSuccessVisited.ShouldEqual("21");
            Aspect2.OnExceptionVisited.ShouldEqual("");
        }

        [TestMethod]
        public void Operation5WorksWithCompoundAspect()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect2(), new Aspect3());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            wrapped.Operation5();

            // --- Assert
            Aspect2.OnEntryVisited.ShouldEqual("12");
            Aspect2.OnExitVisited.ShouldEqual("21");
            Aspect2.OnSuccessVisited.ShouldEqual("21");
            Aspect2.OnExceptionVisited.ShouldEqual("");
        }

        [TestMethod]
        public void Operation6WorksWithCompoundAspect()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect2(), new Aspect3());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            var result = wrapped.Operation6();

            // --- Assert
            result.ShouldEqual("Done.");
            Aspect2.OnEntryVisited.ShouldEqual("12");
            Aspect2.OnExitVisited.ShouldEqual("21");
            Aspect2.OnSuccessVisited.ShouldEqual("21");
            Aspect2.OnExceptionVisited.ShouldEqual("");
        }

        [TestMethod]
        public void InfrastructureExceptionIsCaughtInOnEntry()
        {
            // --- Arrange
            var target = new Foo();
            Aspect4.Reset();
            var aspects = new AspectChain(new Aspect4());

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            var exceptionCaught = false;
            try
            {
                string outArg;
                int refArg = 3;
                wrapped.Operation3(8, out outArg, ref refArg);
            }
            catch (AspectInfrastructureException)
            {
                exceptionCaught = true;
            }

            // --- Assert
            exceptionCaught.ShouldBeTrue();
            Aspect4.OnEntryVisited.ShouldBeTrue();
            Aspect4.OnExitVisited.ShouldBeFalse();
            Aspect4.OnSuccessVisited.ShouldBeFalse();
            Aspect4.OnExceptionVisited.ShouldBeFalse();
        }

        [TestMethod]
        public void InfrastructureExceptionIsCaughtInOnExit()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect5());
            Aspect5.Reset();

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            var exceptionCaught = false;
            try
            {
                string outArg;
                int refArg = 3;
                wrapped.Operation3(8, out outArg, ref refArg);
            }
            catch (AspectInfrastructureException)
            {
                exceptionCaught = true;
            }

            // --- Assert
            exceptionCaught.ShouldBeTrue();
            Aspect5.OnEntryVisited.ShouldBeTrue();
            Aspect5.OnExitVisited.ShouldBeTrue();
            Aspect5.OnSuccessVisited.ShouldBeFalse();
            Aspect5.OnExceptionVisited.ShouldBeFalse();
        }

        [TestMethod]
        public void InfrastructureExceptionIsCaughtInOnSuccess()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect6());
            Aspect6.Reset();

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            var exceptionCaught = false;
            try
            {
                string outArg;
                int refArg = 3;
                wrapped.Operation3(8, out outArg, ref refArg);
            }
            catch (AspectInfrastructureException)
            {
                exceptionCaught = true;
            }

            // --- Assert
            exceptionCaught.ShouldBeTrue();
            Aspect6.OnEntryVisited.ShouldBeTrue();
            Aspect6.OnExitVisited.ShouldBeTrue();
            Aspect6.OnSuccessVisited.ShouldBeTrue();
            Aspect6.OnExceptionVisited.ShouldBeFalse();
        }

        [TestMethod]
        public void InfrastructureExceptionIsCaughtInOnException()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new Aspect7());
            Aspect7.Reset();

            // --- Act
            var wrapped = new WrappedFoo(target, aspects);
            var exceptionCaught = false;
            try
            {
                string outArg;
                int refArg = 0;
                wrapped.Operation3(8, out outArg, ref refArg);
            }
            catch (AspectInfrastructureException)
            {
                exceptionCaught = true;
            }

            // --- Assert
            exceptionCaught.ShouldBeTrue();
            Aspect7.OnEntryVisited.ShouldBeTrue();
            Aspect7.OnExitVisited.ShouldBeTrue();
            Aspect7.OnSuccessVisited.ShouldBeFalse();
            Aspect7.OnExceptionVisited.ShouldBeTrue();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetInterceptedObjectFailsWithNullTarget()
        {
            // --- Act
            Interceptor.GetInterceptedObject(typeof (ISampleService), null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetInterceptedObjectFailsWithNullServiceType()
        {
            // --- Act
            Interceptor.GetInterceptedObject(null, "hello", null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetInterceptedObjectFailsWithUnrelatedTypes()
        {
            // --- Act
            Interceptor.GetInterceptedObject(typeof(ISampleService), "hello", null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetInterceptedObjectFailsWithNonInterface()
        {
            // --- Act
            Interceptor.GetInterceptedObject(typeof(string), "hello", null);
        }

        [TestMethod]
        public void FaultyAspectIsCaught1()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new FaultyAspect1());

            // --- Act
            var wrapped = Interceptor.GetInterceptedObject<IFoo>(target, aspects);
            Exception innerEx = null;
            try
            {
                wrapped.Operation2(8, 3);
            }
            catch (AspectInfrastructureException ex)
            {
                innerEx = ex.InnerException;
            }

            // --- Assert
            innerEx.ShouldBeOfType(typeof (InvalidOperationException));
        }

        [TestMethod]
        public void FaultyAspectIsCaught2()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new FaultyAspect2());

            // --- Act
            var wrapped = Interceptor.GetInterceptedObject<IFoo>(target, aspects);
            Exception innerEx = null;
            try
            {
                wrapped.Operation2(8, 3);
            }
            catch (AspectInfrastructureException ex)
            {
                innerEx = ex.InnerException;
            }

            // --- Assert
            innerEx.ShouldBeOfType(typeof(InvalidOperationException));
        }

        [TestMethod]
        public void FaultyAspectIsCaught3()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new FaultyAspect3());

            // --- Act
            var wrapped = Interceptor.GetInterceptedObject<IFoo>(target, aspects);
            Exception innerEx = null;
            try
            {
                wrapped.Operation2(8, 3);
            }
            catch (AspectInfrastructureException ex)
            {
                innerEx = ex.InnerException;
            }

            // --- Assert
            innerEx.ShouldBeOfType(typeof(InvalidOperationException));
        }

        [TestMethod]
        public void FaultyAspectIsCaught4()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new FaultyAspect4());

            // --- Act
            var wrapped = Interceptor.GetInterceptedObject<IFoo>(target, aspects);
            Exception innerEx = null;
            try
            {
                wrapped.Operation2(8, 0);
            }
            catch (AspectInfrastructureException ex)
            {
                innerEx = ex.InnerException;
            }

            // --- Assert
            innerEx.ShouldBeOfType(typeof(InvalidOperationException));
        }

        [TestMethod]
        public void FaultyAspectIsCaught5()
        {
            // --- Arrange
            var target = new Foo();
            var aspects = new AspectChain(new FaultyAspect5());

            // --- Act
            var wrapped = Interceptor.GetInterceptedObject<IFoo>(target, aspects);
            var exceptionCaught = false;
            try
            {
                wrapped.Operation2(8, 0);
            }
            catch (AspectInfrastructureException)
            {
                exceptionCaught = true;
            }

            // --- Assert
            exceptionCaught.ShouldBeTrue();
        }
    }

    public interface IFoo
    {
        void Operation1(int a);
        int Operation2(int a, int b);
        string Operation3(int a, out string b, ref int c);
        void Operation4(int a, ref string b, out int c);
        void Operation5();
        string Operation6();
        void Operation7(ref Guid a);
    }

    public class Foo : IFoo
    {
        public void Operation1(int a)
        {
            if (a < 0) throw new InvalidOperationException();
        }

        public int Operation2(int a, int b)
        {
            return a / b;
        }

        public string Operation3(int a, out string b, ref int c)
        {
            b = "Hello " + a/c;
            c = c + 1;
            return "Done.";
        }

        public void Operation4(int a, ref string b, out int c)
        {
            c = 1;
            b = b + "Hello " + a/c;
        }

        public void Operation5()
        {
        }

        public string Operation6()
        {
            return "Done.";
        }

        public void Operation7(ref Guid a)
        {
            var b = a.ToByteArray();
            b[0]++;
            a = new Guid(b);
        }
    }

    internal static class TargetMethodHelper
    {
        public static MethodInfo Operation1Method;
        public static MethodInfo Operation2Method;
        public static MethodInfo Operation3Method;
        public static MethodInfo Operation4Method;
        public static MethodInfo Operation5Method;
        public static MethodInfo Operation6Method;
        public static MethodInfo Operation7Method;


        static TargetMethodHelper()
        {
            var types = typeof (IFoo).GetMethods();
            Operation1Method = types[0];
            Operation2Method = types[1];
            Operation3Method = types[2];
            Operation4Method = types[3];
            Operation5Method = types[4];
            Operation6Method = types[5];
            Operation7Method = types[6];
        }

        public static IMethodResultDescriptor IFoo_Operation1(IMethodCallDescriptor args, IFoo target)
        {
            target.Operation1((int)args.GetArgument(0).Value);
            var result = new MethodResultDescriptor(false, null, null);
            return result;
        }

        public static IMethodResultDescriptor IFoo_Operation2(IMethodCallDescriptor args, IFoo target)
        {
            var value = target.Operation2((int)args.GetArgument(0).Value, (int)args.GetArgument(1).Value);
            var result = new MethodResultDescriptor(true, value, null);
            return result;
        }

        public static IMethodResultDescriptor IFoo_Operation3(IMethodCallDescriptor args, IFoo target)
        {
            string outArg1;
            var refArg2 = (int)args.GetArgument(1).Value;
            var value = target.Operation3((int)args.GetArgument(0).Value, out outArg1, ref refArg2);
            var outArgs = new List<MethodArgument>
                {
                    new MethodArgument("b", outArg1),
                    new MethodArgument("c", refArg2)
                };
            var result = new MethodResultDescriptor(true, value, outArgs);
            return result;
        }

        public static IMethodResultDescriptor IFoo_Operation4(IMethodCallDescriptor args, IFoo target)
        {
            var refArg1 = (string)args.GetArgument(1).Value;
            int outArg2;
            target.Operation4((int)args.GetArgument(0).Value, ref refArg1, out outArg2);
            var outArgs = new List<MethodArgument>
                {
                    new MethodArgument("b", refArg1),
                    new MethodArgument("c", outArg2)
                };
            var result = new MethodResultDescriptor(false, null, outArgs);
            return result;
        }

        public static IMethodResultDescriptor IFoo_Operation5(IMethodCallDescriptor args, IFoo target)
        {
            target.Operation5();
            var result = new MethodResultDescriptor(false, null, null);
            return result;
        }

        public static IMethodResultDescriptor IFoo_Operation6(IMethodCallDescriptor args, IFoo target)
        {
            var value = target.Operation6();
            var result = new MethodResultDescriptor(true, value, null);
            return result;
        }

        public static IMethodResultDescriptor IFoo_Operation7(IMethodCallDescriptor args, IFoo target)
        {
            var refArg1 = (Guid)args.GetArgument(0).Value;
            target.Operation7(ref refArg1);
            var outArgs = new List<MethodArgument>
                {
                    new MethodArgument("a", refArg1)
                };
            var result = new MethodResultDescriptor(false, null, outArgs);
            return result;
        }

    }

    public class WrappedFoo: IFoo
    {
        private readonly IFoo _target;
        private readonly AspectChain _aspects;

        public WrappedFoo(IFoo target, AspectChain aspects)
        {
            _target = target;
            _aspects = aspects;
        }

        public void Operation1(int a)
        {
            var args = new List<MethodArgument>
                {
                    new MethodArgument("a", a)
                };
            Interceptor.ExecuteWrappedCall(_target, args, 
                TargetMethodHelper.Operation1Method, TargetMethodHelper.IFoo_Operation1, _aspects);
        }

        public int Operation2(int a, int b)
        {
            var args = new List<MethodArgument>
                {
                    new MethodArgument("a", a),
                    new MethodArgument("b", b)
                };
            var result = Interceptor.ExecuteWrappedCall(_target, args, 
                TargetMethodHelper.Operation2Method, TargetMethodHelper.IFoo_Operation2, _aspects);
            return (int)result.ReturnValue;
        }

        public string Operation3(int a, out string b, ref int c)
        {
            var args = new List<MethodArgument>
                {
                    new MethodArgument("a", a),
                    new MethodArgument("c", c)
                };
            var result = Interceptor.ExecuteWrappedCall(_target, args, 
                TargetMethodHelper.Operation3Method, TargetMethodHelper.IFoo_Operation3, _aspects);
            b = (string)result.GetOutputArgument(0).Value;
            c = (int)result.GetOutputArgument(1).Value;
            return (string)result.ReturnValue;
        }

        public void Operation4(int a, ref string b, out int c)
        {
            var args = new List<MethodArgument>
                {
                    new MethodArgument("a", a),
                    new MethodArgument("b", b)
                };
            var result = Interceptor.ExecuteWrappedCall(_target, args, 
                TargetMethodHelper.Operation4Method, TargetMethodHelper.IFoo_Operation4, _aspects);
            b = (string)result.GetOutputArgument(0).Value;
            c = (int)result.GetOutputArgument(1).Value;
        }

        public void Operation5()
        {
            var args = new List<MethodArgument>();
            Interceptor.ExecuteWrappedCall(_target, args, 
                TargetMethodHelper.Operation5Method, TargetMethodHelper.IFoo_Operation5, _aspects);
        }

        public string Operation6()
        {
            var args = new List<MethodArgument>();
            var result = Interceptor.ExecuteWrappedCall(_target, args,
                TargetMethodHelper.Operation6Method, TargetMethodHelper.IFoo_Operation6, _aspects);
            return (string)result.ReturnValue;
        }

        public void Operation7(ref Guid a)
        {
            var args = new List<MethodArgument>
                {
                    new MethodArgument("a", a),
                };
            var result = Interceptor.ExecuteWrappedCall(_target, args,
                TargetMethodHelper.Operation4Method, TargetMethodHelper.IFoo_Operation4, _aspects);
            a = (Guid)result.GetOutputArgument(0).Value;
        }
    }

    public class Aspect1: IMethodAspect
    {
        public static bool OnEntryVisited { get; set; }
        public static bool OnExitVisited { get; set; }
        public static bool OnSuccessVisited { get; set; }
        public static bool OnExceptionVisited { get; set; }

        public static bool SkipBodyCall { get; set; }

        static Aspect1()
        {
            Reset();
        }

        public static void Reset()
        {
            OnEntryVisited = false;
            OnExitVisited = false;
            OnSuccessVisited = false;
            OnExceptionVisited = false;
            SkipBodyCall = false;
        }

        public IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnEntryVisited = true;
            return SkipBodyCall ? new MethodResultDescriptor(new InvalidOperationException()) : null;
        }

        public void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnExitVisited = true;
        }

        public IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnSuccessVisited = true;
            return result;
        }

        public Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised)
        {
            OnExceptionVisited = true;
            return exceptionRaised;
        }
    }

    public class Aspect2 : IMethodAspect
    {
        public static string OnEntryVisited { get; set; }
        public static string OnExitVisited { get; set; }
        public static string OnSuccessVisited { get; set; }
        public static string OnExceptionVisited { get; set; }

        public static bool SkipBodyCall { get; set; }

        static Aspect2()
        {
            Reset();
        }

        public static void Reset()
        {
            OnEntryVisited = "";
            OnExitVisited = "";
            OnSuccessVisited = "";
            OnExceptionVisited = "";
            SkipBodyCall = false;
        }

        public virtual IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnEntryVisited += "1";
            return SkipBodyCall ? new MethodResultDescriptor(new InvalidOperationException()) : null;
        }

        public virtual void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnExitVisited += "1";
        }

        public virtual IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnSuccessVisited += "1";
            return result;
        }

        public virtual Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised)
        {
            OnExceptionVisited += "1";
            return exceptionRaised;
        }
    }

    public class Aspect3 : Aspect2
    {
        public override IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnEntryVisited += "2";
            return SkipBodyCall ? new MethodResultDescriptor(new InvalidOperationException()) : null;
        }

        public override void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnExitVisited += "2";
        }

        public override IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnSuccessVisited += "2";
            return result;
        }

        public override Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised)
        {
            OnExceptionVisited += "2";
            return exceptionRaised;
        }
    }

    public class Aspect4 : IMethodAspect
    {
        public static bool OnEntryVisited { get; set; }
        public static bool OnExitVisited { get; set; }
        public static bool OnSuccessVisited { get; set; }
        public static bool OnExceptionVisited { get; set; }

        static Aspect4()
        {
            Reset();
        }

        public static void Reset()
        {
            OnEntryVisited = false;
            OnExitVisited = false;
            OnSuccessVisited = false;
            OnExceptionVisited = false;
        }

        public IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnEntryVisited = true;
            throw new InvalidOperationException();
        }

        public void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnExitVisited = true;
        }

        public IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnSuccessVisited = true;
            return result;
        }

        public Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised)
        {
            OnExceptionVisited = true;
            return exceptionRaised;
        }
    }

    public class Aspect5 : IMethodAspect
    {
        public static bool OnEntryVisited { get; set; }
        public static bool OnExitVisited { get; set; }
        public static bool OnSuccessVisited { get; set; }
        public static bool OnExceptionVisited { get; set; }

        static Aspect5()
        {
            Reset();
        }

        public static void Reset()
        {
            OnEntryVisited = false;
            OnExitVisited = false;
            OnSuccessVisited = false;
            OnExceptionVisited = false;
        }

        public IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnEntryVisited = true;
            return null;
        }

        public void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnExitVisited = true;
            throw new InvalidOperationException();
        }

        public IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnSuccessVisited = true;
            return result;
        }

        public Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised)
        {
            OnExceptionVisited = true;
            return exceptionRaised;
        }
    }

    public class Aspect6 : IMethodAspect
    {
        public static bool OnEntryVisited { get; set; }
        public static bool OnExitVisited { get; set; }
        public static bool OnSuccessVisited { get; set; }
        public static bool OnExceptionVisited { get; set; }

        static Aspect6()
        {
            Reset();
        }

        public static void Reset()
        {
            OnEntryVisited = false;
            OnExitVisited = false;
            OnSuccessVisited = false;
            OnExceptionVisited = false;
        }

        public IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnEntryVisited = true;
            return null;
        }

        public void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnExitVisited = true;
        }

        public IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnSuccessVisited = true;
            throw new InvalidOperationException();
        }

        public Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised)
        {
            OnExceptionVisited = true;
            return exceptionRaised;
        }
    }

    public class Aspect7 : IMethodAspect
    {
        public static bool OnEntryVisited { get; set; }
        public static bool OnExitVisited { get; set; }
        public static bool OnSuccessVisited { get; set; }
        public static bool OnExceptionVisited { get; set; }

        static Aspect7()
        {
            Reset();
        }

        public static void Reset()
        {
            OnEntryVisited = false;
            OnExitVisited = false;
            OnSuccessVisited = false;
            OnExceptionVisited = false;
        }

        public IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnEntryVisited = true;
            return null;
        }

        public void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnExitVisited = true;
        }

        public IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnSuccessVisited = true;
            return result;
        }

        public Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised)
        {
            OnExceptionVisited = true;
            throw new InvalidOperationException();
        }
    }

    public class FaultyAspect1: IMethodAspect
    {
        public IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            throw new InvalidOperationException();
        }

        public void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
        }

        public IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            return result;
        }

        public Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised)
        {
            return exceptionRaised;
        }
    }

    public class FaultyAspect2 : IMethodAspect
    {
        public IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            return result;
        }

        public void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            throw new InvalidOperationException();
        }

        public IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            return result;
        }

        public Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised)
        {
            return exceptionRaised;
        }
    }

    public class FaultyAspect3 : IMethodAspect
    {
        public IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            return result;
        }

        public void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
        }

        public IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            throw new InvalidOperationException();
        }

        public Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised)
        {
            return exceptionRaised;
        }
    }

    public class FaultyAspect4 : IMethodAspect
    {
        public IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            return result;
        }

        public void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
        }

        public IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            return result;
        }

        public Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised)
        {
            throw new InvalidOperationException();
        }
    }

    public class FaultyAspect5 : IMethodAspect
    {
        public IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            return result;
        }

        public void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
        }

        public IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            return result;
        }

        public Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised)
        {
            return null;
        }
    }

    public class BodySkipperAspect : IMethodAspect
    {
        public static bool OnEntryVisited { get; set; }
        public static bool OnExitVisited { get; set; }
        public static bool OnSuccessVisited { get; set; }
        public static bool OnExceptionVisited { get; set; }

        static BodySkipperAspect()
        {
            Reset();
        }

        public static void Reset()
        {
            OnEntryVisited = false;
            OnExitVisited = false;
            OnSuccessVisited = false;
            OnExceptionVisited = false;
        }

        public IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnEntryVisited = true;
            return new MethodResultDescriptor(true, 2, new List<MethodArgument>());
        }

        public void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnExitVisited = true;
        }

        public IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            OnSuccessVisited = true;
            throw new InvalidOperationException();
        }

        public Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised)
        {
            OnExceptionVisited = true;
            return exceptionRaised;
        }
    }

    public interface IWrongInterface1<out T>
    {
        T Get();
    }

    class WrongInterface1<T> : IWrongInterface1<T>
    {
        public T Get()
        {
            throw new NotImplementedException();
        }
    }

    public interface IWrongInterface2: IWrongInterface1<int>
    {
    }

    class WrongInterface2 : IWrongInterface2
    {
        public int Get()
        {
            throw new NotImplementedException();
        }
    }

    public interface IWrongInterface3
    {
        T Get<T>();
    }

    class WrongInterface3 : IWrongInterface3
    {
        public T Get<T>()
        {
            throw new NotImplementedException();
        }
    }

    public interface IWrongInterface4 : IWrongInterface3
    {
        
    }

    class WrongInterface4 : IWrongInterface4
    {
        public T Get<T>()
        {
            throw new NotImplementedException();
        }
    }
}
