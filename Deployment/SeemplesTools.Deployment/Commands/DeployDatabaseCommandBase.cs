using System;
using System.Collections.Generic;
using System.IO;
using SeemplesTools.Deployment.Common;
using SeemplesTools.Deployment.Infrastructure;

namespace SeemplesTools.Deployment.Commands
{
    /// <summary>
    /// This class defines an abstract command that can be used as a base class for all commands 
    /// that intend to deploy a database.
    /// </summary>
    public abstract class DeployDatabaseCommandBase : ContainerCommand
    {
        private string _sqlInstance;
        private string _sqlDatabase;
        private string _userName;
        private string _password;
        private readonly List<string> _paths;
        private readonly List<string> _serviceUsers;
        private bool _insertTestData;

        /// <summary>
        /// A parancs neve
        /// </summary>
        public override string Name
        {
            get { return String.Format("{0} database", DatabaseName); }
        }

        /// <summary>
        /// Text describing the progress phase
        /// </summary>
        public override string ProgressText
        {
            get { return String.Format("Deploying {0} database", DatabaseName); }
        }

        /// <summary>
        /// Text describing the commit phase
        /// </summary>
        public override string CommitProgressText
        {
            get { return String.Format("Committing {0} database", DatabaseName); }
        }

        /// <summary>
        /// Text describing the rollback phase
        /// </summary>
        public override string RollbackProgressText
        {
            get { return String.Format("Rolling back {0} database", DatabaseName); }
        }

        /// <summary>
        /// Initializes a new command instance with no parameters
        /// </summary>
        protected DeployDatabaseCommandBase()
        {
            _paths = new List<string>();
            _serviceUsers = new List<string>();
        }

        /// <summary>
        /// Initializes a deployment command with the specified parameters
        /// </summary>
        /// <param name="sqlInstance">The name of the SQL instance</param>
        /// <param name="sqlDatabase">The name of the SQL database</param>
        /// <param name="userName">Az SQL felhasználó neve</param>
        /// <param name="password">Az SQL felhasználó jelszava</param>
        /// <param name="paths">Folders of the SQL scripts to execute</param>
        /// <param name="serviceUsers">Users with full access to the database</param>
        /// <param name="inserTestData">Should test data to be inserted into the database?</param>
        protected DeployDatabaseCommandBase(
            string sqlInstance,
            string sqlDatabase,
            string userName,
            string password,
            IEnumerable<string> paths,
            IEnumerable<string> serviceUsers,
            bool inserTestData)
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
            _userName = userName == null ? null : userName.Trim();
           _password = password == null ? null : password.Trim();

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

            _insertTestData = inserTestData;
        }

        /// <summary>
        /// Parse command arguments
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

                case "username":
                case "user":
                case "userid":
                    argument = ParameterHelper.Mandatory(argument, original).Trim();
                    if (argument.Length == 0)
                    {
                        throw new ParameterException("User name cannot be empty.");
                    }
                    _userName = argument;
                    return true;

                case "password":
                case "pwd":
                    argument = ParameterHelper.Mandatory(argument, original).Trim();
                    if (argument.Length == 0)
                    {
                        throw new ParameterException("Password cannot be empty.");
                    }
                    _password = argument;
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
                    {
                        return true;
                    }

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
                        _paths.Add(isAbsolute ? path : Path.Combine(Environment.CurrentDirectory, path));
                    }
                    return true;
                
                case "serviceuser":
                case "serviceusers":
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
        /// Finalize the initialization of the command
        /// </summary>
        protected override void DoFinishInitialization()
        {
            CheckMandatoryParameter(_sqlInstance, "sql-instance", Original);
            CheckMandatoryParameter(_sqlDatabase, "sql-database", Original);

            if (_paths.Count == 0)
            {
                _paths.Add(Path.Combine(Environment.CurrentDirectory, "Databases"));
            }

            Commands.Add(new CreateSqlDatabaseCommand(
                _sqlInstance,
                _sqlDatabase,
                _userName,
                _password,
                true));

            Commands.Add(new CreateLogicalDatabasesCommand(
                _sqlInstance,
                _sqlDatabase,
                _userName,
                _password,
                LogicalDatabases,
                _paths,
                _serviceUsers,
                _insertTestData));

            base.DoFinishInitialization();
        }

        /// <summary>
        /// Override this property to define the name of the database
        /// </summary>
        protected abstract string DatabaseName { get; }

        /// <summary>
        /// Override this property to defines the list of logical databases to be
        /// deployed with the derived command
        /// </summary>
        protected abstract List<DatabaseNameVersionPair> LogicalDatabases { get; }
    }
}