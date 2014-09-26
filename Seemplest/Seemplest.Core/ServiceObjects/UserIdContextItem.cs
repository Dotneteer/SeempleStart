using System.Diagnostics.CodeAnalysis;
namespace Seemplest.Core.ServiceObjects
{
    /// <summary>
    /// This class defines a context item that stores the current user's ID
    /// </summary>
    public class UserIdContextItem : ServiceCallContextItemBase<string>
    {
        /// <summary>
        /// Initializes the value of this context item
        /// </summary>
        /// <param name="itemValue">ID of the current user</param>
        [ExcludeFromCodeCoverage]
        public UserIdContextItem(string itemValue)
            : base(itemValue)
        {
        }
    }
}