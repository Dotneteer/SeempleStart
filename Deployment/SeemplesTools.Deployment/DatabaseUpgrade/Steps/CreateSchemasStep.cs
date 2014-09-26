using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SeemplesTools.Deployment.Infrastructure;

namespace SeemplesTools.Deployment.DatabaseUpgrade.Steps
{
    /// <summary>
    /// <see cref="Step"/> that creates schemas for a logical database.
    /// </summary>
    public class CreateSchemasStep : Step
    {
        #region Properties

        /// <summary>
        /// Short description (not including database name).
        /// </summary>
        public override string ShortDescription
        {
            get
            {
                return String.Format("new {0}: {1}",
                    _schemasToCreate.Count == 1 ? "schema" : "schemas",
                    String.Join(", ", _schemasToCreate));
            }
        }

        /// <summary>
        /// Long description (including database name).
        /// </summary>
        public override string LongDescription
        {
            get
            {
                return String.Format("Create {0} {1} for {2}",
                    _schemasToCreate.Count == 1 ? "schema" : "schemas",
                    String.Join(", ", _schemasToCreate),
                    StepGroup.DatabaseName);
            }
        }

        /// <summary>
        /// Name of the schemas to create.
        /// </summary>
        private readonly HashSet<string> _schemasToCreate;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes an instance of the <see cref="CreateSchemasStep"/> class.
        /// </summary>
        /// <param name="stepGroup">The step group containing the newly initialized step.</param>
        /// <param name="schemasToCreate">The name of the schemas to be created.</param>
        public CreateSchemasStep(
            StepGroup stepGroup,
            IEnumerable<string> schemasToCreate)
            : base(stepGroup, true)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (schemasToCreate == null || !schemasToCreate.Any())
            {
                throw new ArgumentNullException("schemasToCreate");
            }

            // ReSharper disable once PossibleMultipleEnumeration
            _schemasToCreate = new HashSet<string>(schemasToCreate, StringComparer.InvariantCultureIgnoreCase);
        }

        #endregion

        #region Execution

        /// <summary>
        /// --- A séma létrehozását végző utasítás
        /// </summary>
        private const string CREATE_SCHEMA_QUERY =
@"IF NOT EXISTS
(
  SELECT 1
  FROM [sys].[schemas] [s]
  WHERE [s].[name] = @schemaName
)
BEGIN
  DECLARE @query [nvarchar](max)
  SET @query = N'CREATE SCHEMA ' + QUOTENAME(@schemaName)
  EXEC sp_executesql @query
END
";

        /// <summary>
        /// Create schemas for the logical database.
        /// </summary>
        /// <param name="connection">SQL connection to the database containing the logical database the step should be
        /// performed on.</param>
        /// <param name="transaction">SQL transaction to use.</param>
        public override void Execute(SqlConnection connection, SqlTransaction transaction)
        {
            foreach (var schemaName in _schemasToCreate)
            {
                using (var command = new SqlCommand(CREATE_SCHEMA_QUERY, connection, transaction))
                {
                    command.Parameters.Add("@schemaName", SqlDbType.NVarChar, 128);
                    command.Parameters["@schemaName"].Value = schemaName;

                    DeploymentTransaction.Current.LogSqlCommand(command);
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion
    }
}
