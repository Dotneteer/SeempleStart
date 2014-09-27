﻿using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Younderwater.Webclient.Models.UserManagement;
using Younderwater.Webclient.Providers;

namespace Younderwater.Webclient
{
    public partial class Startup
    {
        // Enable the application to use OAuthAuthorization. You can then secure your Web APIs
        static Startup()
        {
            PublicClientId = "web";

            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                AuthorizeEndpointPath = new PathString("/Account/Authorize"),
                Provider = new AppOAuthProvider(PublicClientId),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            };
        }

        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.CreatePerOwinContext<AppSignInManager>(AppSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<AppUserManager,
                        AppUser>(TimeSpan.FromMinutes(20), 
                        (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "00000000441284CB",
            //    clientSecret: "JbHQEMnSvnUwFILlgyNoTGhrhcevHmBN");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            app.UseFacebookAuthentication(
                appId: "164359940298094",
                appSecret: "9bcae9eaf57bfecccdda61ebf47d4e0b");

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "249525440004-qcsnok7bq6p2ktk37t565vmbgjfpb1af.apps.googleusercontent.com",
                ClientSecret = "Y9Plo7hyNfnwJCHHr_j9U2Hi"
            });
        }
    }
}
