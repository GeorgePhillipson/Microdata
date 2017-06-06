using System;
using System.Web;

namespace RequiresHttps
{
    /// <summary>
    /// Redirects the Request to HTTPS if it comes in on an insecure channel.
    /// </summary>
    public class HttpsOnlyModule : IHttpModule
    {

        public void Init(HttpApplication app)
        {
            app.BeginRequest += (app_BeginRequest);

        }

        private void app_BeginRequest(object sender, EventArgs e)
        {
            int securePort = 443;

            if (!HttpContext.Current.Request.IsSecureConnection || HttpContext.Current.Request.Url.Port != securePort)
            {
                var host = HttpContext.Current.Request.Url.Host;
                var rawUrl = HttpContext.Current.Request.RawUrl;
                var newUrl = $"https://{host}{rawUrl}";
                //HttpContext.Current.Response.RedirectPermanent(newUrl); 
                HttpContext.Current.Response.Redirect("https://localhost:44383/");
            }
        }

        public void Dispose()
        {
            // Needed for IHttpModule
        }
    }
}
