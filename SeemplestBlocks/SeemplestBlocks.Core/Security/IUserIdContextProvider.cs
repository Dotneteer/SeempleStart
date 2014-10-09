using Seemplest.Core.ServiceObjects;

namespace SeemplestBlocks.Core.Security
{
    /// <summary>
    /// This interface defines the operations of a user ID context provider
    /// </summary>
    public interface IUserIdContextProvider
    {
        /// <summary>
        /// Sets up the user context
        /// </summary>
        void SetUsetContext(IServiceCallContext context);
    }
}