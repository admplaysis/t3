using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SGI.Areas.PlugAndPlay.Controllers
{
    public class AspController : Controller
    {
        // GET: PlugAndPlay/Asp
        public string getData()
        {
            DateTime dataMenor = new DateTime(1970, 01, 01, 00, 00, 00);
            DateTime dataAtual = DateTime.Now;
            var difSeconds = (dataAtual - dataMenor).TotalSeconds;
            return string.Format("{0}", Math.Floor(difSeconds));
        }
    }
}