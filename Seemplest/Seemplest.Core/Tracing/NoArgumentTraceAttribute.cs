using System;

namespace Seemplest.Core.Tracing
{
    /// <summary>
    /// This attribute signs that the related operation should not be logged.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class NoArgumentTraceAttribute : Attribute
    {
    }
}