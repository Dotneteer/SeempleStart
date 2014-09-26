namespace Seemplest.Core.Queue
{
    /// <summary>
    /// This interface is the abstraction of a generic message.
    /// </summary>
    public interface IMessage
    {
        /// <summary>String describing a message</summary>
        string MessageText { get; }
    }
}