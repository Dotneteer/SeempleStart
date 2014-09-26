namespace Seemplest.Core.ServiceObjects
{
    /// <summary>
    /// This class defines the user's UI culture context
    /// </summary>
    public class UiCultureContextItem : ServiceCallContextItemBase<string>
    {
        /// <summary>
        /// Initializes the value of this context item
        /// </summary>
        /// <param name="itemValue">ID of the operation instance</param>
        public UiCultureContextItem(string itemValue)
            : base(itemValue)
        {
        }
    }
}