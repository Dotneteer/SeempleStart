using System;
using System.Data;
using System.Data.SqlClient;
using SeemplesTools.Deployment.Common;
using SeemplesTools.Deployment.Infrastructure;

namespace SeemplesTools.Deployment.Commands
{
    /// <summary>
    /// Ez a parancs egy meglévő SQL adatbázist töröl
    /// </summary>
    [Command("dropsqldatabase", "dsd", "dsqld", "dsdb", "dsqldb",
        UsageCommand = @"
  drop-sql-database            Deletes an SQL database.",
        UsageArguments = @"
Arguments for drop-sql-database:
  -sql-instance instance       SQL instance (required)
  -sql-database database       SQL database (required)
  -user                        SQL user name (optional)
  -password                    SQL user password (optional)
  -optional     no             The SQL database is always deleted (default)
                yes            The SQL database is only deleted if it exists")]
    public class DropSqlDatabaseCommand : DatabaseCommand
    {
        #region Properties

        /// <summary>
        /// A parancs neve
        /// </summary>
        public override string Name
        {
            get { return String.Format("SQL database {0} on instance {1}", SqlDatabase, SqlInstance); }
        }

        /// <summary>
        /// A parancs aktuális végrehajtási fázisához kapcsolódó szöveg
        /// </summary>
        public override string ProgressText
        {
            get { return String.Format("Deleting SQL database {0} on instance {1}", SqlDatabase, SqlInstance); }
        }

        /// <summary>
        /// A parancs véglegesítési fázisához kapcsolódó szöveg
        /// </summary>
        public override string CommitProgressText
        {
            get { return null; }
        }

        /// <summary>
        /// A parancs visszagörgetési fázisához kapcsolódó szöveg
        /// </summary>
        public override string RollbackProgressText
        {
            get { return null; }
        }

        //private string _sqlInstance;
        //private string _sqlDatabase;
        private bool _optional;

        #endregion

        #region Initialization

        /// <summary>
        /// Létrehoz egy paraméterek nélküli parancsot
        /// </summary>
        public DropSqlDatabaseCommand()
        {
        }

        /// <summary>
        /// Létrehoz egy parancsot a megadott paraméterkkel
        /// </summary>
        /// <param name="sqlInstance">Az SQL Server példány neve</param>
        /// <param name="sqlDatabase">Az adatbázis neve</param>
        /// <param name="userName">Az SQL felhasználó neve</param>
        /// <param name="password">Az SQL felhasználó jelszava</param>
        /// <param name="optional">Az adatbázist csak akkor tötöljük, ha létezik</param>
        public DropSqlDatabaseCommand(string sqlInstance, string sqlDatabase, string userName, string password, bool optional)
        {
            if (String.IsNullOrWhiteSpace(sqlInstance))
            {
                throw new ArgumentNullException("sqlInstance");
            }

            if (String.IsNullOrWhiteSpace(sqlDatabase))
            {
                throw new ArgumentNullException("sqlDatabase");
            }

            SqlInstance = sqlInstance.Trim();
            SqlDatabase = sqlDatabase.Trim();
            UserName = userName == null ? null : userName.Trim();
            Password = password == null ? null : password.Trim();
            _optional = optional;
        }

        /// <summary>
        /// Értelmezi a parancs argumentumait
        /// </summary>
        /// <param name="option">Az opció neve</param>
        /// <param name="argument">Az argumentum értéke</param>
        /// <param name="original">A parancs szövege</param>
        /// <returns>Sikerült az értelmezés?</returns>
        protected override bool DoParseOption(string option, string argument, string original)
        {
            if (base.DoParseOption(option, argument, original))
            {
                return true;
            }
            switch (option)
            {
                case "optional":
                case "o":
                    argument = ParameterHelper.Mandatory(argument, original).Trim();
                    switch (argument.ToLower())
                    {
                        case "0":
                        case "no":
                        case "false":
                            _optional = false;
                            break;
                        case "1":
                        case "yes":
                        case "true":
                            _optional = true;
                            break;
                        default:
                            throw new ParameterException(String.Format(
                                "Option {0} requires a boolean argument.", original));
                    }
                    return true;
                
                default:
                    throw new ParameterException(String.Format(
                        "Unknown option {0} for command {1}.", original, Original));
            }
        }

        /// <summary>
        /// Befejezi a parancs inicializálását
        /// </summary>
        protected override void DoFinishInitialization()
        {
            CheckMandatoryParameter(SqlInstance, "sql-instance", Original);
            CheckMandatoryParameter(SqlDatabase, "sql-database", Original);
        }

        #endregion

        #region Execution

        // --- Az adatbázist törlő utasítás
        private const string DROP_DATABASE_QUERY =
@"DECLARE @dbName [nvarchar](258)
SET @dbName = quotename(@name)
DECLARE @query [nvarchar](max)
SET @query = 'ALTER DATABASE ' + @dbName + ' SET SINGLE_USER WITH ROLLBACK IMMEDIATE'
EXEC sp_executesql @query
SET @query = 'DROP DATABASE ' + @dbName
EXEC sp_executesql @query
";

        /// <summary>
        /// A parancs végrehajtása
        /// </summary>
        protected override void DoRun()
        {
            var connectionString = DatabaseHelper.GetConnectionString(SqlInstance, SqlDatabase, UserName, Password);
            DeploymentTransaction.Current.Log("Trying to connect to existing database {0} on instance {1}.",
                SqlDatabase, SqlInstance);

            SqlConnection connection;
            try
            {
                using (connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                }
                DeploymentTransaction.Current.Log("Database exists.");
            }
            catch (SqlException e)
            {
                if (e.Number != 4060) throw;

                DeploymentTransaction.Current.Log("Database does not exist.");
                if (!_optional)
                {
                    throw new ApplicationException(String.Format(
                        "SQL database {0} does not exist on SQL instance {1}.", SqlDatabase, SqlInstance));
                }
                return;
            }

            var masterConnectionString = DatabaseHelper.GetConnectionString(SqlInstance, "master", UserName, Password);
            DeploymentTransaction.Current.Log("Connecting to master database on instance {0}.", SqlInstance);
            using (connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(DROP_DATABASE_QUERY, connection))
                {
                    command.Parameters.Add("@name", SqlDbType.NVarChar, 128);
                    command.Parameters["@name"].Value = SqlDatabase;
                    DeploymentTransaction.Current.LogSqlCommand(command);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// A parancs véglegesítése
        /// </summary>
        protected override void DoCommit()
        {
        }

        /// <summary>
        /// A parancs visszagörgetése
        /// </summary>
        protected override void DoRollback()
        {
        }

        #endregion
    }
}
