namespace Seemplest.Core.ServiceObjects.Validation
{
    /// <summary>
    /// This static class defines the most frequently used validation error codes
    /// </summary>
    public static class ValidationCodes
    {
        public const string VALIDATION_FAILS = "ValidationFails";
        public const string REQUIRED = "Required";
        public const string OUT_OF_RANGE = "OutOfRange";
        public const string NULL_NOT_ALLOWED = "NullNotAllowed";
        public const string FUTURE_DATE = "MustBeFutureDate";
        public const string NOT_IN_LIST = "NotInList";
        public const string EMPTY_GUID = "EmptyGuid";
        public const string NO_MATCH = "NoMatch";
        public const string TOO_LONG = "TooLong";
        public const string INVALID_EMAIL = "InvalidEmail";
    }
}