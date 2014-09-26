using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Seemplest.Core.DataAccess.Exceptions;

namespace Seemplest.MsSql.DataAccess
{
    /// <summary>
    /// This class provides helper functions to handle database errors.
    /// </summary>
    public static class DatabaseExceptionHelper
    {
        /// <summary>
        /// Transforms the specified SQL Server exception into the appropriate Nexon.Framework
        /// exception.
        /// </summary>
        /// <param name="exception">SQL Exception instance</param>
        /// <returns>
        /// Transformed Nexon.Framework exception.
        /// </returns>
        public static Exception TransformSqlException(SqlException exception)
        {
            // --- Check for primary key violation
            const string PK_ERROR_PATTERN = @"primary key constraint '(.+?)'.*in object '(.+?)'";
            var regex = new Regex(PK_ERROR_PATTERN, RegexOptions.IgnoreCase);
            var match = regex.Match(exception.Message);
            if (match.Success)
            {
                return new PrimaryKeyViolationException(match.Groups[2].Captures[0].Value, exception);
            }

            // --- Check for unique key violation
            const string UK_ERROR_PATTERN1 = @"unique key constraint '(.+?)'.*in object '(.+?)'";
            regex = new Regex(UK_ERROR_PATTERN1, RegexOptions.IgnoreCase);
            match = regex.Match(exception.Message);
            if (match.Success)
            {
                return new UniqueKeyViolationException(match.Groups[2].Captures[0].Value,
                                                      match.Groups[1].Captures[0].Value, exception);
            }

            const string UK_ERROR_PATTERN2 = @"Cannot insert duplicate key row in object '(.+?)' with unique index '(.+?)'.";
            regex = new Regex(UK_ERROR_PATTERN2, RegexOptions.IgnoreCase);
            match = regex.Match(exception.Message);
            if (match.Success)
            {
                return new UniqueIndexViolationException(match.Groups[1].Captures[0].Value,
                                                      match.Groups[2].Captures[0].Value, exception);
            }

            // --- Check for foreign key violation
            const string FK1_ERROR_PATTERN = "foreign key constraint \"(.+?)\"";
            regex = new Regex(FK1_ERROR_PATTERN, RegexOptions.IgnoreCase);
            match = regex.Match(exception.Message);
            if (match.Success)
            {
                return new ForeignKeyViolationException(match.Groups[1].Captures[0].Value, exception);
            }
            const string FK2_ERROR_PATTERN = "reference constraint \"(.+?)\"";
            regex = new Regex(FK2_ERROR_PATTERN, RegexOptions.IgnoreCase);
            match = regex.Match(exception.Message);
            if (match.Success)
            {
                return new ForeignKeyViolationException(match.Groups[1].Captures[0].Value, exception);
            }

            // --- Check for NULL insertion
            const string NULL_ERROR_PATTERN = @"Cannot insert the value NULL into column '(.+?)'.*table '(.+?)'";
            regex = new Regex(NULL_ERROR_PATTERN, RegexOptions.IgnoreCase);
            match = regex.Match(exception.Message);
            if (match.Success)
            {
                return new NullValueNotAllowedException(match.Groups[2].Captures[0].Value,
                                                      match.Groups[1].Captures[0].Value, exception);
            }

            // --- Check for CHECK constraint violation
            const string CHECK_CONST_ERROR_PATTERN = "CHECK constraint \"(.+?)\".*table \"(.+?)\"";
            regex = new Regex(CHECK_CONST_ERROR_PATTERN, RegexOptions.IgnoreCase);
            match = regex.Match(exception.Message);
            if (match.Success)
            {
                return new CheckConstraintViolationException(match.Groups[2].Captures[0].Value,
                                                      match.Groups[1].Captures[0].Value, exception);
            }
            return exception;
        }
    }
}