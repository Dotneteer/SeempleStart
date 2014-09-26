namespace Seemplest.Core.Queue
{
    /// <summary>
    /// This interface represents a queue that can be addressed by a name.
    /// </summary>
    public interface INamedQueue : IQueue
    {
        /// <summary>
        /// Gets the name of the queue instance.
        /// </summary>
        string Name { get; }
    }
}