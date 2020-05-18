using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KillBug.Startup))]
namespace KillBug
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
