using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SearchDemo.Site.Startup))]
namespace SearchDemo.Site
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
