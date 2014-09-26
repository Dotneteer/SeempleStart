namespace Seemplest.Core.ServiceObjects
{
    /// <summary>
    /// This class defines a context item that stores the business transaction ID
    /// </summary>
    public class BusinessTransactionIdContextItem : ServiceCallContextItemBase<string>
    {
        /// <summary>
        /// Initializes the value of this context item
        /// </summary>
        /// <param name="itemValue">ID of the current business transaction</param>
        public BusinessTransactionIdContextItem(string itemValue)
            : base(itemValue)
        {
        }
    }
}