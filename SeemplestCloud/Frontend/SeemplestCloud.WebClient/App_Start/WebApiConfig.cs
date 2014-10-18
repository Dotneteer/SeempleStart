using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SeemplestBlocks.Core.ServiceInfrastructure;

namespace SeemplestCloud.WebClient
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // --- Setup JSON formatting
            var jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            var jSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.None,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };
            jSettings.Converters.Add(new UtcToLocalDateTimeConverter());
            jsonFormatter.SerializerSettings = jSettings;
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // --- Register exception handling
            GlobalConfiguration.Configuration.Filters.Add(new WebApiExceptionHandlingAttribute());
        }
    }
}
