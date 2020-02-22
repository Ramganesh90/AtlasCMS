using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Atlas.Startup))]
namespace Atlas
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
