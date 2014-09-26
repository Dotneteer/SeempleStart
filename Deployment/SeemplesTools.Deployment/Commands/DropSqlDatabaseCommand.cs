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
  -optional     no             The SQL database is always deleted (default)
                yes            The SQL database is only deleted if it exists")]
    public class DropSqlDatabaseCommand : LeafCommand
    {
        #region Properties

        /// <summary>
        /// A parancs neve
        /// </summary>
        public override string Name
        {
            get { return String.Format("SQL database {0} on instance {1}", _sqlDatabase, _sqlInstance); }
        }

        /// <summary>
        /// A parancs aktuális végrehajtási fázisához kapcsolódó szöveg
        /// </summary>
        public override string ProgressText
        {
            get { return String.Format("Deleting SQL database {0} on instance {1}", _sqlDatabase, _sqlInstance); }
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

        private string _sqlInstance;
        private string _sqlDatabase;
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
        /// <param name="optional">Az adatbázist csak akkor tötöljük, ha létezik</param>
        public DropSqlDatabaseCommand(string sqlInstance, string sqlDatabase, bool optional)
        {
            if (String.IsNullOrWhiteSpace(sqlInstance))
            {
                throw new ArgumentNullException("sqlInstance");
            }

            if (String.IsNullOrWhiteSpace(sqlDatabase))
            {
                throw new ArgumentNullException("sqlDatabase");
            }

            _sqlInstance = sqlInstance.Trim();
            _sqlDatabase = sqlDatabase.Trim();
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
            switch (option)
            {
                case "sqlinstance":
                case "si":
                case "instance":
                case "i":
                    argument = ParameterHelper.Mandatory(argument, original).Trim();
                    if (argument.Length == 0)
                    {
                        throw new ParameterException("SQL instance name cannot be empty.");
                    }
                    _sqlInstance = argument;
                    return true;
                
                case "sqldatabase":
                case "sd":
                case "sdb":
                case "database":
                case "d":
                case "db":
                    argument = ParameterHelper.Mandatory(argument, original).Trim();
                    if (argument.Length == 0)
                    {
                        throw new ParameterException("SQL database name cannot be empty.");
                    }
                    _sqlDatabase = argument;
                    return true;
                
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
            CheckMandatoryParameter(_sqlInstance, "sql-instance", Original);
            CheckMandatoryParameter(_sqlDatabase, "sql-database", Original);
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
            var connectionString = DatabaseHelper.GetConnectionString(_sqlInstance, _sqlDatabase);
            DeploymentTransaction.Current.Log("Trying to connect to existing database {0} on instance {1}.",
                _sqlDatabase, _sqlInstance);

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
                        "SQL database {0} does not exist on SQL instance {1}.", _sqlDatabase, _sqlInstance));
                }
                return;
            }

            var masterConnectionString = DatabaseHelper.GetConnectionString(_sqlInstance, "master");
            DeploymentTransaction.Current.Log("Connecting to master database on instance {0}.", _sqlInstance);
            using (connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(DROP_DATABASE_QUERY, connection))
                {
                    command.Parameters.Add("@name", SqlDbType.NVarChar, 128);
                    command.Parameters["@name"].Value = _sqlDatabase;
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
