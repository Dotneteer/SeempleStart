namespace Seemplest.Core.ServiceObjects
{
    /// <summary>
    /// This class defines a context item that stores the operation's instance ID
    /// </summary>
    public class OperationInstanceIdContextItem : ServiceCallContextItemBase<string>
    {
        /// <summary>
        /// Initializes the value of this context item
        /// </summary>
        /// <param name="itemValue">ID of the operation instance</param>
        public OperationInstanceIdContextItem(string itemValue)
            : base(itemValue)
        {
        }
    }
}