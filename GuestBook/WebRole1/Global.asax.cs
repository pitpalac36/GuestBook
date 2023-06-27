using System;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

namespace GuestBook.WebRole
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BeginRequest += Application_BeginRequest;
        }

        private void Application_BeginRequest(object sender, EventArgs e)
        {
            string url = HttpContext.Current.Request.Url.AbsolutePath.ToLower();
            if (string.IsNullOrEmpty(url) || url == "/")
            {
                HttpContext.Current.Response.Redirect("~/Default.aspx");
            }
        }
    }
}