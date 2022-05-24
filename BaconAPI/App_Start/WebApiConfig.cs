using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace BaconAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors(); // enable cors

            // Fix: Self referencing loop, 
            // solution from https://stackoverflow.com/questions/7397207/json-net-error-self-referencing-loop-detected-for-type
            config.Formatters
                .JsonFormatter
                .SerializerSettings
                .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            // enable json for output
            config.Formatters
                .JsonFormatter
                .SupportedMediaTypes
                .Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
