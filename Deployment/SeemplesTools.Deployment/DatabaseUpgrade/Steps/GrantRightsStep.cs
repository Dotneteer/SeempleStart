using System;
using System.Data;
using System.Data.SqlClient;
using SeemplesTools.Deployment.Infrastructure;

namespace SeemplesTools.Deployment.DatabaseUpgrade.Steps
{
    /// <summary>
    /// <see cref="Step"/> that grants rights to all objects in the schema to a user.
    /// </summary>
    public class GrantRightsStep : Step
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
            get { return String.Format("Grant rights to {0} on {1}", _user, StepGroup.DatabaseName); }
        }

        /// <summary>
        /// Name of the user to grant rights to.
        /// </summary>
        private readonly string _user;

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a new instance of the <see cref="GrantRightsStep"/> class.
        /// </summary>
        /// <param name="stepGroup">The step group containing the newly initialized step.</param>
        /// <param name="user">Name of the user to grant rights to.</param>
        public GrantRightsStep(
            StepGroup stepGroup,
            string user)
            : base(stepGroup)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            _user = user;
        }

        #endregion

        #region Execution

        // --- A felhasználói jogosultságot létrehozó szkript
        private const string GRANT_RIGHTS_TO_USER_QUERY =
@"DECLARE @quotedSchema [nvarchar](258)
DECLARE @quotedUser [nvarchar](258)
SET @quotedSchema = QUOTENAME(@schema)
SET @quotedUser = QUOTENAME(@user)
DECLARE @query [nvarchar](max)

IF NOT EXISTS
(
  SELECT 1
  FROM [sys].[server_principals] s
  WHERE
    [s].[type] IN ('U', 'S') AND
    [s].[name] = @user
)
BEGIN
  SET @query = N'CREATE LOGIN ' + @quotedUser + ' FROM WINDOWS'
  EXEC sp_executesql @query
END

DECLARE @existingUser [nvarchar](128)

SELECT @existingUser = [d].[name]
FROM [sys].[database_principals] [d]
INNER JOIN [sys].[server_principals] [s]
  ON [d].[sid] = [s].[sid]
WHERE
  [s].[type] IN ('U', 'S') AND
  [s].[name] = @user

IF @existingUser IS NULL
BEGIN
  SET @query = N'CREATE USER ' + @quotedUser + ' FOR LOGIN ' + @quotedUser
  EXEC sp_executesql @query
END
ELSE
  SET @quotedUser = QUOTENAME(@existingUser)

IF @quotedUser NOT IN ('[sa]', '[dbo]', '[information_schema]', '[sys]', QUOTENAME(USER_NAME()), @quotedSchema)
BEGIN
  SET @query = N'GRANT CONNECT TO ' + @quotedUser
  EXEC sp_executesql @query
  SET @query = N'GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE ON SCHEMA::' + @quotedSchema + N' TO ' + @quotedUser
  EXEC sp_executesql @query
END
";

        /// <summary>
        /// Grant rights to the specified user.
        /// </summary>
        /// <param name="connection">SQL connection to the database containing the logical database the step should be
        /// performed on.</param>
        /// <param name="transaction">SQL transaction to use.</param>
        public override void Execute(SqlConnection connection, SqlTransaction transaction)
        {
            foreach (var schemaName in StepGroup.SchemaNames)
            {
                using (var command = new SqlCommand(GRANT_RIGHTS_TO_USER_QUERY, connection, transaction))
                {
                    command.Parameters.Add("@schema", SqlDbType.NVarChar, 128);
                    command.Parameters["@schema"].Value = schemaName;
                    command.Parameters.Add("@user", SqlDbType.NVarChar, 128);
                    command.Parameters["@user"].Value = _user;

                    DeploymentTransaction.Current.LogSqlCommand(command);
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion
    }
}
