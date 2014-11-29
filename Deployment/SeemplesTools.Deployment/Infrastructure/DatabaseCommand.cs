using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SeemplesTools.Deployment.Common;

namespace SeemplesTools.Deployment.Infrastructure
{
    /// <summary>
    /// Ez az osztály egy adatbázison működő parancsot valósít meg.
    /// </summary>
    public abstract class DatabaseCommand : LeafCommand
    {
        protected string SqlInstance;
        protected string SqlDatabase;
        protected string UserName;
        protected string Password;

        #region Connect to SQL Server

        /// <summary>
        /// Az egyes adatbázisokhoz tartozó tranzakciók listája
        /// </summary>
        private readonly List<DatabaseTransaction> _transactions = new List<DatabaseTransaction>();

        /// <summary>
        /// Visszadja a megadott adatbázishoz a kapcsolati információt és tranzakciópéldányt
        /// </summary>
        /// <param name="instance">Az SQL Server példány neve</param>
        /// <param name="database">Az adatbázis neve a szerveren</param>
        /// <param name="userName">Az SQL felhasználó neve</param>
        /// <param name="password">Az SQL felhasználó jelszava</param>
        /// <param name="connection">A kapcsolati információ</param>
        /// <param name="transaction">Az elindított tranzakciót leíró példány</param>
        protected void GetConnection(
            string instance,
            string database,
            string userName,
            string password,
            out SqlConnection connection,
            out SqlTransaction transaction)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (database == null)
            {
                throw new ArgumentNullException("instance");
            }

            var connectionString = DatabaseHelper.GetConnectionString(instance, database, userName, password);
            GetConnection(connectionString, instance, database, out connection, out transaction);
        }

        private void GetConnection(string connectionString, string instance, string database, 
            out SqlConnection connection, out SqlTransaction transaction)
        {
            var connectionKey = "DatabaseConnection:" + connectionString;
            var transactionKey = "DatabaseTransaction:" + connectionString;

            // --- Van már kapcsolati információ ehhez az adatbázishoz?
            if (DeploymentTransaction.Current.Properties.ContainsKey(connectionKey))
            {
                // --- Van kapcsolati infó, ekkor már tranzakciónak is lennie kell
                if (!DeploymentTransaction.Current.Properties.ContainsKey(transactionKey))
                {
                    throw new ApplicationException("Database transaction does not exist for existing SQL connection.");
                }

                // --- Ezt a parancsot is hozzávesszük a tranzakciós listához
                var databaseTransaction = (DatabaseTransaction) DeploymentTransaction.Current.Properties[transactionKey];
                if (!databaseTransaction.Commands.Contains(this))
                {
                    _transactions.Add(databaseTransaction);
                    databaseTransaction.Commands.Add(this);
                }

                connection = (SqlConnection) DeploymentTransaction.Current.Properties[connectionKey];
                transaction = databaseTransaction.Transaction;
            }
            else
            {
                // --- Még nincs az aktuális tranzakció kapcsán kapcsolódás ehhez az adatbázishoz,
                // --- ekkor tranzakciónak sem szabad lennie.
                if (DeploymentTransaction.Current.Properties.ContainsKey(transactionKey))
                {
                    throw new ApplicationException("Database transaction exists for nonexistent SQL connection.");
                }

                // --- Előállítjuk az adatbázis kapcsolatot, és elindítjuk a tranzakciót
                DeploymentTransaction.Current.Log("Connecting to SQL database {0} on SQL instance {1}.",
                    database, instance);

                connection = new SqlConnection(connectionString);
                connection.Open();

                DeploymentTransaction.Current.Log("Starting new transaction.");
                var databaseTransaction = new DatabaseTransaction
                {
                    Transaction = transaction = connection.BeginTransaction(IsolationLevel.Serializable)
                };
                _transactions.Add(databaseTransaction);
                databaseTransaction.Commands.Add(this);

                DeploymentTransaction.Current.Properties.Add(connectionKey, connection);
                DeploymentTransaction.Current.Properties.Add(transactionKey, databaseTransaction);
            }
        }

        #endregion

        #region Initialization

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
                    SqlInstance = argument;
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
                    SqlDatabase = argument;
                    return true;

                case "user":
                case "username":
                case "userid":
                case "u":
                    argument = ParameterHelper.Mandatory(argument, original).Trim();
                    if (argument.Length == 0)
                    {
                        throw new ParameterException("User name cannot be empty.");
                    }
                    UserName = argument;
                    return true;

                case "password":
                case "pwd":
                case "p":
                    argument = ParameterHelper.Mandatory(argument, original).Trim();
                    if (argument.Length == 0)
                    {
                        throw new ParameterException("Password cannot be empty.");
                    }
                    Password = argument;
                    return true;

                default:
                    return false;
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

        #region Commit/rollback

        /// <summary>
        /// A parancs véglegesítése
        /// </summary>
        protected override void DoCommit()
        {
            CommitDatabaseTransaction();
        }

        /// <summary>
        /// A parancs visszagörgetése
        /// </summary>
        protected override void DoRollback()
        {
            RollbackDatabaseTransaction();
        }

        /// <summary>
        /// Az adatbázis tranzakció véglegesítése
        /// </summary>
        protected virtual void CommitDatabaseTransaction()
        {
            foreach (var transaction in _transactions)
            {
                transaction.Commands.Remove(this);
                if (transaction.Commands.Count != 0 || transaction.Transaction == null) continue;
                DeploymentTransaction.Current.Log("Committing database transaction.");
                transaction.Transaction.Commit();
                transaction.Transaction = null;
            }
        }

        /// <summary>
        /// Az adatbázis tranzakció visszagörgetése
        /// </summary>
        protected virtual void RollbackDatabaseTransaction()
        {
            foreach (var transaction in _transactions)
            {
                transaction.Commands.Remove(this);
                if (transaction.Commands.Count != 0 || transaction.Transaction == null) continue;
                DeploymentTransaction.Current.Log("Rolling back database transaction.");
                transaction.Transaction.Rollback();
                transaction.Transaction = null;
            }
        }

        #endregion
    }
}
