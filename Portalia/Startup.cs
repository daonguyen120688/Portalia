using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Portalia.Startup))]
[assembly:log4net.Config.XmlConfigurator(ConfigFile = "web.config", Watch = true)]
namespace Portalia
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
