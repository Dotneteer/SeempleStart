using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Seemplest.Core.Common;

namespace Seemplest.Core.ServiceObjects.Validation
{
    /// <summary>
    /// This class provides a number of validation methods that can be used
    /// to validate arguments of business operations
    /// </summary>
    public sealed class Validator
    {
        private NotificationList _notifications;

        public Validator()
        {
            Reset();
        }

        /// <summary>
        /// Resets this object by removing all validation notifications
        /// </summary>
        public void Reset()
        {
            _notifications = new NotificationList();
        }

        /// <summary>
        /// Gets the list of notification items belonging to this validator
        /// </summary>
        public IReadOnlyList<NotificationItem> Notifications
        {
            get { return _notifications.Items; }
        }

        /// <summary>
        /// Gets the flag indicating if the list contains an item with error
        /// </summary>
        public bool HasError
        {
            get { return _notifications.HasError; }
        }

        /// <summary>
        /// Gest the flag indicating if the notification list is empty
        /// </summary>
        public bool IsEmpty
        {
            get { return _notifications.Count == 0; }
        }

        /// <summary>
        /// Raises an ArgumentValidationException if validation fails.
        /// </summary>
        public void RaiseWhenFailed()
        {
            if (!HasError) return;
            var ex = new ArgumentValidationException();
            ex.SetNotifications(_notifications);
            throw ex;
        }

        /// <summary>
        /// Raises an ArgumentValidationException if validation has any issues.
        /// </summary>
        public void RaiseIfHasIssues()
        {
            if (IsEmpty) return;
            var ex = new ArgumentValidationException();
            ex.SetNotifications(_notifications);
            throw ex;
        }

        /// <summary>
        /// Checks if the specified validation condition is satisfied
        /// </summary>
        /// <param name="condition">Condition to check</param>
        /// <param name="target">Target entity/property name</param>
        /// <param name="code">Validation error code</param>
        /// <param name="targetType">Validation target type</param>
        /// <param name="attributes">Attributes related to the issue</param>
        /// <param name="exception">Exception instance</param>
        /// <param name="handleAsError">True, if this issue should be handled as an error; otherwise, false</param>
        /// <returns>This object itself, for fluent interface purpose</returns>
        public Validator Require(bool condition, string target, string code = ValidationCodes.VALIDATION_FAILS,
            NotificationTargetType targetType = NotificationTargetType.Property,
            IList<object> attributes = null, Exception exception = null,
            bool handleAsError = true)
        {
            if (!condition)
            {
                _notifications.Add(new NotificationItem(
                    handleAsError ? NotificationType.Error : NotificationType.Warning,
                    code,
                    target,
                    targetType,
                    attributes,
                    exception));
            }
            return this;
        }

        /// <summary>
        /// Checks if the specified validation condition is satisfied
        /// </summary>
        /// <param name="condition">Condition to check</param>
        /// <param name="selector">Property selector</param>
        /// <param name="code">Validation error code</param>
        /// <param name="attributes">Attributes related to the issue</param>
        /// <param name="exception">Exception instance</param>
        /// <param name="handleAsError">True, if this issue should be handled as an error; otherwise, false</param>
        /// <returns>This object itself, for fluent interface purpose</returns>
        public Validator Require<TClass, T>(bool condition, Expression<Func<TClass, T>> selector, 
            string code = ValidationCodes.VALIDATION_FAILS,
            IList<object> attributes = null, Exception exception = null,
            bool handleAsError = true)
        {
            if (!condition)
            {
                _notifications.Add(new NotificationItem(
                    handleAsError ? NotificationType.Error : NotificationType.Warning,
                    code,
                    ExpressionHelper.GetPropertyName(selector),
                    NotificationTargetType.Property,
                    attributes,
                    exception));
            }
            return this;
        }

        /// <summary>
        /// Checks if the specified value is not null
        /// </summary>
        /// <param name="entity">Entity to check</param>
        /// <param name="target">Target entity/property name</param>
        /// <param name="code">Validation error code</param>
        /// <param name="targetType">Validation target type</param>
        /// <param name="handleAsError">True, if this issue should be handled as an error; otherwise, false</param>
        /// <returns>This object itself, for fluent interface purpose</returns>
        public Validator NotNull<TEnt>(TEnt entity, string target, string code = ValidationCodes.NULL_NOT_ALLOWED,
            NotificationTargetType targetType = NotificationTargetType.Entity,
            bool handleAsError = true)
            where TEnt: class
        {
            return Require(entity != null, target, code, targetType, handleAsError: handleAsError);
        }

        /// <summary>
        /// Checks if the specified value is not null
        /// </summary>
        /// <param name="entity">Entity to check</param>
        /// <param name="selector">Property selector</param>
        /// <param name="code">Validation error code</param>
        /// <param name="handleAsError">True, if this issue should be handled as an error; otherwise, false</param>
        /// <returns>This object itself, for fluent interface purpose</returns>
        public Validator NotNull<TEnt, TClass, T>(TEnt entity, Expression<Func<TClass, T>> selector, 
            string code = ValidationCodes.NULL_NOT_ALLOWED,
            bool handleAsError = true)
            where TEnt : class
        {
            return Require(entity != null, selector, code, handleAsError: handleAsError);
        }

        /// <summary>
        /// Checks if the specified string value is not null or empty
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="target">Target entity/property name</param>
        /// <param name="code">Validation error code</param>
        /// <param name="targetType">Validation target type</param>
        /// <param name="handleAsError">True, if this issue should be handled as an error; otherwise, false</param>
        /// <returns>This object itself, for fluent interface purpose</returns>
        public Validator NotNullOrEmpty(string value, string target,
            string code = ValidationCodes.NULL_NOT_ALLOWED,
            NotificationTargetType targetType = NotificationTargetType.Property,
            bool handleAsError = true)
        {
            return Require(!String.IsNullOrWhiteSpace(value), target, code, targetType,
                handleAsError: handleAsError);
        }

        /// <summary>
        /// Checks if the specified string value is not null or empty
        /// </summary>
        /// <param name="date">Date to check</param>
        /// <param name="target">Target entity/property name</param>
        /// <param name="code">Validation error code</param>
        /// <param name="targetType">Validation target type</param>
        /// <param name="handleAsError">True, if this issue should be handled as an error; otherwise, false</param>
        /// <returns>This object itself, for fluent interface purpose</returns>
        public Validator IsFutureDate(DateTime date, string target,
            string code = ValidationCodes.FUTURE_DATE,
            NotificationTargetType targetType = NotificationTargetType.Property,
            bool handleAsError = true)
        {
            return Require(date.ToLocalTime() > DateTime.Now, target, code, targetType,
                handleAsError: handleAsError);
        }

        /// <summary>
        /// Checks if the specified GUID value is not empty
        /// </summary>
        /// <param name="guid">Date to check</param>
        /// <param name="target">Target entity/property name</param>
        /// <param name="code">Validation error code</param>
        /// <param name="targetType">Validation target type</param>
        /// <param name="handleAsError">True, if this issue should be handled as an error; otherwise, false</param>
        /// <returns>This object itself, for fluent interface purpose</returns>
        public Validator NotEmpty(Guid guid, string target,
            string code = ValidationCodes.EMPTY_GUID,
            NotificationTargetType targetType = NotificationTargetType.Property,
            bool handleAsError = true)
        {
            return Require(guid != Guid.Empty, target, code, targetType,
                handleAsError: handleAsError);
        }

        /// <summary>
        /// Checks if the specified GUID value is not empty
        /// </summary>
        /// <param name="guid">Date to check</param>
        /// <param name="target">Target entity/property name</param>
        /// <param name="code">Validation error code</param>
        /// <param name="targetType">Validation target type</param>
        /// <param name="handleAsError">True, if this issue should be handled as an error; otherwise, false</param>
        /// <returns>This object itself, for fluent interface purpose</returns>
        public Validator NotNullOrEmpty(Guid? guid, string target,
            string code = ValidationCodes.EMPTY_GUID,
            NotificationTargetType targetType = NotificationTargetType.Property,
            bool handleAsError = true)
        {
            return Require(guid.HasValue && guid.Value != Guid.Empty, target, code, targetType,
                handleAsError: handleAsError);
        }

        /// <summary>
        /// Checks if the specified integer value is within the specified range
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="lowerValue">Lower bound of the range</param>
        /// <param name="upperValue">Upper bound of the range</param>
        /// <param name="target">Target entity/property name</param>
        /// <param name="code">Validation error code</param>
        /// <param name="targetType">Validation target type</param>
        /// <param name="handleAsError">True, if this issue should be handled as an error; otherwise, false</param>
        /// <returns>This object itself, for fluent interface purpose</returns>
        public Validator IsInRange(int value, int lowerValue, int upperValue, string target,
            string code = ValidationCodes.OUT_OF_RANGE,
            NotificationTargetType targetType = NotificationTargetType.Property,
            bool handleAsError = true)
        {
            return Require(lowerValue <= value && value <= upperValue, target, code, targetType,
                handleAsError: handleAsError);
        }

        /// <summary>
        /// Checks if the specified long integer value is within the specified range
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="lowerValue">Lower bound of the range</param>
        /// <param name="upperValue">Upper bound of the range</param>
        /// <param name="target">Target entity/property name</param>
        /// <param name="code">Validation error code</param>
        /// <param name="targetType">Validation target type</param>
        /// <param name="handleAsError">True, if this issue should be handled as an error; otherwise, false</param>
        /// <returns>This object itself, for fluent interface purpose</returns>
        public Validator IsInRange(long value, long lowerValue, long upperValue, string target,
            string code = ValidationCodes.OUT_OF_RANGE,
            NotificationTargetType targetType = NotificationTargetType.Property,
            bool handleAsError = true)
        {
            return Require(lowerValue <= value && value <= upperValue, target, code, targetType,
                handleAsError: handleAsError);
        }

        /// <summary>
        /// Checks if the specified decimal value is within the specified range
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="lowerValue">Lower bound of the range</param>
        /// <param name="upperValue">Upper bound of the range</param>
        /// <param name="target">Target entity/property name</param>
        /// <param name="code">Validation error code</param>
        /// <param name="targetType">Validation target type</param>
        /// <param name="handleAsError">True, if this issue should be handled as an error; otherwise, false</param>
        /// <returns>This object itself, for fluent interface purpose</returns>
        public Validator IsInRange(decimal value, decimal lowerValue, decimal upperValue, string target,
            string code = ValidationCodes.OUT_OF_RANGE,
            NotificationTargetType targetType = NotificationTargetType.Property,
            bool handleAsError = true)
        {
            return Require(lowerValue <= value && value <= upperValue, target, code, targetType,
                handleAsError: handleAsError);
        }

        /// <summary>
        /// Checks if the specified double value is within the specified range
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="lowerValue">Lower bound of the range</param>
        /// <param name="upperValue">Upper bound of the range</param>
        /// <param name="target">Target entity/property name</param>
        /// <param name="code">Validation error code</param>
        /// <param name="targetType">Validation target type</param>
        /// <param name="handleAsError">True, if this issue should be handled as an error; otherwise, false</param>
        /// <returns>This object itself, for fluent interface purpose</returns>
        public Validator IsInRange(double value, double lowerValue, double upperValue, string target,
            string code = ValidationCodes.OUT_OF_RANGE,
            NotificationTargetType targetType = NotificationTargetType.Property,
            bool handleAsError = true)
        {
            return Require(lowerValue <= value && value <= upperValue, target, code, targetType,
                handleAsError: handleAsError);
        }

        /// <summary>
        /// Checks if the specified value is in the specified list of values
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="list"></param>
        /// <param name="target">Target entity/property name</param>
        /// <param name="code">Validation error code</param>
        /// <param name="targetType">Validation target type</param>
        /// <param name="handleAsError">True, if this issue should be handled as an error; otherwise, false</param>
        /// <returns>This object itself, for fluent interface purpose</returns>
        public Validator IsInList<T>(T value, IList<T> list, string target,
            string code = ValidationCodes.NOT_IN_LIST,
            NotificationTargetType targetType = NotificationTargetType.Property,
            bool handleAsError = true)
            where T : IComparable<T>
        {
            return Require(list.Contains(value), target, code, targetType,
                handleAsError: handleAsError);
        }

        /// <summary>
        /// Checks if the specified values mathces with the provided regular expression
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="regexp">Regular expression to check</param>
        /// <param name="target">Target entity/property name</param>
        /// <param name="code">Validation error code</param>
        /// <param name="targetType">Validation target type</param>
        /// <param name="handleAsError">True, if this issue should be handled as an error; otherwise, false</param>
        /// <returns>This object itself, for fluent interface purpose</returns>
        public Validator Matches(string value, string regexp, string target,
            string code = ValidationCodes.NO_MATCH, NotificationTargetType targetType = NotificationTargetType.Property,
            bool handleAsError = true)
        {
            return Require(value == null || Regex.IsMatch(value, regexp), target, code, targetType,
                handleAsError: handleAsError);
        }

        /// <summary>
        /// Checks if the specified values mathces with the provided regular expression
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="regexp">Regular expression to check</param>
        /// <param name="target">Target entity/property name</param>
        /// <param name="code">Validation error code</param>
        /// <param name="targetType">Validation target type</param>
        /// <param name="handleAsError">True, if this issue should be handled as an error; otherwise, false</param>
        /// <returns>This object itself, for fluent interface purpose</returns>
        public Validator Matches(string value, Regex regexp, string target,
            string code = ValidationCodes.NO_MATCH, NotificationTargetType targetType = NotificationTargetType.Property,
            bool handleAsError = true)
        {
            return Require(value == null || regexp.IsMatch(value), target, code, targetType,
                handleAsError: handleAsError);
        }

        /// <summary>
        /// Checks if the specified string is not longer than specified.
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="maxLength">Maximum length of the string</param>
        /// <param name="target">Target entity/property name</param>
        /// <param name="code">Validation error code</param>
        /// <param name="targetType">Validation target type</param>
        /// <param name="handleAsError">True, if this issue should be handled as an error; otherwise, false</param>
        /// <returns>This object itself, for fluent interface purpose</returns>
        public Validator MaxLength(string value, int maxLength, string target,
            string code = ValidationCodes.TOO_LONG, NotificationTargetType targetType = NotificationTargetType.Property,
            bool handleAsError = true)
        {
            return Require(String.IsNullOrEmpty(value) || value.Length <= maxLength, target, code, targetType,
                handleAsError: handleAsError);
        }
    }
}