using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Younderwater.Webclient.Models.UserManagement
{
    /// <summary>
    /// Configure the application sign-in manager which is used in this application.   
    /// </summary>
    public class AppSignInManager : SignInManager<AppUser, string>
    {
        public AppSignInManager(UserManager<AppUser, string> userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager) { }

        /// <summary>
        /// Creates a ClaimsIdentity for the specified user
        /// </summary>
        /// <param name="user">User to create identity for</param>
        /// <returns>The claims that identify the user</returns>
        public override Task<ClaimsIdentity> CreateUserIdentityAsync(AppUser user)
        {
            return user.GenerateUserIdentityAsync((AppUserManager)UserManager);
        }

        /// <summary>
        /// Creates an object of this class for the specified Owin context
        /// </summary>
        /// <param name="options">Create options</param>
        /// <param name="context">Owin context</param>
        /// <returns>Application sign in manager object</returns>
        public static AppSignInManager Create(IdentityFactoryOptions<AppSignInManager> options, IOwinContext context)
        {
            return new AppSignInManager(context.GetUserManager<AppUserManager>(), context.Authentication);
        }
    }
}