using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;

namespace K2.WebApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //GlobalConfiguration.Configuration.UseSqlServerStorage("K2Context");
            //app.UseHangfireDashboard();
            //app.UseHangfireServer();

        }
    }
}