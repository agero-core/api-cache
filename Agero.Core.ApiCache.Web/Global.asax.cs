﻿using System.Web;
using System.Web.Http;

namespace Agero.Core.ApiCache.Web
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
