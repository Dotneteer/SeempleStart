using Seemplest.Core.DataAccess.DataServices;
using Seemplest.Core.DependencyInjection;
using SeemplestBlocks.Core.Security;
using SeemplestCloud.Services.SubscriptionService;
using SeemplestCloud.Services.SubscriptionService.DataAccess;
using SeemplestCloud.WebClient.Providers;

namespace SeemplestCloud.WebClient
{
    public static class ServiceConfig
    {
        private const string DB_CONN = "connStr=Sc";

        /// <summary>
        /// Registers services used by this website
        /// </summary>
        public static void RegisterServices()
        {
            // --- Prepare the service manager
            ServiceManager.SetRegistry(new DefaultServiceRegistry());

            // --- Register services
            ServiceManager.Register<ISubscriptionDataOperations, SubscriptionDataOperations>(DB_CONN);
            ServiceManager.Register<ISubscriptionService, SubscriptionService>();

            // --- Configure the user ID provider service
            ServiceManager.Register<IUserIdContextProvider, UserIdContextProvider>();

            // --- Let the data access factory use the service manager
            DataAccessFactory.SetRegistry(ServiceManager.ServiceRegistry);
        }
    }
}