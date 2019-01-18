/* 
 * Class - ApiConfiguration file
 * Created by: Aravind Kothandaraman (aravind.pk@aol.in)
 */

using Swashbuckle.Application;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace TaskManApi
{
    /// <summary>
    /// Web Api startup configuration class
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registration method
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // -- Enable CORS
            config.EnableCors();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.IgnoreRoute("schema","{resource}.axd/{*pathInfo}");

            config.Routes.MapHttpRoute(
                name: "swagger_root",
                routeTemplate: "",
                defaults: null,
                constraints: null,
                handler: new RedirectHandler((message => message.RequestUri.ToString()), "swagger"));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{TaskId}",
                defaults: new { TaskId = RouteParameter.Optional }
                //constraints: new { TaskId = @"^[0-9]+$" }
            );

            config.Formatters.XmlFormatter.AddQueryStringMapping("format", "xml", "text/xml");
            config.Formatters.JsonFormatter.AddQueryStringMapping("format", "json", "application/json");
        }
    }
}
