using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Seemplest.Core.DependencyInjection;
using SeemplestBlocks.Core.Internationalization;
using SeemplestCloud.Dto.Subscription;
using SeemplestCloud.Services.Infrastructure;
using SeemplestCloud.Services.SubscriptionService;
using SeemplestCloud.WebClient.Infrastructure;
using SeemplestCloud.WebClient.Models;
using SeemplestCloud.WebClient.Models.UserManagement;
using Res = SeemplestCloud.Resources.Resources;

namespace SeemplestCloud.WebClient.Controllers
{
    /// <summary>
    /// This controller manages views related to subscription and user account management
    /// </summary>
    [Authorize]
    public class AccountController : LanguageAwareControllerBase
    {
        /// <summary>
        /// The name of the cookie used to store user ID
        /// </summary>

        /// <summary>
        /// UserManager object used to handle users
        /// </summary>
        public AppUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
        }

        /// <summary>
        /// SignInManger object used to handle logon
        /// </summary>
        public AppSignInManager SignInManager
        {
            get { return HttpContext.GetOwinContext().Get<AppSignInManager>(); }
        }

        /// <summary>
        /// Navigates to the Login view
        /// </summary>
        /// <param name="returnUrl">URL to return after successful login</param>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel
            {
                RememberMe = true,
                ReturnUrl = returnUrl
            });
        }

        /// <summary>
        /// Executes th login
        /// </summary>
        /// <param name="model">User login information</param>
        /// <param name="returnUrl">URL to return after successful login</param>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // --- This doesn't count login failures towards account lockout
            // --- To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    var subscSrv = ServiceManager.GetService<ISubscriptionService>();
                    var token = await subscSrv.GetUserTokenByEmailAsync(model.Email);
                    CreateAuthenticationTicket(model.RememberMe, token.UserId, token.UserName, token.IsServiceUser, token.SubscriptionId, token.IsSubscriptionOwner);
                    return RedirectToLocal(returnUrl);
                
                case SignInStatus.LockedOut:
                    return View("Lockout");
                
                // ReSharper disable once RedundantCaseLabel
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", Res.Error_InvalidLogin);
                    return View(model);
            }
        }

        /// <summary>
        /// Initializes the SignUpIndex view that allows selecting from local and external authentication
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult SignUpIndex()
        {
            return View();
        }

        /// <summary>
        /// Initializes the sign up with local authentication
        /// </summary>
        [AllowAnonymous]
        public ActionResult SignUp()
        {
            return View();
        }

        /// <summary>
        /// Executes the sign up action
        /// </summary>
        /// <param name="model">Sign up information</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.UserName, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // --- Sign in the user
                    await SignInManager.SignInAsync(user, false, false);

                    // --- Create the subscription for the user
                    var subscrSrv = ServiceManager.GetService<ISubscriptionService>();
                    var newSubscr = new SubscriptionDto
                    {
                        SubscriberName = user.UserName,
                        PrimaryEmail = user.Email,
                        CreatedUtc = DateTimeOffset.UtcNow,
                    };
                    var subscriptionId = await subscrSrv.CreateSubscriptionAsync(newSubscr, user.Id);
                    CreateAuthenticationTicket(false, new Guid(user.Id), user.UserName, false, subscriptionId, true);
                    return RedirectToAction("SelectSubscriptionPackage");
                }
                AddErrors(result);
            }
            return View(model);
        }


        /// <summary>
        /// Initializes the sign up with external authentication
        /// </summary>
        [AllowAnonymous]
        public ActionResult SignUpWithExternalLogin(string provider, string email, string userName)
        {
            return View(new SignUpWithExternalLoginViewModel
            {
                Provider = provider,
                Email = email,
                UserName = userName
            });
        }

        /// <summary>
        /// Initializes the selection of a subscription package
        /// </summary>
        public ActionResult SelectSubscriptionPackage()
        {
            return View();
        }

        /// <summary>
        /// Saves the selected subscription package
        /// </summary>
        /// <param name="model">Subscription package information</param>
        public ActionResult SaveSubscriptionPackage(SubscriptionPackageViewModel model)
        {
            return View(model);
        }

        /// <summary>
        /// Initiates the external login with the specified provider
        /// </summary>
        /// <param name="provider">External provider ID</param>
        /// <param name="reason">Login reason (e.g. login, signup</param>
        /// <param name="returnUrl">URL to return after successful authentication</param>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string reason, string returnUrl)
        {
            // --- Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", 
                new { Reason = reason, ReturnUrl = returnUrl }));
        }

        /// <summary>
        /// Manages the post-login activites after a successful external authentication
        /// </summary>
        /// <param name="reason">Login reason (e.g. login, signup</param>
        /// <param name="returnUrl">URL to return after successful authentication</param>
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string reason, string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // --- At this point the user is successfully logged in.

            // --- Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    if (reason == "signup")
                    {
                        // TODO: A user tried to sign up, but its account is already created.
                    }

                    var subscSrv = ServiceManager.GetService<ISubscriptionService>();
                    var token = await subscSrv.GetUserTokenByProviderDataAsync(loginInfo.Login.LoginProvider,
                        loginInfo.Login.ProviderKey);
                    CreateAuthenticationTicket(false, token.UserId, token.UserName, 
                        token.IsServiceUser, token.SubscriptionId, token.IsSubscriptionOwner);
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                default:
                    // --- The user has no account and subscription yet
                    if (reason == "signup")
                    {
                        return View("SignUpWithExternalLogin",
                            new SignUpWithExternalLoginViewModel
                            {
                                Provider = loginInfo.Login.LoginProvider,
                                Email = loginInfo.Email,
                                UserName = loginInfo.ExternalIdentity.Name
                            });
                    }
                    if (reason == "login")
                    {
                        return View("LoginWithNoSubscriptionView",
                            new SignUpWithExternalLoginViewModel
                            {
                                Provider = loginInfo.Login.LoginProvider,
                                Email = loginInfo.Email,
                                UserName = loginInfo.ExternalIdentity.Name
                            });
                    }
                    return RedirectToLocal(returnUrl);
            }
        }

        /// <summary>
        /// Signs up after an external login
        /// </summary>
        /// <param name="model">Signup information</param>
        /// <param name="returnUrl">URL to return after successful authentication</param>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignUpWithExternalLogin(SignUpWithExternalLoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    // TODO: Implement this view
                    return View("ExternalLoginFailure");
                }
                var user = new AppUser { UserName = model.UserName, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        // --- Sign in the user
                        await SignInManager.SignInAsync(user, false, false);

                        // --- Create the subscription for the user
                        var subscrSrv = ServiceManager.GetService<ISubscriptionService>();
                        var newSubscr = new SubscriptionDto
                        {
                            SubscriberName = user.UserName,
                            PrimaryEmail = user.Email,
                            CreatedUtc = DateTimeOffset.UtcNow,
                        };
                        var subscriptionId = await subscrSrv.CreateSubscriptionAsync(newSubscr, user.Id);
                        CreateAuthenticationTicket(false, new Guid(user.Id), user.UserName, false, subscriptionId, true);
                        return RedirectToAction("SelectSubscriptionPackage");
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        /// <summary>
        /// Logs off the user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            var userName = HttpContext.User.Identity.Name;
            AuthenticationManager.SignOut();
            TerminateAuthenticationTicket(userName);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Manages external login failure
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            // TODO: implement this view
            return View();
        }

        // GET: Subscription
        public ActionResult InviteUsers()
        {
            return View();
        }

        public ActionResult ManageUserRights()
        {
            return View();
        }

        /// <summary>
        /// Checks whether the invitation code provided is valid
        /// </summary>
        /// <param name="id">Invitiation code</param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmInvitation(string id)
        {
            // --- Check the invitation code
            var subscrSrv = ServiceManager.GetService<ISubscriptionService>();
            var invitation = await subscrSrv.GetUserInvitationByCodeAsync(id);
            if (invitation == null)
            {
                return View("InvalidInvitationCode");
            }
            var model = new ConfirmInvitationViewModel
            {
                InvitationId = invitation.Id,
                SubscriptionId = invitation.SubscriptionId ?? -1,
                UserName = invitation.InvitedUserName,
                Email = invitation.InvitedEmail
            };
            return View("RegisterInvitedUser", model);
        }

        [AllowAnonymous]
        public async Task<ActionResult> RegisterInvitedUser(ConfirmInvitationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.UserName, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var subscrSrv = ServiceManager.GetService<ISubscriptionService>();
                    var userGuid = new Guid(user.Id);
                    await subscrSrv.AssignUserToSubscription(userGuid, model.SubscriptionId);
                    await subscrSrv.SetInvitationState(model.InvitationId, UserInvitationState.ACTIVATED);
                    CreateAuthenticationTicket(false, userGuid, user.UserName, false, model.SubscriptionId, false);
                    await SignInManager.SignInAsync(user, false, false);
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }
            return View(model);
        }

        #region Helpers

        // --- Used for XSRF protection when adding external logins
        private const string XSRF_KEY = "XsrfId";

        /// <summary>
        /// Gets the object that manages authentication
        /// </summary>
        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        /// <summary>
        /// Add an error to the current model
        /// </summary>
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        /// <summary>
        /// Redirect to a local URL
        /// </summary>
        /// <param name="returnUrl">Local URL to redirect to</param>
        /// <returns></returns>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl) 
                ? (ActionResult) Redirect(returnUrl) 
                : RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Represents the result of an external authentication
        /// </summary>
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XSRF_KEY] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        /// <summary>
        /// Creates an authentication ticket with the specified properties
        /// </summary>
        /// <param name="keepLoggedIn">Keep the user logged in</param>
        /// <param name="userId">User ID</param>
        /// <param name="userName">User name</param>
        /// <param name="isServiceUser">Is this user a service user?</param>
        /// <param name="subscriptionId">Optional subscription ID</param>
        /// <param name="isSubscriptionOwner">Is this user a subscription owner?</param>
        public void CreateAuthenticationTicket(bool keepLoggedIn, Guid userId, string userName, bool isServiceUser, 
            int? subscriptionId, bool isSubscriptionOwner)
        {
            var appPrincipal = new AppPrincipal(userId, userName, isServiceUser, subscriptionId, isSubscriptionOwner,
                null);
            var jsonTicket = appPrincipal.Serialize();
            var authTicket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.AddHours(24), true, jsonTicket);
            var encTicket = FormsAuthentication.Encrypt(authTicket);
            var faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            if (keepLoggedIn)
            {
                faCookie.Expires = DateTime.Now.AddDays(14);
            }
            Response.Cookies.Add(faCookie);
        }

        /// <summary>
        /// Creates an authentication ticket with the specified properties
        /// </summary>
        public static void TerminateAuthenticationTicket(string userName)
        {
            var authTicket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.AddHours(24), true,
                AuthenticationTicketState.Terminated);
            var encTicket = FormsAuthentication.Encrypt(authTicket);
            var faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            System.Web.HttpContext.Current.Response.Cookies.Add(faCookie);
        }

        #endregion
    }
}