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
    public class DropSchemasStep : Step
    {
        #region Properties

        /// <summary>
        /// Short description (not including database name).
        /// </summary>
        public override string ShortDescription
        {
            get
            {
                return String.Format("delete {0}: {1}",
                    _schemasToDelete.Count == 1 ? "schema" : "schemas",
                    String.Join(", ", _schemasToDelete));
            }
        }

        /// <summary>
        /// Long description (including database name).
        /// </summary>
        public override string LongDescription
        {
            get
            {
                return String.Format("Delete {0} {1} for {2}",
                    _schemasToDelete.Count == 1 ? "schema" : "schemas",
                    String.Join(", ", _schemasToDelete),
                    StepGroup.DatabaseName);
            }
        }

        /// <summary>
        /// Name of the schemas to delete.
        /// </summary>
        private readonly HashSet<string> _schemasToDelete;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes an instance of the <see cref="DropSchemasStep"/> class.
        /// </summary>
        /// <param name="stepGroup">The step group containing the newly initialized step.</param>
        /// <param name="schemasToDelete">The name of the schemas to be deleted.</param>
        public DropSchemasStep(
            StepGroup stepGroup,
            IEnumerable<string> schemasToDelete)
            : base(stepGroup)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (schemasToDelete == null || !schemasToDelete.Any())
            {
                throw new ArgumentNullException("schemasToDelete");
            }

            // ReSharper disable once PossibleMultipleEnumeration
            _schemasToDelete = new HashSet<string>(schemasToDelete, StringComparer.InvariantCultureIgnoreCase);
        }

        #endregion

        #region Execution

        // --- A séma eltávolítását végző utasítások
        private const string DROP_SCHEMA_QUERY =
@"DECLARE @quotedSchema [nvarchar](258)
SET @quotedSchema = QUOTENAME(@schemaName)

IF OBJECT_ID('tempdb..#drop') IS NOT NULL
  DROP TABLE #drop

CREATE TABLE #drop
(
  Id [int] IDENTITY(1, 1),
  Command [nvarchar](max)
)

INSERT INTO #drop
SELECT 'ALTER TABLE ' + @quotedSchema + '.' + QUOTENAME([t].[name]) + ' DROP CONSTRAINT ' + QUOTENAME([o].[name])
FROM [sys].[objects] o
INNER JOIN [sys].[tables] t
  ON [o].[parent_object_id] = [t].[object_id]
INNER JOIN [sys].[schemas] s
  ON [t].[schema_id] = [s].[schema_id]
WHERE
  [s].[name] = @schemaName AND
  [o].[type] IN ('C', 'D', 'F', 'PK', 'UQ')
ORDER BY
  CASE WHEN [o].[type] = 'F'
       THEN 1
       WHEN [o].[type] = 'PK'
       THEN 2
       ELSE 3
  END

INSERT INTO #drop
SELECT
  CASE WHEN [o].[type] IN ('FN', 'FS', 'FT', 'IF', 'TF')
         THEN N'DROP FUNCTION ' + @quotedSchema + N'.' + QUOTENAME([o].[name])
       WHEN [o].[type] = 'P'
         THEN N'DROP PROCEDURE ' + @quotedSchema + N'.' + QUOTENAME([o].[name])
       WHEN [o].[type] = 'TR'
         THEN N'DROP TRIGGER ' + @quotedSchema + N'.' + QUOTENAME([o].[name])
       WHEN [o].[type] = 'U'
         THEN N'DROP TABLE ' + @quotedSchema + N'.' + QUOTENAME([o].[name])
       WHEN [o].[type] = 'V'
         THEN N'DROP VIEW ' + @quotedSchema + N'.' + QUOTENAME([o].[name])
  END
FROM [sys].[objects] o
INNER JOIN [sys].[schemas] s
  ON [o].[schema_id] = [s].[schema_id]
WHERE
  [s].[name] = @schemaName AND
  [o].[type] IN ('FN', 'FS', 'FT', 'IF', 'TF', 'P', 'TR', 'U', 'V')
ORDER BY
  CASE WHEN [o].[type] IN ('FN', 'FS', 'FT', 'IF', 'TF')
       THEN 1
       WHEN [o].[type] = 'P'
       THEN 2
       WHEN [o].[type] = 'TR'
       THEN 3
       WHEN [o].[type] = 'V'
       THEN 4
       WHEN [o].[type] = 'U'
       THEN 5
  END

INSERT INTO #drop
SELECT N'DROP SCHEMA ' + @quotedSchema

DECLARE @command [nvarchar](max)

DECLARE CommandCursor CURSOR FOR
SELECT [Command]
FROM #drop
ORDER BY [Id]

OPEN CommandCursor

FETCH CommandCursor INTO @command
WHILE (@@FETCH_STATUS = 0)
BEGIN
  EXEC sp_executesql @command
  FETCH CommandCursor INTO @command
END

CLOSE CommandCursor
DEALLOCATE CommandCursor
";

        /// <summary>
        /// Delete schemas for the logical database.
        /// </summary>
        /// <param name="connection">SQL connection to the database containing the logical database the step should be
        /// performed on.</param>
        /// <param name="transaction">SQL transaction to use.</param>
        public override void Execute(SqlConnection connection, SqlTransaction transaction)
        {
            foreach (var schemaName in _schemasToDelete)
            {
                using (var command = new SqlCommand(DROP_SCHEMA_QUERY, connection, transaction))
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
