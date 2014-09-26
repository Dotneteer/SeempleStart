using System;
using System.Data;
using System.Data.SqlClient;
using SeemplesTools.Deployment.Infrastructure;

namespace SeemplesTools.Deployment.DatabaseUpgrade.Steps
{
    /// <summary>
    /// <see cref="Step"/> that sets version information and schema list for a logical database in the Metadata
    /// schema.
    /// </summary>
    public class SetMetadataStep : Step
    {
        #region Properties

        /// <summary>
        /// Short description (not including database name).
        /// </summary>
        public override string ShortDescription
        {
            get { return null; }
        }

        /// <summary>
        /// Long description (including database name).
        /// </summary>
        public override string LongDescription
        {
            get { return String.Format("Store metadata for {0}", StepGroup.DatabaseName); }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a new instance of the <see cref="SetMetadataStep"/> class.
        /// </summary>
        /// <param name="stepGroup">The step group containing the newly initialized step.</param>
        public SetMetadataStep(StepGroup stepGroup)
            : base(stepGroup)
        {
        }

        #endregion

        #region Execution

        // --- A metaadatok beállítását végző utasítások
        private const string SET_METADATA_QUERY =
@"DELETE FROM [Metadata].[Schemas]
WHERE [LogicalDatabaseName] = @name

IF @version IS NULL
  DELETE FROM [Metadata].[LogicalDatabases]
  WHERE [Name] = @name
ELSE
BEGIN
  IF EXISTS
  (
    SELECT 1
    FROM [Metadata].[LogicalDatabases]
    WHERE [Name] = @name
  )
    UPDATE [Metadata].[LogicalDatabases]
    SET [Version] = @version
    WHERE [Name] = @name
  ELSE
    INSERT INTO [Metadata].[LogicalDatabases]
    (
      [Name],
      [Version]
    )
    VALUES
    (
      @name,
      @version
    )
END
";

        // --- A séma hozzáadását végző utasítások
        private const string ADD_SCHEMA_QUERY =
@"INSERT INTO [Metadata].[Schemas]
(
  [Name],
  [LogicalDatabaseName]
)
VALUES
(
  @schemaName,
  @logicalDatabaseName
)
";

        /// <summary>
        /// Set metadata in the Metadata schema.
        /// </summary>
        /// <param name="connection">SQL connection to the database containing the logical database the step should be
        /// performed on.</param>
        /// <param name="transaction">SQL transaction to use.</param>
        public override void Execute(SqlConnection connection, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(SET_METADATA_QUERY, connection, transaction))
            {
                command.Parameters.Add("@name", SqlDbType.NVarChar, 128);
                command.Parameters["@name"].Value = StepGroup.DatabaseName;
                command.Parameters.Add("@version", SqlDbType.NVarChar, 128);
                command.Parameters["@version"].Value =
                    StepGroup.TargetVersion == null
                        ? (object)DBNull.Value
                        : StepGroup.TargetVersion.ToString();

                DeploymentTransaction.Current.LogSqlCommand(command);
                command.ExecuteNonQuery();
            }

            foreach (var schemaName in StepGroup.SchemaNames)
            {
                using (var command = new SqlCommand(ADD_SCHEMA_QUERY, connection, transaction))
                {
                    command.Parameters.Add("@logicalDatabaseName", SqlDbType.NVarChar, 128);
                    command.Parameters["@logicalDatabaseName"].Value = StepGroup.DatabaseName;
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
