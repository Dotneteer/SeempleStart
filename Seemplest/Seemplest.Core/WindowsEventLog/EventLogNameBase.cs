using System;

namespace Seemplest.Core.WindowsEventLog
{
    /// <summary>
    /// This class is intended to be the base class of all classes defining a Windows event 
    /// log name. The derived types can be used as parameters of the <see cref="EventLogNameAttribute"/> 
    /// class.
    /// </summary>
    public abstract class EventLogNameBase
    {
        /// <summary>
        /// Gets the name of the log assigned to the class
        /// </summary>
        public string LogName { get; private set; }

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        protected EventLogNameBase()
        {
            System.Reflection.MemberInfo info = GetType();
            var attributes = info.GetCustomAttributes(typeof(EventLogNameAttribute), false)
                as EventLogNameAttribute[];
            // ReSharper disable PossibleNullReferenceException
            if (attributes.Length == 0)
            // ReSharper restore PossibleNullReferenceException
            {
                throw new InvalidOperationException("EventLogNameAttribute not set on this class");
            }
            var attribute = attributes[0];
            LogName = attribute.Value;
        }
    }
}