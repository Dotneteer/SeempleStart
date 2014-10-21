using System.Collections.Generic;
using SeemplesTools.Deployment.Commands;
using SeemplesTools.Deployment.Common;
using SeemplesTools.Deployment.Infrastructure;

namespace Setup.Commands
{
    /// <summary>
    /// This command deploys the SeemplestCloud database
    /// </summary>
    [Command("deployscdatabase", "dscdb",
        UsageCommand = @"
  deploy-sc-database          Deploys the SeemplestCloud database.",
        UsageArguments = @"
Arguments for deploy-sc-database:
  -sql-instance instance       SQL instance (required)
  -sql-database database       SQL database (required)
  -paths        directory      Directories containing SQL scripts (default:
                               Databases directory under current directory)
  -service-user user           Comma-separated list of users to which
                               permission is granted for the newly created or
                               upgraded logical databases (optional)
  -test-data    boolean        Insert test data (default: false)")]
    public class DeployScDatabaseCommand : DeployDatabaseCommandBase
    {
        /// <summary>
        /// Override this property to define the name of the database
        /// </summary>
        protected override string DatabaseName
        {
            get { return "SeemplestCloud"; }
        }

        /// <summary>
        /// Override this property to defines the list of logical databases to be
        /// deployed with the derived command
        /// </summary>
        protected override List<DatabaseNameVersionPair> LogicalDatabases
        {
            get
            {
                return new List<DatabaseNameVersionPair>
                {
                    new DatabaseNameVersionPair(DatabaseName, new DatabaseVersion(DbVersionInfo.VERSION_MAJOR, DbVersionInfo.VERSION_MINOR))
                };
            }
        }
    }
}
