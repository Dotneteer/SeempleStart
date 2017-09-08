using Seemplest.Core.DataAccess.Mapping;
using Seemplest.Core.Interception;
using Seemplest.Core.ServiceObjects;
using Seemplest.Core.Tracing;

namespace SeemplestBlocks.Core.Diagnostics
{
    public class DataReaderMappingTraceAttribute: AspectAttributeBase
    {
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

            var logItem = new TraceLogItem
            {
                Type = TraceLogItemType.Informational,
                OperationType = "DataReaderMapping",
                Message = "Current counters",
                DetailedMessage = string.Format("Mappers: {0}, Converters: {1}",
                    DataReaderMappingManager.GetMapperCount(), 
                    DataReaderMappingManager.GetConverterCount())
            };
            Tracer.Log(logItem);
            return null;
        }

    }
}