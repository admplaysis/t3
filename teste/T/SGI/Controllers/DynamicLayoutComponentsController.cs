using SGI.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SGI.Controllers
{
    public class DynamicLayoutComponentsController : Controller
    {
        // GET: DynamicLayoutComponents
        [ChildActionOnly]
        public ActionResult Menu()
        {
            using (var db = new JSgi())
            {
                try
                {
                    ViewBag.MaquinasMenu = db.Maquina.Where(f => f.ControlIp != "" && f.ControlIp != null && (f.MaqIdMaqPai == null||f.MaqIdMaqPai == "")).ToList();
                }
                finally { }
            }
            return View();
        }
    }
}