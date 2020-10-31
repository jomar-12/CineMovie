using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CineMovie.Startup))]
namespace CineMovie
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
