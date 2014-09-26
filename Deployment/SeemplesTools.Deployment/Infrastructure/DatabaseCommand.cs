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
        /// <param name="connection">A kapcsolati információ</param>
        /// <param name="transaction">Az elindított tranzakciót leíró példány</param>
        protected void GetConnection(
            string instance,
            string database,
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

            var connectionString = DatabaseHelper.GetConnectionString(instance, database);
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
                var databaseTransaction = (DatabaseTransaction)DeploymentTransaction.Current.Properties[transactionKey];
                if (!databaseTransaction.Commands.Contains(this))
                {
                    _transactions.Add(databaseTransaction);
                    databaseTransaction.Commands.Add(this);
                }

                connection = (SqlConnection)DeploymentTransaction.Current.Properties[connectionKey];
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
