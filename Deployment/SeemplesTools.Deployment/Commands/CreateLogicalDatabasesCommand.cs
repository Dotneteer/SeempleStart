using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using SeemplesTools.Deployment.Common;
using SeemplesTools.Deployment.DatabaseUpgrade;
using SeemplesTools.Deployment.Infrastructure;

namespace SeemplesTools.Deployment.Commands
{
    /// <summary>
    /// Ez a parancs egy logikai adatbázist hoz létre
    /// </summary>
    [Command("createlogicaldatabases", "createlogicaldatabase", "cld", "cldb", "clds", "cldbs",
        UsageCommand = @"
  create-logical-databases     Creates or upgrades logical databases.",
        UsageArguments = @"
Arguments for create-logical-databases:
  -sql-instance instance       SQL server instance (required)
  -sql-database database       SQL database name (required)
  -databases    databases      Comma-separated list of logical database names
                               or database-version pairs (required)
  -paths        directory      Directories containing SQL scripts (default:
                               Databases directory under current directory)
  -service-user user           Comma-separated list of users to which
                               permission is granted for the newly created or
                               upgraded logical databases (optional)
  -test-data    boolean        Insert test data (default: false)")]
    public class CreateLogicalDatabasesCommand : DatabaseCommand
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
            get { return "Creating logical databases"; }
        }

        /// <summary>
        /// A parancs véglegesítési fázisához kapcsolódó szöveg
        /// </summary>
        public override string CommitProgressText
        {
            get { return "Committing creation of logical databases"; }
        }

        /// <summary>
        /// A parancs visszagörgetési fázisához kapcsolódó szöveg
        /// </summary>
        public override string RollbackProgressText
        {
            get { return "Rolling back creation of logical databases"; }
        }

        private string _sqlInstance;
        private string _sqlDatabase;
        private readonly List<DatabaseNameVersionPair> _databases;
        private readonly List<string> _paths;
        private readonly List<string> _serviceUsers;
        private bool _insertTestData;

        #endregion

        #region Initialization

        /// <summary>
        /// Inicializálja a parancsot
        /// </summary>
        public CreateLogicalDatabasesCommand()
        {
            _databases = new List<DatabaseNameVersionPair>();
            _serviceUsers = new List<string>();
        }

        /// <summary>
        /// A megadott paraméterekkel inicializálja a parancsot.
        /// </summary>
        /// <param name="sqlInstance">Az SQL Server neve</param>
        /// <param name="sqlDatabase">Az adatbázis neve</param>
        /// <param name="databases">Az adatbázisok listája</param>
        /// <param name="paths">A szkriptekhez tartozó útvonalak</param>
        /// <param name="serviceUsers">A létrehozáshoz tartozó felhasználók</param>
        /// <param name="insertTestData">Teszteléshez használt adatokak be kell szúrni?</param>
        public CreateLogicalDatabasesCommand(
            string sqlInstance,
            string sqlDatabase,
            IEnumerable<DatabaseNameVersionPair> databases,
            IEnumerable<string> paths,
            IEnumerable<string> serviceUsers,
            bool insertTestData)
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

            _databases = new List<DatabaseNameVersionPair>();

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var database in databases)
            {
                if (String.IsNullOrWhiteSpace(database.Name))
                {
                    throw new ArgumentException("Database name cannot be null or empty.");
                }

                if (database.Name.Trim().Length != database.Name.Length)
                {
                    throw new ArgumentException("Database name must be trimmed.");
                }

                if (database.Name.IndexOfAny(new[] {'?', '*'}) != -1)
                {
                    throw new ArgumentException("Database name may not contain path wildcards.");
                }

                if (database.Version == null)
                {
                    throw new ArgumentException("Database version cannot be null.");
                }

                _databases.Add(database);
            }

            _paths = new List<string>();

            if (paths != null)
            {
                foreach (var path in paths)
                {
                    if (String.IsNullOrEmpty(path))
                    {
                        throw new ArgumentException("Database script path cannot be null or empty");
                    }
                    _paths.Add(Path.IsPathRooted(path) 
                        ? path 
                        : Path.Combine(Environment.CurrentDirectory, path));
                }
            }

            _serviceUsers = new List<string>();

            if (serviceUsers != null)
            {
                foreach (var serviceUser in serviceUsers)
                {
                    if (String.IsNullOrWhiteSpace(serviceUser))
                    {
                        throw new ArgumentException("Service user name cannot be null or empty.");
                    }
                    _serviceUsers.Add(serviceUser.Trim());
                }
            }

            _insertTestData = insertTestData;
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
                        string name;
                        string version;

                        var index = database.IndexOf('-');
                        if (index == -1)
                        {
                            name = database.Trim();
                            version = "";
                        }
                        else
                        {
                            name = database.Substring(0, index).Trim();
                            version = database.Substring(index + 1).Trim();
                        }

                        if (String.IsNullOrWhiteSpace(name))
                            throw new ParameterException("Logical database name cannot be empty.");

                        if (name.IndexOfAny(new[] { '?', '*' }) != -1)
                            throw new ArgumentException("Database name may not contain path wildcards.");

                        DatabaseVersion parsedVersion;
                        if (String.IsNullOrWhiteSpace(version))
                            parsedVersion = VersionHelper.DatabaseVersion;
                        else if (!DatabaseVersion.TryParse(version, out parsedVersion))
                            throw new ParameterException("Invalid version number specified for logical database");

                        _databases.Add(new DatabaseNameVersionPair(name, parsedVersion));
                    }
                    return true;
                
                case "paths":
                case "path":
                case "p":
                case "ps":
                case "scriptpaths":
                case "sqlpaths":
                case "scriptpath":
                case "sqlpath":
                case "sp":
                case "sps":
                case "sqlp":
                case "sqlps":
                    argument = ParameterHelper.Mandatory(argument, original);

                    var paths = argument.Split(',');
                    _paths.Clear();

                    if (paths.Length == 1 && String.IsNullOrEmpty(paths[0]))
                        return true;

                    foreach (var path in paths)
                    {
                        bool isAbsolute;

                        try
                        {
                            isAbsolute = Path.IsPathRooted(path);
                        }
                        catch (ArgumentException)
                        {
                            throw new ParameterException("Database script path must be a valid path.");
                        }

                        _paths.Add(isAbsolute 
                            ? path 
                            : Path.Combine(Environment.CurrentDirectory, path));
                    }
                    return true;

                case "serviceuser":
                case "serviceusers":
                case "su":
                case "sus":
                case "user":
                case "users":
                case "u":
                case "us":
                    argument = ParameterHelper.Mandatory(argument, original);

                    var users = argument.Split(',');
                    _serviceUsers.Clear();

                    foreach (var user in users)
                    {
                        if (String.IsNullOrWhiteSpace(user))
                        {
                            throw new ParameterException("Service user name cannot be empty.");
                        }
                        _serviceUsers.Add(user.Trim());
                    }
                    return true;

                case "testdata":
                case "t":
                case "td":
                case "test":
                case "inserttestdata":
                case "it":
                case "itd":
                    argument = ParameterHelper.Mandatory(argument, original).Trim();
                    switch (argument.ToLower())
                    {
                        case "0":
                        case "no":
                        case "false":
                            _insertTestData = false;
                            break;
                        case "1":
                        case "yes":
                        case "true":
                            _insertTestData = true;
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
                throw new ParameterException(String.Format("No logical databases specified for command {0}.", Original));
            }

            if (_paths.Count == 0)
            {
                _paths.Add(Path.Combine(Environment.CurrentDirectory, "Databases"));
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

            CreateMetadataSchema(connection, transaction);

            var dependencyGraph = new DependencyGraph();

            foreach (var database in _databases)
            {
                DatabaseVersion currentVersion;
                HashSet<string> existingSchemaNames;

                GetMetadata(connection, transaction, database.Name, out currentVersion, out existingSchemaNames);

                dependencyGraph.BuildUpgradePath(
                    database.Name,
                    currentVersion,
                    database.Version,
                    existingSchemaNames,
                    _paths,
                    _serviceUsers,
                    _insertTestData);
            }

            var orderedStepGroups = dependencyGraph.GetTopologicalOrder();

            foreach (var stepGroup in orderedStepGroups)
            {
                stepGroup.Execute(connection, transaction);
            }
        }

        // --- A metaadat sémát létrehozó utasítások
        private const string CREATE_METADATA_SCHEMA_QUERY =
@"IF NOT EXISTS
(
  SELECT 1
  FROM [sys].[schemas] [s]
  WHERE [s].[name] = 'Metadata'
)
  EXEC sp_executesql N'CREATE SCHEMA [Metadata]'

IF NOT EXISTS
(
  SELECT 1
  FROM [sys].[objects] [o]
  INNER JOIN [sys].[schemas] [s]
    ON [o].[schema_id] = [s].[schema_id]
  WHERE
    [o].[name] = 'LogicalDatabases' AND
    [s].[name] = 'Metadata'
)
  CREATE TABLE [Metadata].[LogicalDatabases]
  (
    [Name] [nvarchar](128) COLLATE Latin1_General_CI_AS PRIMARY KEY NOT NULL,
    [Version] [nvarchar](128) COLLATE Latin1_General_CI_AS NOT NULL
  )

IF NOT EXISTS
(
  SELECT 1
  FROM [sys].[objects] [o]
  INNER JOIN [sys].[schemas] [s]
    ON [o].[schema_id] = [s].[schema_id]
  WHERE
    [o].[name] = 'Schemas' AND
    [s].[name] = 'Metadata'
)
  CREATE TABLE [Metadata].[Schemas]
  (
    [Name] [nvarchar](128) COLLATE Latin1_General_CI_AS PRIMARY KEY NOT NULL,
    [LogicalDatabaseName] [nvarchar](128) COLLATE Latin1_General_CI_AS NOT NULL
  )

IF NOT EXISTS
(
  SELECT 1
  FROM [sys].[foreign_keys] [fk]
  INNER JOIN [sys].[schemas] [s]
    ON [fk].[schema_id] = [s].[schema_id]
  WHERE
    [fk].[name] = N'FK_Schemas_LogicalDatabaseName' AND
    [s].[name] = 'Metadata'
)
  ALTER TABLE [Metadata].[Schemas]
  ADD CONSTRAINT [FK_Schemas_LogicalDatabaseName]
    FOREIGN KEY ([LogicalDatabaseName])
    REFERENCES [Metadata].[LogicalDatabases] ([Name])
";

        /// <summary>
        /// Létrehozza a metaadatok tábláit
        /// </summary>
        /// <param name="connection">Kapcsolati infó</param>
        /// <param name="transaction">Tranzakciópéldány</param>
        private static void CreateMetadataSchema(SqlConnection connection, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(CREATE_METADATA_SCHEMA_QUERY, connection, transaction))
            {
                DeploymentTransaction.Current.LogSqlCommand(command);
                command.ExecuteNonQuery();
            }
        }

        // --- A metaadatokat lekérdező utasítások
        private const string GET_METADATA_QUERY =
@"SELECT [Version]
FROM [Metadata].[LogicalDatabases]
WHERE [Name] = @name

SELECT [Name]
FROM [Metadata].[Schemas]
WHERE [LogicalDatabaseName] = @name
";

        /// <summary>
        /// Lekérdezi a metadatokat
        /// </summary>
        /// <param name="connection">Kapcsolati infó</param>
        /// <param name="transaction">Tranzakciópéldány</param>
        /// <param name="databaseName">Adatbázisnév</param>
        /// <param name="currentVersion">Adatbázis-változat</param>
        /// <param name="existingSchemaNames">A már létező sémák neve</param>
        private static void GetMetadata(
            SqlConnection connection,
            SqlTransaction transaction,
            string databaseName,
            out DatabaseVersion currentVersion,
            out HashSet<string> existingSchemaNames)
        {
            using (var command = new SqlCommand(GET_METADATA_QUERY, connection, transaction))
            {
                command.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar) { Value = databaseName });

                var dataSet = new DataSet();

                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dataSet);
                }

                if (dataSet.Tables.Count != 2)
                {
                    throw new ApplicationException("Metadata retrieval query returned unexpected number of tables.");
                }

                var versionTable = dataSet.Tables[0];
                var schemaNameTable = dataSet.Tables[1];

                switch (versionTable.Rows.Count)
                {
                    case 0:
                        currentVersion = null;
                        break;
                    case 1:
                        var version = versionTable.Rows[0][0];
                        if (version == null || version == DBNull.Value)
                            currentVersion = null;
                        else
                        {
                            if (!(version is string) ||
                                !DatabaseVersion.TryParse((string)version, out currentVersion))
                            {
                                throw new ApplicationException("The installed logical database version is invalid.");
                            }
                        }
                        break;
                    default:
                        throw new ApplicationException("Metadata retrieval query returned multiple version numbers.");
                }

                existingSchemaNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
                foreach (var schemaName in from DataRow schemaNameRow in schemaNameTable.Rows select schemaNameRow[0])
                {
                    if (schemaName == null || schemaName == DBNull.Value || !(schemaName is string))
                    {
                        throw new ApplicationException("The installed logical database has an invalid schema name.");
                    }

                    existingSchemaNames.Add((string)schemaName);
                }
            }
        }

        #endregion
    }
}
