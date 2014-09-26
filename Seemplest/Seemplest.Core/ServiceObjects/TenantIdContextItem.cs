namespace Seemplest.Core.ServiceObjects
{
    /// <summary>
    /// This class defines a context item that stores the tenant's ID
    /// </summary>
    public class TenantIdContextItem : ServiceCallContextItemBase<string>
    {
        /// <summary>
        /// Initializes the value of this context item
        /// </summary>
        /// <param name="itemValue">ID of the tenant</param>
        public TenantIdContextItem(string itemValue)
            : base(itemValue)
        {
        }
    }
}