using Seemplest.Core.Interception;

namespace SeemplestBlocks.Core.Diagnostics
{
    /// <summary>
    /// This class defines an aspect with the common diagnostics arttributes
    /// </summary>
    [PerformanceCounter]
    [Trace]
    public class CommonDiagnosticsAspectsAttribute : CompositeAspectAttributeBase
    {
    }
}