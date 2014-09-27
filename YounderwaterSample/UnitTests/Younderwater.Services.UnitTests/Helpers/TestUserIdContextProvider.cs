using Seemplest.Core.ServiceObjects;
using Younderwater.Services.Security;

namespace Younderwater.Services.UnitTests.Helpers
{
    /// <summary>
    /// This provider adds a test user to the service context
    /// </summary>
    public class TestUserIdContextProvider: IUserIdContextProvider
    {
        /// <summary>
        /// Sets up the user context
        /// </summary>
        public void SetUsetContext(IServiceCallContext context)
        {
            context.Set(new UserIdContextItem("test"));
        }
    }
}