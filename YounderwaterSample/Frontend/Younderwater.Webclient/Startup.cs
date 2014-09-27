using Microsoft.Owin;
using Owin;
using Younderwater.Webclient;

[assembly: OwinStartup(typeof(Startup))]

namespace Younderwater.Webclient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
