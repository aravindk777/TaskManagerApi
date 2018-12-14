using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Unity.AspNet.WebApi;

namespace TaskManApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // -- Enable CORS
            config.EnableCors();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{TaskId}",
                defaults: new { TaskId = RouteParameter.Optional }
                //constraints: new { TaskId = @"^[0-9]+$" }
            );

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApiWithActionName",
            //    routeTemplate: "{controller}/{action}/{name}",
            //    defaults: null,
            //    constraints: new { name = @"&^[a-z]+$" }
            //);

            //config.Routes.MapHttpRoute(
            //    name: "ApiByAction",
            //    routeTemplate: "{controller}/{action}",
            //    defaults: new { action = "Get" }
            //    );
        }
    }
}
