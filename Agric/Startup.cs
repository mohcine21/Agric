using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Agric.StartupOwin))]

namespace Agric
{
    public partial class StartupOwin
    {
        public void Configuration(IAppBuilder app)
        {
            //AuthStartup.ConfigureAuth(app);
        }
    }
}
