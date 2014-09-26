using System;

namespace Seemplest.Core.WindowsEventLog
{
    /// <summary>
    /// This attribute can be added to <see cref="LogEventBase"/> derived classes
    /// in order to prevent the log source installation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SkipLogSourceInstallationAttribute: Attribute
    {
    }
}