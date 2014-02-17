using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace SampleApi
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
        }
    }
}