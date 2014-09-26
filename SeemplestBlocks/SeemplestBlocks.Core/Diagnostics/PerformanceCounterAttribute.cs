using System;
using System.Reflection;
using Seemplest.Core.Interception;
using Seemplest.Core.PerformanceCounters;

namespace SeemplestBlocks.Core.Diagnostics
{
    /// <summary>
    /// This attribute defines an aspect that handles performance counters related to service operations
    /// </summary>
    public class PerformanceCounterAttribute : AspectAttributeBase
    {
        private const string TOTAL = "_Total";

        /// <summary>
        /// This method is called before the body of the aspected method is about to be
        /// invoked.
        /// </summary>
        /// <param name="args">Descriptor representing the method call</param>
        /// <param name="result">Result descriptor coming from the previous aspect.</param>
        /// <returns>
        /// This method should return null value indicating that the aspected method's body should
        /// be called. If the method body invocation should be omitted, this method returns the
        /// result descriptor substituting the result coming from the invocation of the method body.
        /// </returns>
        public override IMethodResultDescriptor OnEntry(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            // --- Increment Total counters
            PmcManager.GetCounter<NumberOfCallsPmc>(TOTAL).Increment();
            PmcManager.GetCounter<NumberOfCallsPerSecondsPmc>(TOTAL).Increment();
            PmcManager.GetCounter<CurrentCallsPmc>(TOTAL).Increment();

            // --- Increment operation counters
            var instanceName = GetInstanceName(args.Method);
            PmcManager.GetCounter<NumberOfCallsPmc>(instanceName).Increment();
            PmcManager.GetCounter<NumberOfCallsPerSecondsPmc>(instanceName).Increment();
            PmcManager.GetCounter<CurrentCallsPmc>(instanceName).Increment();

            return base.OnEntry(args, result);
        }

        /// <summary>
        /// This method is called right after the body of the aspected method has been
        /// executed, independently whether it was successful or failed.
        /// </summary>
        /// <param name="args">Descriptor representing the method call</param>
        /// <param name="result">Result descriptor representing the return values of the call</param>
        public override void OnExit(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            // --- Decrement current call counters
            PmcManager.GetCounter<CurrentCallsPmc>(TOTAL).Decrement();
            var instanceName = GetInstanceName(args.Method);
            PmcManager.GetCounter<CurrentCallsPmc>(instanceName).Decrement();
        }

        /// <summary>
        /// This method is called right after <see cref="IMethodAspect.OnExit"/>, when the method body
        /// invocation raised an exception. Otherwise, the <see cref="IMethodAspect.OnSuccess"/> method is
        /// called.
        /// </summary>
        /// <param name="argsMessage">Message representing the method call</param>
        /// <param name="exceptionRaised">Exception raised by the method body</param>
        /// <returns>Exception instance to be raised by the caller of the aspected method</returns>
        public override Exception OnException(IMethodCallDescriptor argsMessage, Exception exceptionRaised)
        {
            // --- Increment Total counters
            PmcManager.GetCounter<NumberOfFailedCallsPmc>(TOTAL).Increment();
            PmcManager.GetCounter<NumberOfFailedCallsPerSecondsPmc>(TOTAL).Increment();

            // --- Increment operation counters
            var instanceName = GetInstanceName(argsMessage.Method);
            PmcManager.GetCounter<NumberOfFailedCallsPmc>(instanceName).Increment();
            PmcManager.GetCounter<NumberOfFailedCallsPerSecondsPmc>(instanceName).Increment();

            return base.OnException(argsMessage, exceptionRaised);
        }

        /// <summary>
        /// Gets the method instance name
        /// </summary>
        /// <param name="method">Method call information</param>
        /// <returns>Method instance name</returns>
        public static string GetInstanceName(MethodInfo method)
        {
            return String.Format("{0}_{1}",
                method.DeclaringType == null ? "" : method.DeclaringType.Name,
                method.Name);
        }
    }
}