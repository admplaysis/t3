using SGI.Util;
using System.Web;
using System.Web.Mvc;

namespace SGI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}