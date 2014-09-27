using System.Web;
using Seemplest.Core.DependencyInjection;
using Seemplest.Core.ServiceObjects;
using Younderwater.Services.Security;

namespace SeemplestBlocks.Core.ServiceInfrastructure
{
    /// <summary>
    /// This class creates service objects that may use the HTTP context to obtain 
    /// context information from header values, and also can use a an
    /// IUserIdContextProvider to obtain the current user's ID.
    /// </summary>
    public class BusinessContextServiceFactory : ContextedServiceFactoryBase
    {
        /// <summary>
        /// Initializes a new instance with the specified service locator
        /// </summary>
        /// <param name="serviceLocator">ServiceLocator objektum</param>
        public BusinessContextServiceFactory(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
        }

        /// <summary>
        /// Creates a call context for the service object
        /// </summary>
        /// <returns>Service call context instance</returns>
        protected override IServiceCallContext CreateServiceCallContext()
        {
            var context = new ServiceCallContext();
            if (HttpContext.Current != null)
            {
                var headers = HttpContext.Current.Request.Headers;
                var operationId = headers[CustomHttpHeaders.OPERATION_ID_HEADER];
                if (operationId != null)
                {
                    context.Set(new OperationInstanceIdContextItem(operationId));
                }
                var uiCulture = headers[CustomHttpHeaders.UI_CULTURE_HEADER];
                if (uiCulture != null)
                {
                    context.Set(new UiCultureContextItem(uiCulture));
                }
            }

            // --- Set up the user context
            var userIdContextProvider = ServiceManager.GetService<IUserIdContextProvider>();
            if (userIdContextProvider != null)
            {
                userIdContextProvider.SetUsetContext(context);
            }
            return context;
        }
    }
}