using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProjectDP.Startup))]
namespace ProjectDP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
