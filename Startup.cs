using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppAzureAuthentication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            try
            {
                ConfigureAuth(app);
            }
            catch (Exception ex)
            {
                logger.Info("Exception - " + ex.Message.ToString() + "StakStace => " + ex.StackTrace.ToString());
            }
        }
    }
}