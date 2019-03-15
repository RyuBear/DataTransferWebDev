using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DataTransferWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
             HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null)
            {
                // get culture name
                var cultureInfoName = CultureHelper.GetImplementedCulture(cultureCookie.Value);

                // set culture
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureInfoName);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cultureInfoName);
            }
            else
            {
                string[] culture = Request.UserLanguages;
                if (culture == null)
                {
                    string cName = System.Configuration.ConfigurationManager.AppSettings["CultureName"].ToString();
                    culture = new string[] { cName };
                }
                    
                // get culture name
                var cultureInfoName = CultureHelper.GetImplementedCulture(culture[0]);

                // set culture
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureInfoName);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cultureInfoName);
            }
        }
    }
}
