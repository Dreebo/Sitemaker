using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sitemaker.Startup))]
namespace Sitemaker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
