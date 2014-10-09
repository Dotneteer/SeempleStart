using System.Web;
using Microsoft.AspNet.Identity;
using Seemplest.Core.ServiceObjects;
using SeemplestBlocks.Core.Security;

namespace Younderwater.Webclient.Providers
{
    /// <summary>
    /// This provides obtains the current user's ID and pushes it to the service context.
    /// </summary>
    public class UserIdContextProvider : IUserIdContextProvider
    {
        /// <summary>
        /// Sets up the user context
        /// </summary>
        public void SetUsetContext(IServiceCallContext context)
        {
            context.Set(new UserIdContextItem(HttpContext.Current.User.Identity.GetUserId()));
        }
    }
}