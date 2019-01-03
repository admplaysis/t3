//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Routing;
//using System.Web.Security;
using SGI.App_Start;
using SGI.Areas.PlugAndPlay.AppServices;
using SGI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using FormsAuthenticationExtensions;
using System.Web.Script.Serialization;
using SGI.Autenticacao;
using SGI.Util;

namespace SGI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            System.Diagnostics.Debug.WriteLine("\n\n ### - - - > " + exception.Message + " < - - - ### \n\n");//remover depois

            //Response.Clear();
            if (new HttpRequestWrapper(System.Web.HttpContext.Current.Request).IsAjaxRequest())
            {
                IController errorController = new ErrosController();
                RouteData routeData = new RouteData();
                routeData.Values.Add("controller", "Error");
                routeData.Values.Add("error", exception);
                routeData.Values.Add("action", "ErroAjax");
                errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
            }
            //else
            //{
            //    var httpException = exception as HttpException;

            //    if (httpException != null)
            //    {
            //        string action;

            //        switch (httpException.GetHttpCode())
            //        {
            //            case 404:
            //                // page not found
            //                action = "Erro404";
            //                break;
            //            case 403:
            //                // forbidden
            //                action = "Erro403";
            //                break;
            //            case 500:
            //                // server error
            //                action = "Erro500";
            //                break;
            //            default:
            //                action = "Index";
            //                break;
            //        }
            //        // clear error on server
            //        Server.ClearError();

            //        Response.Redirect(String.Format("~/Erros/{0}", action));
            //    }
            //    else
            //    {
            //        // this is my modification, which handles any type of an exception.
            //        Response.Redirect(String.Format("~/Erros/Index"));
            //    }
            //}
        }
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null || authCookie.Value == "")
            {
                return;
            }
            try
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                CustomPrincipalSerializeModel serializeModel = new JavaScriptSerializer().Deserialize<CustomPrincipalSerializeModel>(authTicket.UserData);
                CustomPrincipal newUser = new CustomPrincipal(authTicket.Name, serializeModel.Roles) {
                    Id = serializeModel.Id,
                    Name = serializeModel.Name,
                };
                HttpContext.Current.User = newUser;
            }
            catch(Exception ex)
            {
                return;
            }
        }
    }
}
