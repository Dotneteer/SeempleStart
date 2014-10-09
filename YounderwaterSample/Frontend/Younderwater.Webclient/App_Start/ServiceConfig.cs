using Seemplest.Core.DataAccess.DataServices;
using Seemplest.Core.DependencyInjection;
using SeemplestBlocks.Core.Security;
using SeemplestBlocks.Core.Security.DataAccess;
using Younderwater.Services.DiveLog;
using Younderwater.Services.DiveLog.DataAccess;
using Younderwater.Webclient.Providers;

namespace Younderwater.Webclient
{
    public static class ServiceConfig
    {
        private const string DB_CONN = "connStr=Yw";

        /// <summary>
        /// Az adatbázis eléréséhez használt kapcsolati információ
        /// </summary>
        public static void RegisterServices()
        {
            // --- Prepare the service manager
            ServiceManager.SetRegistry(new DefaultServiceRegistry());

            // --- Register services
            ServiceManager.Register<ISecurityDataOperations, SecurityDataOperations>(DB_CONN);
            ServiceManager.Register<ISecurityService, SecurityService>();

            ServiceManager.Register<IDiveLogDataAccessOperations, DiveLogDataAccessOperations>(DB_CONN);
            ServiceManager.Register<IDiveLogService, DiveLogService>();

            // --- Configure the user ID provider service
            ServiceManager.Register<IUserIdContextProvider, UserIdContextProvider>();

            // --- Let the data access factory use the service manager
            DataAccessFactory.SetRegistry(ServiceManager.ServiceRegistry);
        }
    }
}