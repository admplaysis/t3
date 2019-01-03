using System.Web.Mvc;

namespace SGI.Areas.PlugAndPlay
{
    public class PlugAndPlayAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PlugAndPlay";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PlugAndPlay_default",
                "PlugAndPlay/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
