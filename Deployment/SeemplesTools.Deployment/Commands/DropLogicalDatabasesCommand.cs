using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SeemplesTools.Deployment.Common;
using SeemplesTools.Deployment.DatabaseUpgrade;
using SeemplesTools.Deployment.DatabaseUpgrade.Steps;
using SeemplesTools.Deployment.Infrastructure;

namespace SeemplesTools.Deployment.Commands
{
    /// <summary>
    /// Ez a parancs eldobja a logikai adatbázisokat
    /// </summary>
    [Command("droplogicaldatabases", "droplogicaldatabase", "deletelogicaldatabase", "deletelogicaldatabases",
        "dld", "dldb", "dlds", "dldbs",
        UsageCommand = @"
  drop-logical-databases       Drops logical databases.",
        UsageArguments = @"
Arguments for drop-logical-databases:
  -sql-instance instance       SQL server instance (required)
  -sql-database database       SQL database name (required)
  -databases    databases      Comma-separated list of logical database names
                               (required)
  -optional     no             The logical database is always deleted (default)
                yes            The logical database is only deleted if it
                               exists")]
    public class DropLogicalDatabasesCommand : DatabaseCommand
    {
        #region Properties

        /// <summary>
        /// A parancs neve
        /// </summary>
        public override string Name
        {
            get { return "Logical databases"; }
        }

        /// <summary>
        /// A parancs aktuális végrehajtási fázisához kapcsolódó szöveg
        /// </summary>
        public override string ProgressText
        {
            get { return "Deleting logical databases"; }
        }

        /// <summary>
        /// A parancs véglegesítési fázisához kapcsolódó szöveg
        /// </summary>
        public override string CommitProgressText
        {
            get { return "Committing deletion of logical databases"; }
        }

        /// <summary>
        /// A parancs visszagörgetési fázisához kapcsolódó szöveg
        /// </summary>
        public override string RollbackProgressText
        {
            get { return "Rolling back deletion of logical databases"; }
        }

        private string _sqlInstance;
        private string _sqlDatabase;
        private readonly List<string> _databases;
        private bool _optional;

        #endregion

        #region Initialization

        /// <summary>
        /// Létrehoz egy paraméterek nélküli parancsot
        /// </summary>
        public DropLogicalDatabasesCommand()
        {
            _databases = new List<string>();
        }

        /// <summary>
        /// Létrehoz egy parancsot a megadott paraméterekkel
        /// </summary>
        /// <param name="sqlInstance">Az SQL Server példány neve</param>
        /// <param name="sqlDatabase">Az adatbázis neve</param>
        /// <param name="databases">A logikai adatbázis sémák neve (vesszővel elválasztva)</param>
        /// <param name="optional">A logikai adatbázis csak akkor kerül törlésre, ha már létre van hozva.</param>
        public DropLogicalDatabasesCommand(
            string sqlInstance,
            string sqlDatabase,
            IEnumerable<string> databases,
            bool optional)
        {
            if (String.IsNullOrWhiteSpace(sqlInstance))
            {
                throw new ArgumentNullException("sqlInstance");
            }

            if (String.IsNullOrWhiteSpace(sqlDatabase))
            {
                throw new ArgumentNullException("sqlDatabase");
            }

            // ReSharper disable once PossibleMultipleEnumeration
            if (databases == null || !databases.Any())
            {
                throw new ArgumentNullException("databases");
            }

            _sqlInstance = sqlInstance.Trim();
            _sqlDatabase = sqlDatabase.Trim();

            _databases = new List<string>();

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var database in databases)
            {
                if (String.IsNullOrWhiteSpace(database))
                {
                    throw new ArgumentException("Database name cannot be null or empty.");
                }

                if (database.Trim().Length != database.Length)
                {
                    throw new ArgumentException("Database name must be trimmed.");
                }
                _databases.Add(database);
            }
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
                    argument = ParameterHelper.Mandatory(argument, original).Trim();
                    if (argument.Length == 0)
                    {
                        throw new ParameterException("SQL database name cannot be empty.");
                    }
                    _sqlDatabase = argument;
                    return true;
                
                case "databases":
                case "database":
                case "d":
                case "db":
                case "ds":
                case "dbs":
                case "logicaldatabases":
                case "logicaldatabase":
                case "ld":
                case "ldb":
                case "lds":
                case "ldbs":
                    argument = ParameterHelper.Mandatory(argument, original);

                    var databases = argument.Split(',');
                    _databases.Clear();

                    foreach (var database in databases)
                    {
                        if (String.IsNullOrWhiteSpace(database))
                        {
                            throw new ParameterException("Logical database name cannot be empty.");
                        }
                        _databases.Add(database);
                    }
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

            if (_databases.Count == 0)
            {
                throw new ParameterException(String.Format("No logical databases specified for command {0}.", 
                    Original));
            }
        }

        #endregion

        #region Execution

        /// <summary>
        /// A parancs végrehajtása
        /// </summary>
        protected override void DoRun()
        {
            SqlConnection connection;
            SqlTransaction transaction;
            GetConnection(_sqlInstance, _sqlDatabase, out connection, out transaction);

            foreach (var database in _databases)
            {
                var stepGroup = new StepGroup(database, null);
                var schemaNames = GetSchemas(connection, transaction, database);
                if (schemaNames == null)
                {
                    if (!_optional)
                    {
                        throw new ApplicationException(String.Format("Logical database {0} does not exist", database));
                    }
                    continue;
                }

                // ReSharper disable once UnusedVariable
                foreach (var schemaName in schemaNames)
                {
                    // ReSharper disable once ObjectCreationAsStatement
                    new DropSchemasStep(stepGroup, schemaNames);
                }

                // ReSharper disable once ObjectCreationAsStatement
                new SetMetadataStep(stepGroup);

                stepGroup.Execute(connection, transaction);
            }
        }

        // --- A sémákat lekérdező utasítások
        private const string GET_SCHEMAS_QUERY =
@"IF EXISTS
(
  SELECT 1
  FROM [sys].[objects] [o]
  INNER JOIN [sys].[schemas] [s]
    ON [o].[schema_id] = [s].[schema_id]
  WHERE
    [o].[name] = 'Schemas' AND
    [s].[name] = 'Metadata'
)
  SELECT [Name]
  FROM [Metadata].[Schemas]
  WHERE [LogicalDatabaseName] = @name
";

        /// <summary>
        /// Lekérdezi az adatbázisban található sémákat
        /// </summary>
        /// <param name="connection">Kapcsolati infó</param>
        /// <param name="transaction">Tranzakciópéldány</param>
        /// <param name="databaseName">Adatbázis neve</param>
        /// <returns>Az adatbázisban lévő sémák listája</returns>
        private static HashSet<string> GetSchemas(
            SqlConnection connection,
            SqlTransaction transaction,
            string databaseName)
        {
            using (var command = new SqlCommand(GET_SCHEMAS_QUERY, connection, transaction))
            {
                command.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar) { Value = databaseName });
                var dataSet = new DataSet();
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dataSet);
                }

                switch (dataSet.Tables.Count)
                {
                    case 0:
                        return null;
                    
                    case 1:
                        var schemaNameTable = dataSet.Tables[0];

                        var result = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
                        foreach (var schemaName in from DataRow schemaNameRow in schemaNameTable.Rows select schemaNameRow[0])
                        {
                            if (schemaName == null || schemaName == DBNull.Value || !(schemaName is string))
                            {
                                throw new ApplicationException(
                                    "The installed logical database has an invalid schema name.");
                            }
                            result.Add((string)schemaName);
                        }
                        return result;

                    default:
                        throw new ApplicationException(
                            "Schema name retrieval query returned unexpected number of tables.");
                }
            }
        }

        #endregion
    }
}
