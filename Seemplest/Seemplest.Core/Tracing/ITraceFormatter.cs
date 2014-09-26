namespace Seemplest.Core.Tracing
{
    /// <summary>
    /// This interface defines the behavior of a trace formatter
    /// </summary>
    public interface ITraceFormatter
    {
        /// <summary>
        /// Formats the specified trace log item into a string
        /// </summary>
        /// <param name="item">Trace log item</param>
        /// <returns></returns>
        string Format(TraceLogItem item);
    }
}