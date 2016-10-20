using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace Seemplest.FbSql.DataAccess
{
    /// <summary>
    /// This class represents an SQL expression
    /// </summary>
    public sealed class SqlExpression
    {
        private const string SEPARATOR = ", ";

        // --- Stores the next expression in the chain
        private SqlExpression _nextExpression;

        // --- Stores the inner SQL text
        private string _innerSql;

        // --- Stores the inner SQL arguments
        private object[] _innerArgs;

        // --- Stores the built string of this SQL expression
        private string _sqlBuilt;

        // --- Stores the built arguments of this SQL expression
        private object[] _argsBuilt;

        // --- This regex defines the SQL parameter token.
        private static readonly Regex s_RxParams = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);

        // --- This regex defines a select clause for parsing purposes
        private static readonly Regex s_RxSimpleSelectClause =
            new Regex(@"\A(\s*select\s+)(?:(all|distinct)\s*)?(?:(top)\s*(?:\()?(\d+)(?:\s*\))?\s*(percent)?\s*(with\s+ties)?)?(.*)$",
                      RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Creates an empty instance
        /// </summary>
        public SqlExpression()
        {
        }

        /// <summary>
        /// Creates an instance with the specified strings and parameters.
        /// </summary>
        /// <param name="sql">SQL text</param>
        /// <param name="args">SQL arguments</param>
        public SqlExpression(string sql, IEnumerable<object> args = null)
        {
            _innerSql = sql;
            _innerArgs = args?.ToArray();
        }

        /// <summary>
        /// Creates an instance with the specified strings and parameters.
        /// </summary>
        /// <param name="sql">SQL text</param>
        /// <param name="args">SQL arguments</param>
        public SqlExpression(string sql, params object[] args)
        {
            _innerSql = sql;
            _innerArgs = args;
        }

        /// <summary>
        /// Factory property to create a new instance
        /// </summary>
        public static SqlExpression New => new SqlExpression();

        /// <summary>
        /// Creates a new SqlExpression instance with the specified attributes.
        /// </summary>
        /// <param name="sql">SQL text</param>
        /// <param name="args">SQL arguments</param>
        /// <returns></returns>
        public static SqlExpression CreateFrom(string sql, IEnumerable<object> args = null)
        {
            return new SqlExpression(sql, args);
        }

        /// <summary>
        /// Creates a new SqlExpression instance with the specified attributes.
        /// </summary>
        /// <param name="sql">SQL text</param>
        /// <param name="args">SQL arguments</param>
        /// <returns></returns>
        public static SqlExpression CreateFrom(string sql, params object[] args)
        {
            return new SqlExpression(sql, args);
        }

        /// <summary>
        /// Gets the SQL text of this expression (with parameter placeholders)
        /// </summary>
        public string SqlText
        {
            get
            {
                BuildExpression();
                return _sqlBuilt;
            }
        }

        /// <summary>
        /// Gets the arguments of this expression (to replace the placeholders)
        /// </summary>
        public object[] Arguments
        {
            get
            {
                BuildExpression();
                return _argsBuilt;
            }
        }

        /// <summary>
        /// Apeends the specified SQL expression to this expression.
        /// </summary>
        /// <param name="expr">Expression to append</param>
        /// <returns>This expression</returns>
        public SqlExpression Append(SqlExpression expr)
        {
            if (_nextExpression != null) _nextExpression.Append(expr);
            else if (_innerSql != null) _nextExpression = expr;
            else
            {
                _innerSql = expr.SqlText;
                _innerArgs = expr.Arguments;
                _nextExpression = expr._nextExpression;
            }
            return this;
        }

        /// <summary>
        /// Appends a new SQL expression to this expression.
        /// </summary>
        /// <param name="sql">SQL string</param>
        /// <param name="args">Arguments</param>
        /// <returns>This expression</returns>
        public SqlExpression Append(string sql, object[] args)
        {
            return Append(new SqlExpression(sql, args));
        }

        /// <summary>
        /// Appends a SELECT clause with the specified columns
        /// </summary>
        /// <param name="columns">Columns to put into the SELECT clause</param>
        /// <returns>SELECT clause</returns>
        public SqlExpression Select(IEnumerable<object> columns)
        {
            if (columns == null) throw new ArgumentNullException(nameof(columns));
            var columnList = columns.ToList();
            if (columnList.Count == 0)
                throw new ArgumentException("At least one column should be specified", nameof(columns));
            return Append(new SqlExpression("select " +
                string.Join(SEPARATOR, columnList.Select(c => c.ToString() == "*" ? "*" : "\"" + c.ToString() + "\""))));
        }

        /// <summary>
        /// Appends a SELECT clause with the specified columns
        /// </summary>
        /// <param name="firstColumn">First columns to put into the SELECT clause</param>
        /// <param name="otherColumns">Other columns to put into the SELECT clause</param>
        /// <returns>SELECT clause</returns>
        public SqlExpression Select(object firstColumn, params object[] otherColumns)
        {
            var columns = new object[otherColumns.Length + 1];
            columns[0] = firstColumn;
            for (var i = 0; i < otherColumns.Length; i++) columns[i + 1] = otherColumns[i];
            return Select(columns);
        }

        /// <summary>
        /// Appends a SELECT clause according to the properties of <typeparamref name="TData"/>
        /// </summary>
        /// <typeparam name="TData">Data record type</typeparam>
        /// <returns>SELECT clause</returns>
        public SqlExpression Select<TData>()
        {
            return Select(RecordMetadataManager
                              .GetMetadata(typeof(TData))
                              .DataColumns
                              .Select(c => c.ColumnName));
        }

        /// <summary>
        /// Appends a select clause using the columns specified with expressions.
        /// </summary>
        /// <typeparam name="TData">Data record type</typeparam>
        /// <param name="firstProp">First property selector</param>
        /// <param name="otherProps">Other property selectors</param>
        /// <returns>SELECT clause</returns>
        public SqlExpression Select<TData>(Expression<Func<TData, object>> firstProp,
                                              params Expression<Func<TData, object>>[] otherProps)
        {
            var paramNames = new List<string> { GetPropertyName(firstProp) };
            paramNames.AddRange(otherProps.Select(GetPropertyName));
            return Select(paramNames);
        }

        /// <summary>
        /// Sets this SELECT clause to SELECT DISTINCT provided this is a SELECT clause
        /// </summary>
        public SqlExpression Distinct
        {
            get
            {
                var match = s_RxSimpleSelectClause.Match(_innerSql);
                if (match.Success)
                {
                    // --- Ok, this is a SELECT clause
                    if (string.Compare(match.Groups[2].Value, "distinct", StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        // --- This is not a SELECT DISTINCT
                        if (string.Compare(match.Groups[2].Value, "all", StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            // --- This is a SELECT ALL, so change to SELECT DISTINCT
                            var capture = match.Groups[2].Captures[0];
                            _innerSql = _innerSql.Substring(0, capture.Index) + "distinct" +
                                       _innerSql.Substring(capture.Index + capture.Length);
                        }
                        else
                        {
                            // --- This is a SELECT without a DISTINCT
                            var capture = match.Groups[1].Captures[0];
                            _innerSql = _innerSql.Substring(0, capture.Index + capture.Length) + "distinct " +
                                       _innerSql.Substring(capture.Index + capture.Length);
                        }
                    }
                }
                return this;
            }
        }

        /// <summary>
        /// Sets this SELECT clause to SELECT ALL provided this is a SELECT clause
        /// </summary>
        public SqlExpression All
        {
            get
            {
                var match = s_RxSimpleSelectClause.Match(_innerSql);
                if (match.Success)
                {
                    // --- Ok, this is a SELECT clause
                    if (string.Compare(match.Groups[2].Value, "all", StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        // --- This is not a SELECT ALL
                        if (string.Compare(match.Groups[2].Value, "distinct", StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            // --- This is a SELECT DISTINCT, so change to SELECT ALL
                            var capture = match.Groups[2].Captures[0];
                            _innerSql = _innerSql.Substring(0, capture.Index) + "all" +
                                        _innerSql.Substring(capture.Index + capture.Length);
                        }
                        else
                        {
                            // --- This is a SELECT without an ALL, so add ALL
                            var capture = match.Groups[1].Captures[0];
                            _innerSql = _innerSql.Substring(0, capture.Index + capture.Length) + "all " +
                                        _innerSql.Substring(capture.Index + capture.Length);
                        }
                    }
                }
                return this;
            }
        }

        /// <summary>
        /// Appends a FROM clause with the table names provided
        /// </summary>
        /// <param name="tables">Collection of table names</param>
        /// <returns>FROM clause</returns>
        public SqlExpression From(IEnumerable<object> tables)
        {
            if (tables == null) throw new ArgumentNullException(nameof(tables));
            var tableList = tables.ToList();
            if (tableList.Count == 0)
                throw new ArgumentException("At least one table should be specified", nameof(tables));
            return Append(new SqlExpression("from " +
                string.Join(SEPARATOR, tableList.Select(c => "\"" + c.ToString() + "\""))));
        }

        /// <summary>
        /// Appends a FROM clause with the table names provided
        /// </summary>
        /// <param name="firstTable">First table name</param>
        /// <param name="otherTables">Other table names</param>
        /// <returns>FROM clause</returns>
        public SqlExpression From(object firstTable, params object[] otherTables)
        {
            var tables = new object[otherTables.Length + 1];
            tables[0] = firstTable;
            for (var i = 0; i < otherTables.Length; i++) tables[i + 1] = otherTables[i];
            return From(tables);
        }

        /// <summary>
        /// Appends a FROM clause using the specified type argument
        /// </summary>
        /// <typeparam name="T1">Table name</typeparam>
        /// <returns>FROM clause</returns>
        public SqlExpression From<T1>()
        {
            var t1Name = RecordMetadataManager.GetMetadata<T1>().TableName;
            return From(t1Name);
        }

        /// <summary>
        /// Appends a FROM clause using the specified type argument
        /// </summary>
        /// <typeparam name="T1">Table name #1</typeparam>
        /// <typeparam name="T2">Table name #2</typeparam>
        /// <returns>FROM clause</returns>
        public SqlExpression From<T1, T2>()
            where T1 : IDataRecord
            where T2 : IDataRecord
        {
            var t1Name = RecordMetadataManager.GetMetadata<T1>().TableName;
            var t2Name = RecordMetadataManager.GetMetadata<T2>().TableName;
            return From(t1Name, t2Name);
        }

        /// <summary>
        /// Appends a FROM clause using the specified type argument
        /// </summary>
        /// <typeparam name="T1">Table name #1</typeparam>
        /// <typeparam name="T2">Table name #2</typeparam>
        /// <typeparam name="T3">Table name #3</typeparam>
        /// <returns>FROM clause</returns>
        public SqlExpression From<T1, T2, T3>()
            where T1 : IDataRecord
            where T2 : IDataRecord
            where T3 : IDataRecord
        {
            var t1Name = RecordMetadataManager.GetMetadata<T1>().TableName;
            var t2Name = RecordMetadataManager.GetMetadata<T2>().TableName;
            var t3Name = RecordMetadataManager.GetMetadata<T3>().TableName;
            return From(t1Name, t2Name, t3Name);
        }

        /// <summary>
        /// Appends a FROM clause using the specified type argument
        /// </summary>
        /// <typeparam name="T1">Table name #1</typeparam>
        /// <typeparam name="T2">Table name #2</typeparam>
        /// <typeparam name="T3">Table name #3</typeparam>
        /// <typeparam name="T4">Table name #4</typeparam>
        /// <returns>FROM clause</returns>
        public SqlExpression From<T1, T2, T3, T4>()
            where T1 : IDataRecord
            where T2 : IDataRecord
            where T3 : IDataRecord
            where T4 : IDataRecord
        {
            var t1Name = RecordMetadataManager.GetMetadata<T1>().TableName;
            var t2Name = RecordMetadataManager.GetMetadata<T2>().TableName;
            var t3Name = RecordMetadataManager.GetMetadata<T3>().TableName;
            var t4Name = RecordMetadataManager.GetMetadata<T4>().TableName;
            return From(t1Name, t2Name, t3Name, t4Name);
        }

        /// <summary>
        /// Appends a WHERE clause with the specified condition and arguments
        /// </summary>
        /// <param name="condition">SQL WHERE condition</param>
        /// <param name="args">Arguments</param>
        /// <returns>WHERE clause</returns>
        public SqlExpression Where(string condition, IEnumerable<object> args = null)
        {
            return Append($"where {condition}",
                args?.ToArray());
        }

        /// <summary>
        /// Appends a WHERE clause with the specified condition and arguments
        /// </summary>
        /// <param name="condition">SQL WHERE condition</param>
        /// <param name="args">Arguments</param>
        /// <returns>WHERE clause</returns>
        public SqlExpression Where(string condition, params object[] args)
        {
            return Append($"where {condition}", args);
        }

        /// <summary>
        /// Appends a WHERE clause with the specified condition and arguments, if the predicate is true
        /// </summary>
        /// <param name="predicate">Predicate to use</param>
        /// <param name="condition">SQL WHERE condition</param>
        /// <param name="args">Arguments</param>
        /// <returns>WHERE clause</returns>
        public SqlExpression Where(bool predicate, string condition, IEnumerable<object> args = null)
        {
            return predicate
                ? Append($"where {condition}", args?.ToArray())
                : this;
        }

        /// <summary>
        /// Appends a WHERE clause with the specified condition and arguments, if the predicate is true
        /// </summary>
        /// <param name="predicate">Predicate to use</param>
        /// <param name="condition">SQL WHERE condition</param>
        /// <param name="args">Arguments</param>
        /// <returns>WHERE clause</returns>
        public SqlExpression Where(bool predicate, string condition, params object[] args)
        {
            return predicate
                ? Append($"where {condition}", args)
                : this;
        }

        /// <summary>
        /// Appends an ORDER BY clause with the specified condition and arguments
        /// </summary>
        /// <param name="columns"></param>
        /// <returns>ORDER BY clause</returns>
        public SqlExpression OrderBy(IEnumerable<object> columns)
        {
            if (columns == null) throw new ArgumentNullException(nameof(columns));
            var columnList = columns.ToList();
            if (columnList.Count == 0)
                throw new ArgumentException("At least one table should be specified", nameof(columns));
            return Append(new SqlExpression("order by " +
                string.Join(SEPARATOR, columnList.Select(c => "\"" + c.ToString() + "\""))));
        }

        /// <summary>
        /// Appends an ORDER BY clause with the specified condition and arguments
        /// </summary>
        /// <param name="firstColumn">First columns to put into the SELECT clause</param>
        /// <param name="otherColumns">Other columns to put into the SELECT clause</param>
        /// <returns>SELECT clause</returns>
        public SqlExpression OrderBy(object firstColumn, params object[] otherColumns)
        {
            var columns = new object[otherColumns.Length + 1];
            columns[0] = firstColumn;
            for (var i = 0; i < otherColumns.Length; i++) columns[i + 1] = otherColumns[i];
            return OrderBy(columns);
        }

        /// <summary>
        /// Appends a select clause using the columns specified with expressions.
        /// </summary>
        /// <typeparam name="TData">Data record type</typeparam>
        /// <param name="firstProp">First property selector</param>
        /// <param name="otherProps">Other property selectors</param>
        /// <returns>SELECT clause</returns>
        public SqlExpression OrderBy<TData>(Expression<Func<TData, object>> firstProp,
                                              params Expression<Func<TData, object>>[] otherProps)
        {
            var paramNames = new List<string> { GetPropertyName(firstProp) };
            paramNames.AddRange(otherProps.Select(GetPropertyName));
            return OrderBy(paramNames);
        }

        /// <summary>
        /// Completes the specified SQL statement with the required SELECT and FROM clauses.
        /// </summary>
        /// <typeparam name="TData">Type of the data record</typeparam>
        /// <returns>The complete select statement</returns>
        public SqlExpression CompleteSelect<TData>()
        {
            return Is(this, "select") || Is(this, "execute") ? this : New.Select<TData>().From<TData>().Append(this);
        }

        #region Helper methods

        /// <summary>
        /// Checks if the specified Sql instance is the given clause
        /// </summary>
        /// <param name="sql">Sql instance</param>
        /// <param name="sqltype">SQL clause (e.g. WHERE, ORDER BY, etc.)</param>
        /// <returns>True, if this instance is the specified clause; otherwise, false.</returns>
        private static bool Is(SqlExpression sql, string sqltype)
        {
            return sql?._innerSql != null 
                && sql._innerSql.StartsWith(sqltype, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Builds the SQL expression
        /// </summary>
        private void BuildExpression()
        {
            // --- Back, if the expression is already built
            if (_sqlBuilt != null) return;

            // --- Build this expression
            var sb = new StringBuilder();
            var args = new List<object>();
            BuildExpression(sb, args, null);

            // --- Set the built parts
            _sqlBuilt = sb.ToString();
            _argsBuilt = args.ToArray();
        }

        /// <summary>
        /// Builds the SQL epxression 
        /// </summary>
        /// <param name="sb">String builder</param>
        /// <param name="args">Arguments processed</param>
        /// <param name="prevExpression">Previous SQL expression</param>
        void BuildExpression(StringBuilder sb, List<object> args, SqlExpression prevExpression)
        {
            const string WHERE = "where ";
            const string AND = "and ";
            const string ORDER_BY = "order by ";

            if (!string.IsNullOrEmpty(_innerSql))
            {
                if (sb.Length > 0) sb.Append("\n");

                var sql = ProcessParameters(_innerSql, _innerArgs, args);

                if (Is(prevExpression, WHERE) && Is(this, WHERE))
                    sql = AND + sql.Substring(WHERE.Length);
                if (Is(prevExpression, ORDER_BY) && Is(this, ORDER_BY))
                    sql = SEPARATOR + sql.Substring(ORDER_BY.Length);
                sb.Append(sql);
            }

            _nextExpression?.BuildExpression(sb, args, this);
        }

        /// <summary>
        /// Processes the specified named parameters
        /// </summary>
        /// <param name="sql">SQL statement with parameter placeholders</param>
        /// <param name="argsSrc">Array of source parameters</param>
        /// <param name="argsDest">List of destination arguments</param>
        /// <returns>SQL statement with processed parameters</returns>
        public static string ProcessParameters(string sql, object[] argsSrc, List<object> argsDest)
        {
            return s_RxParams.Replace(sql,
                m =>
                {
                    // --- Obtain the parameter name/index
                    var param = m.Value.Substring(1);
                    int paramIndex;

                    // --- Obtain the parameter value
                    var argVal = int.TryParse(param, out paramIndex)
                                     ? ExtractParameterByIndex(sql, argsSrc, paramIndex)
                                     : ExtractParameterByName(sql, argsSrc, param);

                    // --- Expand collections to parameter lists
                    var parameterList = ExpandParameterList(argsDest, argVal);
                    if (parameterList != null) return parameterList;

                    // --- In case of a single parameter check for duplication
                    var indexOfValue = argsDest.IndexOf(argVal);

                    // --- Duplication
                    if (indexOfValue >= 0) return "@" + indexOfValue;

                    // --- Add the new parameter value to the parameter list
                    argsDest.Add(argVal);
                    return "@" + (argsDest.Count - 1).ToString(CultureInfo.InvariantCulture);
                });
        }

        /// <summary>
        /// Expands the parameter list provided the value is a real list.
        /// </summary>
        /// <param name="argsDest">Destination argument list</param>
        /// <param name="argVal">Argument value to expand</param>
        /// <returns>String with argument list, if a list is provided; otherwise, null</returns>
        private static string ExpandParameterList(IList<object> argsDest, object argVal)
        {
            if ((argVal as IEnumerable) != null && (argVal as string) == null && (argVal as byte[]) == null)
            {
                var sb = new StringBuilder();
                foreach (var i in (IEnumerable) argVal)
                {
                    var indexOfExistingValue = argsDest.IndexOf(i);
                    if (indexOfExistingValue >= 0)
                    {
                        sb.Append((sb.Length == 0 ? "@" : ", @") + indexOfExistingValue);
                    }
                    else
                    {
                        sb.Append((sb.Length == 0 ? "@" : ", @") + argsDest.Count);
                        argsDest.Add(i);
                    }
                }
                if (sb.Length == 0)
                {
                    sb.Append("select 1 where 1 = 0");
                }
                return sb.ToString();
            }
            return null;
        }

        /// <summary>
        /// Extract parameter by its index.
        /// </summary>
        /// <param name="sql">SQL string</param>
        /// <param name="argsSrc">Argument array</param>
        /// <param name="paramIndex">Parameter index</param>
        /// <returns>The extracted parameter value</returns>
        private static object ExtractParameterByIndex(string sql, IList<object> argsSrc, int paramIndex)
        {
            if (paramIndex >= argsSrc.Count)
            {
                throw new ArgumentOutOfRangeException(
                    $"Parameter '@{paramIndex}' specified but only {argsSrc.Count} parameters supplied (in '{sql}')");
            }
            return argsSrc[paramIndex];
        }

        /// <summary>
        /// Extract parameter by its name
        /// </summary>
        /// <param name="sql">SQL string</param>
        /// <param name="argsSrc">Argument array</param>
        /// <param name="param">Argument name</param>
        /// <returns></returns>
        private static object ExtractParameterByName(string sql, IEnumerable<object> argsSrc, string param)
        {
            // --- Look for a property on one of the arguments with this name
            var found = false;
            object argVal = null;
            foreach (var obj in argsSrc)
            {
                var propInfo = obj.GetType().GetProperty(param);
                if (propInfo == null) continue;
                argVal = propInfo.GetValue(obj, null);
                found = true;
                break;
            }
            if (!found)
            {
                throw new ArgumentException(
                    $"Parameter '@{param}' specified but none of the passed arguments have a property with " +
                    $"this name (in '{sql}')");
            }
            return argVal;
        }

        /// <summary>
        /// Gets the name of a data record property specified by an expression.
        /// </summary>
        /// <typeparam name="TClass">Data record type</typeparam>
        /// <param name="propertySelector">Property selector expression</param>
        /// <returns>Property name</returns>
        [ExcludeFromCodeCoverage]
        public static string GetPropertyName<TClass>(Expression<Func<TClass, object>> propertySelector)
        {
            if (propertySelector == null) throw new ArgumentNullException(nameof(propertySelector));

            MemberExpression memberExpr;
            var unaryExpr = propertySelector.Body as UnaryExpression;
            if (unaryExpr != null)
            {
                memberExpr = unaryExpr.Operand as MemberExpression;
            }
            else
            {
                memberExpr = propertySelector.Body as MemberExpression;
            }
            if (memberExpr != null)
            {
                // --- Member expression is used
                var propertyInfo = memberExpr.Member as PropertyInfo;
                if (propertyInfo == null)
                    throw new ArgumentException("The argument does not references a valid Property!",
                        nameof(propertySelector));

                if (propertyInfo.GetGetMethod(true).IsStatic)
                    throw new ArgumentException("The argument is not an instance property", nameof(propertySelector));
                var colNameAttr = propertyInfo.GetCustomAttribute<ColumnNameAttribute>();
                return colNameAttr == null ? memberExpr.Member.Name : colNameAttr.Value;
            }
            throw new ArgumentException("The argument is an invalid expression", nameof(propertySelector));
        }

        #endregion
    }
}