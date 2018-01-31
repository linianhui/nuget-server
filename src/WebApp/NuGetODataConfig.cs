using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using NuGet.Server;
using NuGet.Server.V2;
using WebApp;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NuGetODataConfig), "Start")]

namespace WebApp 
{
    public static class NuGetODataConfig 
	{
        public static void Start() 
		{
            ServiceResolver.SetServiceResolver(new DefaultServiceResolver());

            var config = GlobalConfiguration.Configuration;

            config.UseNuGetV2WebApiFeed("NuGetDefault", "nuget", "PackagesOData");

            config.Routes.MapHttpRoute(
                name: "NuGetDefault_ClearCache",
                routeTemplate: "nuget/clear-cache",
                defaults: new { controller = "PackagesOData", action = "ClearCache" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

        }
    }
}
