using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Seemplest.Core.Common
{
    /// <summary>
    /// This static class provides a set of helper functions using expression trees and selectors.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ExpressionHelper
    {
        /// <summary>
        /// Gets the name of the variable provided by the selector expression.
        /// </summary>
        /// <typeparam name="T">Selector expression type</typeparam>
        /// <param name="selector">Selector instance</param>
        /// <returns>Name of the variable the selector points to</returns>
        public static string GetMemberName<T>(Expression<Func<T>> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");
            var memberExpression = selector.Body as MemberExpression;
            if (memberExpression != null)
            {
                return memberExpression.Member.Name;
            }
            var methodCallExpression = selector.Body as MethodCallExpression;
            if (methodCallExpression != null)
            {
                return methodCallExpression.Method.Name;
            }
            throw GetInvalidMemberOrMethodCallException("selector");
        }

        /// <summary>
        /// Gets the name of a member from the specified selector expression.
        /// </summary>
        /// <typeparam name="TClass">Type holding the member</typeparam>
        /// <typeparam name="T">Member type</typeparam>
        /// <param name="selector">Selector instance</param>
        /// <returns>Name of the member the selector points to</returns>
        public static string GetMemberName<TClass, T>(Expression<Func<TClass, T>> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");
            var memberExpression = selector.Body as MemberExpression;
            if (memberExpression != null)
            {
                return memberExpression.Member.Name;
            }
            var methodCallExpression = selector.Body as MethodCallExpression;
            if (methodCallExpression != null)
            {
                return methodCallExpression.Method.Name;
            }
            throw GetInvalidMemberOrMethodCallException("selector");
        }

        /// <summary>
        /// Gets the full name of the member specified by the selector expression.
        /// </summary>
        /// <typeparam name="TClass">Type holding the member</typeparam>
        /// <param name="selector">Selector instance</param>
        /// <returns>Name of the member the selector points to</returns>
        public static string GetMemberFullName<TClass>(Expression<Func<TClass, object>> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");
            var expressionName = selector.Body.ToString();
            if (selector.Parameters.Any())
            {
                var param = selector.Parameters[0];
                var paramName = param.Name;
                var typeName = typeof(TClass).Name;
                if (expressionName.StartsWith(paramName))
                    return expressionName.Replace(paramName, typeName);
            }
            return expressionName;
        }

        /// <summary>
        /// Gets the full name of the member specified by the selector expression.
        /// </summary>
        /// <typeparam name="TClass">Type holding the member</typeparam>
        /// <typeparam name="T">Member type</typeparam>
        /// <param name="selector">Selector instance</param>
        /// <returns>Name of the member the selector points to</returns>
        public static string GetMemberFullName<TClass, T>(Expression<Func<TClass, T>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            var expressionName = selector.Body.ToString();
            if (selector.Parameters.Any())
            {
                var param = selector.Parameters[0];
                var paramName = string.Format("{0}.", param.Name);
                if (expressionName.StartsWith(paramName))
                    return expressionName.Substring(paramName.Length);
            }
            return expressionName;
        }

        /// <summary>
        /// Gets the name of the property specified by the selector expression.
        /// </summary>
        /// <typeparam name="TClass">Type holding the property</typeparam>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="selector">Selector instance</param>
        /// <returns>Name of the property the selector points to</returns>
        public static string GetPropertyName<TClass, T>(Expression<Func<TClass, T>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            var memberExpression = selector.Body as MemberExpression;
            if (memberExpression == null)
                throw GetInvalidMemberExpressionException("selector");

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw GetInvalidPropertyReferenceExpressionException("selector");

            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Gets the name of the property specified by the selector expression.
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="selector">Selector instance</param>
        /// <returns>Name of the property the selector points to</returns>
        public static string GetPropertyName<T>(Expression<Func<T>> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");
            var memberExpression = selector.Body as MemberExpression;
            if (memberExpression == null) throw GetInvalidMemberExpressionException("selector");
            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null) throw GetInvalidPropertyReferenceExpressionException("selector");
            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Gets the name of the field specified by the selector expression.
        /// </summary>
        /// <typeparam name="TClass">Type holding the field</typeparam>
        /// <typeparam name="T">Field type</typeparam>
        /// <param name="selector">Selector instance</param>
        /// <returns>Name of the field the selector points to</returns>
        public static string GetFieldName<TClass, T>(Expression<Func<TClass, T>> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");
            var memberExpression = selector.Body as MemberExpression;
            if (memberExpression == null) throw GetInvalidMemberExpressionException("selector");
            var fieldInfo = memberExpression.Member as FieldInfo;
            if (fieldInfo == null) throw GetInvalidFieldReferenceExpressionException("selector");
            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Gets the name of the method specified by the selector expression.
        /// </summary>
        /// <typeparam name="TClass">Type holding the property</typeparam>
        /// <typeparam name="T">Method type</typeparam>
        /// <param name="selector">Selector instance</param>
        /// <returns>Name of the method the selector points to</returns>
        public static string GetMethodName<TClass, T>(Expression<Func<TClass, T>> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");
            var methodCallExpression = selector.Body as MethodCallExpression;
            if (methodCallExpression == null) throw GetInvalidMethodCallExpressionExpressionException("selector");
            return methodCallExpression.Method.Name;
        }

        /// <summary>
        /// Gets the value of the member specified by the member function.
        /// </summary>
        /// <typeparam name="TClass">Type holding the property</typeparam>
        /// <typeparam name="T">Member type</typeparam>
        /// <param name="obj">Object holding the member</param>
        /// <param name="getMemberFunc">Member function getter</param>
        /// <returns>Value of the member the getter function points to</returns>
        public static T GetMemberValue<TClass, T>(TClass obj, Func<TClass, T> getMemberFunc)
        {
            if (getMemberFunc == null) throw new ArgumentNullException("getMemberFunc");
            return getMemberFunc(obj);
        }

        /// <summary>
        /// Gets the value of the member specified by the selector.
        /// </summary>
        /// <typeparam name="TClass">Type holding the member</typeparam>
        /// <typeparam name="T">Member type</typeparam>
        /// <param name="obj">Object holding the member</param>
        /// <param name="selector">Selector instance</param>
        /// <returns>Value of the member the selector points to</returns>
        public static T GetMemberValue<TClass, T>(TClass obj, Expression<Func<TClass, T>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            return InvokeExpression<TClass, T>(obj, selector.Body, selector.Parameters);
        }

        /// <summary>
        /// Gets the value of the property specified by the selector.
        /// </summary>
        /// <typeparam name="TClass">Type holding the property</typeparam>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="obj">Object holding the member</param>
        /// <param name="selector">Selector instance</param>
        /// <returns>Value of the property the selector points to</returns>
        public static T GetPropertyValue<TClass, T>(TClass obj, Expression<Func<TClass, T>> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");
            var memberExpression = selector.Body as MemberExpression;
            if (memberExpression == null) throw GetInvalidMemberExpressionException("selector");
            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null) throw GetInvalidPropertyReferenceExpressionException("selector");
            return InvokeExpression<TClass, T>(obj, memberExpression, selector.Parameters);
        }

        /// <summary>
        /// Gets the value of the field specified by the selector.
        /// </summary>
        /// <typeparam name="TClass">Type holding the field</typeparam>
        /// <typeparam name="T">Field type</typeparam>
        /// <param name="obj">Object holding the member</param>
        /// <param name="selector">Selector instance</param>
        /// <returns>Value of the field the selector points to</returns>
        public static T GetFieldValue<TClass, T>(TClass obj, Expression<Func<TClass, T>> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");
            var memberExpression = selector.Body as MemberExpression;
            if (memberExpression == null) throw GetInvalidMemberExpressionException("selector");
            var fieldInfo = memberExpression.Member as FieldInfo;
            if (fieldInfo == null) throw GetInvalidFieldReferenceExpressionException("selector");
            return InvokeExpression<TClass, T>(obj, memberExpression, selector.Parameters);
        }

        /// <summary>
        /// Gets the value of the method specified by the selector.
        /// </summary>
        /// <typeparam name="TClass">Type holding the method</typeparam>
        /// <typeparam name="T">Member type</typeparam>
        /// <param name="obj">Object holding the method</param>
        /// <param name="selector">Selector instance</param>
        /// <returns>Value of the method the selector points to</returns>
        public static T GetMethodValue<TClass, T>(TClass obj, Expression<Func<TClass, T>> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");
            var methodCallExpression = selector.Body as MethodCallExpression;
            if (methodCallExpression == null) throw GetInvalidMemberExpressionException("selector");
            return InvokeExpression<TClass, T>(obj, methodCallExpression, selector.Parameters);
        }

        /// <summary>
        /// Gets the value of the member specified by the selector using the specified default value.
        /// </summary>
        /// <typeparam name="TClass">Type holding the property</typeparam>
        /// <typeparam name="T">Member type</typeparam>
        /// <param name="obj">Object holding the member</param>
        /// <param name="selector">Selector instance</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Value of the property the selector points to</returns>
        public static T GetMemberValueOrDefault<TClass, T>(TClass obj, Expression<Func<TClass, T>> selector,
            T defaultValue = default(T))
        {
            if (Equals(obj, default(TClass))) return defaultValue;
            if (selector == null) throw new ArgumentNullException("selector");
            var expressionParts = new Stack<Expression>();
            var parentExpression = DecomposeExpression(selector.Body, expressionParts);
            if (parentExpression == null) return defaultValue;
            object result = null;
            while (expressionParts.Count > 0)
            {
                var expression = expressionParts.Pop();
                result = InvokeExpression<TClass, object>(obj, expression, selector.Parameters);
                if (result == null) return defaultValue;
            }
            return (T)result;
        }

        /// <summary>
        /// Invokes the specified expression
        /// </summary>
        /// <typeparam name="TClass">Type to invoke the expression on</typeparam>
        /// <typeparam name="T">Expression type</typeparam>
        /// <param name="obj">Object to invoke the expression on</param>
        /// <param name="expression">Expression</param>
        /// <param name="parameters">Expression parameters</param>
        /// <returns></returns>
        private static T InvokeExpression<TClass, T>(TClass obj,
            Expression expression, IEnumerable<ParameterExpression> parameters)
        {
            var memberExpression = expression as MemberExpression;
            // --- for simple member expressions access the given vallue using the filed/propertyInfo instances.
            if ((memberExpression != null) && (memberExpression.Expression == null))
            {
                var fieldInfo = memberExpression.Member as FieldInfo;
                if (fieldInfo != null)
                {
                    return (T)fieldInfo.GetValue(obj);
                }

                var propertyInfo = memberExpression.Member as PropertyInfo;
                if (propertyInfo != null)
                {
                    // note: indexer should be handled!
                    return (T)propertyInfo.GetValue(obj, null);
                }
            }
            var targetExpression = Expression.Lambda(expression, parameters);
            var targetDelegate = targetExpression.Compile();
            var result = (T)targetDelegate.DynamicInvoke(obj);
            return result;
        }

        /// <summary>
        /// Decompose the specified expression.
        /// </summary>
        /// <param name="expression">Original expression</param>
        /// <param name="expressionParts">Expression parts</param>
        /// <returns>Decomposed expression</returns>
        private static Expression DecomposeExpression(Expression expression, Stack<Expression> expressionParts)
        {
            var parentExpression = expression;
            while (parentExpression != null && parentExpression.NodeType != ExpressionType.Parameter)
            {
                expressionParts.Push(parentExpression);
                switch (parentExpression.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        parentExpression = ((MemberExpression)parentExpression).Expression;
                        break;
                    case ExpressionType.Call:
                        parentExpression = ((MethodCallExpression)parentExpression).Object;
                        break;
                    default:
                        parentExpression = null;
                        break;
                }
            }
            return parentExpression;
        }

        /// <summary>
        /// Gets an invalid member expression exception
        /// </summary>
        /// <param name="argumentName">Member name</param>
        /// <returns>Exception instance</returns>
        private static ArgumentException GetInvalidMemberExpressionException(string argumentName)
        {
            return new ArgumentException(@"The argument is not a valid MemberExpression!", argumentName);
        }

        /// <summary>
        /// Gets an invalid property expression exception
        /// </summary>
        /// <param name="argumentName">Member name</param>
        /// <returns>Exception instance</returns>
        private static ArgumentException GetInvalidPropertyReferenceExpressionException(string argumentName)
        {
            return new ArgumentException(@"The argument does not references a property!", argumentName);
        }

        /// <summary>
        /// Gets an invalid field expression exception
        /// </summary>
        /// <param name="argumentName">Member name</param>
        /// <returns>Exception instance</returns>
        private static ArgumentException GetInvalidFieldReferenceExpressionException(string argumentName)
        {
            return new ArgumentException(@"The argument does not references a field!", argumentName);
        }

        /// <summary>
        /// Gets an invalid method call expression exception
        /// </summary>
        /// <param name="argumentName">Member name</param>
        /// <returns>Exception instance</returns>
        private static ArgumentException GetInvalidMethodCallExpressionExpressionException(string argumentName)
        {
            return new ArgumentException(@"The argument is not a valid MethodCallExpression!", argumentName);
        }

        /// <summary>
        /// Gets an invalid member or method call expression exception
        /// </summary>
        /// <param name="argumentName">Member name</param>
        /// <returns>Exception instance</returns>
        private static ArgumentException GetInvalidMemberOrMethodCallException(string argumentName)
        {
            return new ArgumentException(@"The argument is not a valid Member or MethodCallExpression!", argumentName);
        }

        /// <summary>
        /// Gets an invalid constant expression exception
        /// </summary>
        /// <param name="argumentName">Member name</param>
        /// <returns>Exception instance</returns>
        // ReSharper disable UnusedMember.Local
        private static ArgumentException GetInvalidConstantException(string argumentName)
        // ReSharper restore UnusedMember.Local
        {
            return new ArgumentException(@"The argument is not a valid ConstantExpression!", argumentName);
        }
    }

    /// <summary>
    /// Generic wrapper for ExpressionHelper methods.
    /// </summary>
    /// <typeparam name="TClass">Type holding the members</typeparam>
    public static class ObjectSupport<TClass>
    {
        /// <summary>
        /// Gets the name of a member from the specified selector expression.
        /// </summary>
        /// <typeparam name="T">Member type</typeparam>
        /// <param name="selector">Selector instance</param>
        /// <returns>Name of the member the selector points to</returns>
        public static string GetMemberName<T>(Expression<Func<TClass, T>> selector)
        {
            return ExpressionHelper.GetMemberName(selector);
        }

        /// <summary>
        /// Gets the full name of a member from the specified selector expression.
        /// </summary>
        /// <typeparam name="T">Member type</typeparam>
        /// <param name="selector">Selector instance</param>
        /// <returns>Name of the member the selector points to</returns>
        public static string GetMemberFullName<T>(Expression<Func<TClass, T>> selector)
        {
            return ExpressionHelper.GetMemberFullName(selector);
        }

        /// <summary>
        /// Gets the name of a property from the specified selector expression.
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="selector">Selector instance</param>
        /// <returns>Name of the property the selector points to</returns>
        public static string GetPropertyName<T>(Expression<Func<TClass, T>> selector)
        {
            return ExpressionHelper.GetPropertyName(selector);
        }

        /// <summary>
        /// Gets the name of a field from the specified selector expression.
        /// </summary>
        /// <typeparam name="T">Field type</typeparam>
        /// <param name="selector">Selector instance</param>
        /// <returns>Name of the field the selector points to</returns>
        public static string GetFieldName<T>(Expression<Func<TClass, T>> selector)
        {
            return ExpressionHelper.GetFieldName(selector);
        }

        /// <summary>
        /// Gets the name of a method from the specified selector expression.
        /// </summary>
        /// <typeparam name="T">Method type</typeparam>
        /// <param name="selector">Selector instance</param>
        /// <returns>Name of the method the selector points to</returns>
        public static string GetMethodName<T>(Expression<Func<TClass, T>> selector)
        {
            return ExpressionHelper.GetMethodName(selector);
        }

        /// <summary>
        /// Gets the name of a member from the specified selector expression.
        /// </summary>
        /// <typeparam name="T">Member type</typeparam>
        /// <param name="obj">Instance to use the selector on</param>
        /// <param name="getMemberFunc">Member getter function</param>
        /// <returns>Name of the member the selector points to</returns>
        public static T GetMemberValue<T>(TClass obj, Func<TClass, T> getMemberFunc)
        {
            return ExpressionHelper.GetMemberValue(obj, getMemberFunc);
        }

        /// <summary>
        /// Gets the value of a member from the specified selector expression.
        /// </summary>
        /// <typeparam name="T">Member type</typeparam>
        /// <param name="obj">Instance to use the selector on</param>
        /// <param name="selector">Selector instance</param>
        /// <returns>Value of the member the selector points to</returns>
        public static T GetMemberValue<T>(TClass obj, Expression<Func<TClass, T>> selector)
        {
            return ExpressionHelper.GetMemberValue(obj, selector);
        }

        /// <summary>
        /// Gets the value of a property from the specified selector expression.
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="obj">Instance to use the selector on</param>
        /// <param name="selector">Selector instance</param>
        /// <returns>Value of the property the selector points to</returns>
        public static T GetPropertyValue<T>(TClass obj, Expression<Func<TClass, T>> selector)
        {
            return ExpressionHelper.GetPropertyValue(obj, selector);
        }

        /// <summary>
        /// Gets the value of a field from the specified selector expression.
        /// </summary>
        /// <typeparam name="T">Field type</typeparam>
        /// <param name="obj">Instance to use the selector on</param>
        /// <param name="selector">Selector instance</param>
        /// <returns>Value of the field the selector points to</returns>
        public static T GetFieldValue<T>(TClass obj, Expression<Func<TClass, T>> selector)
        {
            return ExpressionHelper.GetFieldValue(obj, selector);
        }

        /// <summary>
        /// Gets the value of a method from the specified selector expression.
        /// </summary>
        /// <typeparam name="T">Method type</typeparam>
        /// <param name="obj">Instance to use the selector on</param>
        /// <param name="selector">Selector instance</param>
        /// <returns>Value of the method the selector points to</returns>
        public static T GetMethodValue<T>(TClass obj, Expression<Func<TClass, T>> selector)
        {
            return ExpressionHelper.GetMethodValue(obj, selector);
        }

        /// <summary>
        /// Gets the value of a member from the specified selector expression with a default value.
        /// </summary>
        /// <typeparam name="T">Member type</typeparam>
        /// <param name="obj">Instance to use the selector on</param>
        /// <param name="selector">Selector instance</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Value of the member the selector points to</returns>
        public static T GetMemberValueOrDefault<T>(TClass obj, Expression<Func<TClass, T>> selector,
            T defaultValue = default(T))
        {
            return ExpressionHelper.GetMemberValueOrDefault(obj, selector, defaultValue);
        }
    }
}