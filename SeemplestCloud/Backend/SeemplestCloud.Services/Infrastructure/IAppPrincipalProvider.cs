namespace SeemplestCloud.Services.Infrastructure
{
    /// <summary>
    /// This interface is responsible for providing an AppPrincipal object according the current 
    /// user context
    /// </summary>
    public interface IAppPrincipalProvider
    {
        /// <summary>
        /// Gets the current application principal
        /// </summary>
        AppPrincipal Principal { get; }   
    }
}