using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Swashbuckle.Application;

namespace HomeControl.Cloud.HoneyImHome
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            log4net.Config.XmlConfigurator.Configure();
            config.EnableCors();

            // Web API routes
            config.MapHttpAttributeRoutes();
            //config.EnableSwagger(c => c.SingleApiVersion("v1", "HoneyImHome")).EnableSwaggerUi();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
