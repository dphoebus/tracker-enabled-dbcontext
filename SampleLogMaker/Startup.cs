using Microsoft.Owin;

using SampleLogMaker;

[assembly: OwinStartup(typeof(Startup))]

namespace SampleLogMaker
{
    using Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}