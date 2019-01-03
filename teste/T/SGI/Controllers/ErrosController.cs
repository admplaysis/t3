using SGI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SGI.Controllers
{
    [CustomAuthorize]
    public class ErrosController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Erro404()
        {
            return View();
        }

        public ActionResult Erro403()
        {
            return View();
        }

        public ActionResult Erro405()
        {
            return View();
        }

        public ActionResult Erro408()
        {
            return View();
        }

        public ActionResult Erro500()
        {
            return View();
        }

        public ActionResult Erro503()
        {
            return View();
        }
        public HttpStatusCodeResult ErroAjax() {
            return new HttpStatusCodeResult(500);
        }
    }
}
