namespace SeemplestBlocks.Dto.Security
{
    /// <summary>
    /// This class describes a user account
    /// </summary>
    public class UserAccountDto
    {
        /// <summary>
        /// User identifier
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Identifies the provider
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// Account identifier for the provider
        /// </summary>
        public string ProviderData { get; set; }
    }
}