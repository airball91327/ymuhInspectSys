using Owin;

//[assembly: OwinStartupAttribute(typeof(InspectSystem.Startup))]
namespace InspectSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
