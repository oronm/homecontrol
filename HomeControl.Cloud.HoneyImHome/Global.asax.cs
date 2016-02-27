using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using HomeControl.Cloud.Contracts;

namespace HomeControl.Cloud.HoneyImHome
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        // TODO : UGLY!!! Change This!!
        public static ITokensStore TokensStore { get { return _tokensStore; } }
        private static ITokensStore _tokensStore;

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            WebApiApplication._tokensStore = new UglyTokenStore();
        }
    }
}
