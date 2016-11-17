using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Aeero.Startup))]
namespace Aeero
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
