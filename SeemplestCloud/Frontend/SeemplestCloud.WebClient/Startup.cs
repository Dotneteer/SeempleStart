using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SeemplestCloud.WebClient.Startup))]
namespace SeemplestCloud.WebClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
