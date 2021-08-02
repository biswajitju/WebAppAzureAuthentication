using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using NLog;
using Owin;
using System;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Threading.Tasks;

namespace WebAppAzureAuthentication
{
    public partial class Startup
    {
        //Gets the keys from Web.Config file
        private static string clientId = ConfigurationManager.AppSettings["ClientId"];
        private static string tenantId = ConfigurationManager.AppSettings["ClientId"];
        private static string addInstance = ConfigurationManager.AppSettings["AADInstance"];
        private static string redirectedUri = ConfigurationManager.AppSettings["RedirectedUri"];
        private static string NoAccessPath = ConfigurationManager.AppSettings["PathError"];

        //Concatenate addInstance, tenantId to form authority value
        private string authority = string.Format(CultureInfo.InvariantCulture, addInstance, tenantId, "/v2.0");
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public void ConfigureAuth(IAppBuilder app)
        {
            try
            {
                app.SetDefaultSignInAsAuthenticationType (CookieAuthenticationDefaults.AuthenticationType);

                app.UseCookieAuthentication(new CookieAuthenticationOptions()
                {
                    //Added to resolved the login issue of multiple redirection 
                    AuthenticationType = "Cookies",
                    CookieManager = new SystemWebChunkingCookieManager()
                }) ;

                app.UseOpenIdConnectAuthentication(
                    new OpenIdConnectAuthenticationOptions
                    {
                        ClientId = clientId,
                        Authority = authority,
                        RedirectUri = redirectedUri,
                        PostLogoutRedirectUri = redirectedUri,
                        Scope = OpenIdConnectScope.OpenIdProfile,
                        ResponseType = OpenIdConnectResponseType.CodeIdToken,
                        TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                        {
                            ValidateIssuer = true,
                            ValidIssuer = tenantId
                        },
                        Notifications = new OpenIdConnectAuthenticationNotifications
                        {
                            RedirectToIdentityProvider = (context) =>
                            {
                                //Each time login screen will appear and  manully put credentials to login.
                                //If you want your cred saved after first login then conmment below line
                                context.ProtocolMessage.Prompt = "login";
                                return Task.FromResult(0);
                            },
                            AuthenticationFailed = (context) =>
                            {
                                logger.Info("Authentication Failed = > " + context.Exception.Message);
                                context.HandleResponse();
                                context.Response.Redirect(NoAccessPath);
                                return Task.FromResult(0);
                            },

                        }

                    });
                
            }
            catch (Exception ex)
            {
                logger.Info("Exception - " + ex.Message.ToString() + "StakStace => " + ex.StackTrace.ToString());
            }

        }
    }
}