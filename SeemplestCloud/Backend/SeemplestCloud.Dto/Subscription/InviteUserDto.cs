namespace SeemplestCloud.Dto.Subscription
{
    /// <summary>
    /// This DTO describes the information used by the Invite User operation
    /// </summary>
    public class InviteUserDto
    {
        /// <summary>
        /// User name of the invited user
        /// </summary>
        public string InvitedUserName { get; set; }
        
        /// <summary>
        /// Email address of the invited user
        /// </summary>
        public string InvitedEmail { get; set; }
    }
}