namespace SeemplestCloud.Dto
{
    /// <summary>
    /// This class summarises SeemplesCloud related error codes
    /// </summary>
    public class ScErrorCodes
    {
        public const string EMAIL_RESERVED = "EmailReserved";
        public const string EMAIL_ALREADY_INVITED = "EmailAlreadyInvited";
        public const string USER_NAME_RESERVED = "UserNameReserved";
        public const string USER_NAME_ALREADY_INVITED = "UserNameAlreadyInvited";
        public const string INVALID_INVITATION_CODE = "InvalidInvitationCode";
        public const string UNKNOWN_EMAIL = "UnknownEmail";
        public const string UNKNOWN_USER_ID = "UnknownUserId";
        public const string UNKNOWN_INVITATION_ID = "UnknownInvitationId";
    }
}