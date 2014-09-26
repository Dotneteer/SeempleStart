namespace Seemplest.Core.Tasks
{
    /// <summary>
    /// This enumeration defines the type of frequencies used for a task.
    /// </summary>
    public enum TaskFrequencyType
    {
        /// <summary>Undefined</summary>
        None,
        /// <summary>Every month</summary>
        Month,
        /// <summary>Every week</summary>
        Week,
        /// <summary>Every day</summary>
        Day,
        /// <summary>Every hour</summary>
        Hour,
        /// <summary>Every minute</summary>
        Minute,
        /// <summary>Every second</summary>
        Second
    }
}