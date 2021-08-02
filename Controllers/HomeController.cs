using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAppAzureAuthentication.Controllers
{
    public class HomeController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static string redirectedUri = ConfigurationManager.AppSettings["RedirectedUri"];
        private static string NoAccessPath = ConfigurationManager.AppSettings["PathError"];
        public ActionResult Index()
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    //Write your own logic as user is already authenticated.
                    var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;
                    //cst_claim_ein parameter should be set in azure end.
                    string txtUserEIN = userClaims?.FindFirst("cst_claim_ein")?.Value;
                    string txtUserName = userClaims?.FindFirst("name")?.Value;
                }
                else
                {
                    HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties()
                    {
                        RedirectUri = "/",
                        AllowRefresh = true
                    },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
                }
            }
            catch (Exception ex)
            {
                logger.Info("Exception - " + ex.Message.ToString() + "StakStace => " + ex.StackTrace.ToString());
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}