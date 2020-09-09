using ApplicationMaintanceTool.DAL;
using ApplicationMaintanceTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ApplicationMaintanceTool
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services



            // Web API routes
            config.EnableCors();

            config.Filters.Add(new CustomRequireHttpsAttribute());
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
