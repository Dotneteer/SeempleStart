using System;
using System.Globalization;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Newtonsoft.Json;
using Seemplest.Core.Configuration;
using Seemplest.Core.Interception;
using Seemplest.Core.ServiceObjects;
using Seemplest.Core.Tracing;

namespace SeemplestBlocks.Core.Diagnostics
{
    /// <summary>
    /// This class provides an aspect that traces service method calls
    /// </summary>
    public class TraceAttribute : AspectAttributeBase
    {
        private const string START_TICK_LABEL = "$$$StartTick$$$";

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
            if (args.Method.DeclaringType == typeof(IServiceObject)) return null;

            CallContext.LogicalSetData(START_TICK_LABEL, EnvironmentInfo.GetCurrentDateTimeUtc().Ticks);
            var details = new StringBuilder();
            var traceAttrs = args.Method.GetCustomAttributes(typeof(NoArgumentTraceAttribute), true);
            if (traceAttrs.Length == 0)
            {
                for (var i = 0; i < args.ArgumentCount; i++)
                {
                    var arg = args.GetArgument(i);
                    var argJson = JsonConvert.SerializeObject(arg.Value);
                    details.AppendFormat("{0}: {1}\r\n", arg.Name, argJson);
                }
            }

            var logItem = new TraceLogItem
            {
                Type = TraceLogItemType.Informational,
                OperationType = GetOperationName(args),
                Message = "Enter",
                DetailedMessage = details.ToString()
            };
            Tracer.Log(logItem);
            return null;
        }

        /// <summary>
        /// This method is called right after <see cref="IMethodAspect.OnExit"/>, when the method body
        /// invocation was successful. Otherwise, the <see cref="IMethodAspect.OnException"/> method is
        /// called.
        /// </summary>
        /// <param name="args">Descriptor representing the method call</param>
        /// <param name="result">Result descriptor representing the return values of the call</param>
        /// <returns>
        /// This method should return the value of <paramref name="result"/> by default, or it
        /// can modify the original result on return that value.
        /// </returns>
        public override IMethodResultDescriptor OnSuccess(IMethodCallDescriptor args, IMethodResultDescriptor result)
        {
            if (args.Method.DeclaringType == typeof(IServiceObject)) return null;

            var startTimeData = CallContext.GetData(START_TICK_LABEL);
            var timeSpan = startTimeData == null
                ? (TimeSpan?)null
                : TimeSpan.FromTicks(EnvironmentInfo.GetCurrentDateTimeUtc().Ticks - (long)startTimeData);

            var logItem = new TraceLogItem
            {
                Type = TraceLogItemType.Success,
                OperationType = GetOperationName(args),
                Message = "Exit",
                DetailedMessage = string.Format("({0} ms)",
                    timeSpan.HasValue ? timeSpan.Value.Milliseconds.ToString(CultureInfo.InvariantCulture) : "??")
            };
            Tracer.Log(logItem);
            return result;
        }

        /// <summary>
        /// This method is called right after <see cref="IMethodAspect.OnExit"/>, when the method body
        /// invocation raised an exception. Otherwise, the <see cref="IMethodAspect.OnSuccess"/> method is
        /// called.
        /// </summary>
        /// <param name="args">Message representing the method call</param>
        /// <param name="exceptionRaised">Exception raised by the method body</param>
        /// <returns>Exception instance to be raised by the caller of the aspected method</returns>
        public override Exception OnException(IMethodCallDescriptor args, Exception exceptionRaised)
        {
            if (args.Method.DeclaringType == typeof(IServiceObject)) return null;

            var logItem = new TraceLogItem
            {
                Type = TraceLogItemType.Error,
                OperationType = GetOperationName(args),
                Message = "Exception",
                DetailedMessage = exceptionRaised.ToString()
            };
            Tracer.Log(logItem);
            return exceptionRaised;
        }

        /// <summary>
        /// Lekérdezi a művelet naplóban megjelenő nevét
        /// </summary>
        private static string GetOperationName(IMethodCallDescriptor args)
        {
            // ReSharper disable once PossibleNullReferenceException
            return string.Format("{0}.{1}",
                args.Method.DeclaringType.Name,
                args.Method.Name);
        }
    }
}