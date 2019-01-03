using SGI.Context;
using SGI.Models;
using SGI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SGI.Controllers
{
    [CustomAuthorize(Roles = "AdiminstradorTI, AdiminstradorPCP")]
    public class DreController : Controller
    {
        private JSgi db = new JSgi();
        //
        // GET: /Dre/

        public ActionResult Index(int? pIdVisao, string pGrafico, string pAno)
        {
            int idVisao = pIdVisao ??
                (db.Tr_CabViscao.Count(x => x.CAB_STATUS == (int)Enums.Ativo.Liberado) > 0 ? db.Tr_CabViscao.Where(x => x.CAB_STATUS != (int)Enums.Ativo.EmEdicao).FirstOrDefault().CAB_ID : 0);
            //Defini layout
            #region ViewDataLayout
            ViewData["mainmenu_scroll"] = ""; //pagescroll , fixedscroll
            ViewData["body_class"] = "sidebar-collapse";
            ViewData["html_class"] = "";
            ViewData["pagetopbar_class"] = "sidebar_shift";
            ViewData["maincontent_class"] = "sidebar_shift";
            ViewData["maincontent_type"] = "";
            ViewData["pagesidebar_class"] = "collapseit";
            ViewData["pagechatapi_class"] = "";
            ViewData["chatapiwindows_demo"] = "";
            ViewData["chatapiwindows_class"] = "";
            #endregion ViewDataLayout

            #region ViewBags
            ViewBag.pIdVisao = new SelectList(db.Tr_CabViscao.Where(x => x.CAB_STATUS == (int)SGI.Enums.Ativo.Liberado), "CAB_ID", "CAB_DESC", idVisao);
            var anos = db.Tr_Movimentos.Select(x => new { Ano = x.MOV_DATA.Substring(0, 4) }).GroupBy(x => new { x.Ano }).OrderByDescending(x => x.Key.Ano).ToList();
            pAno = (pAno == "" || pAno == null ? anos.FirstOrDefault().Key.Ano : pAno);
            ViewBag.anoAtual = pAno;
            ViewBag.pAno = new SelectList(anos, "Key.Ano", "Key.Ano", pAno);
            #endregion ViewBags
            var dre = new Dre();
            //Busca dados da DRE
            dre.Tr_Visoes = db.Tr_Visoes.ToList();
            dre.DreView = ExtratorDre.DrePadrao(idVisao, pAno);

            //Busca unidade de medida
            #region AgrupaUnidades
            var uniAgrupadas = dre.DreView.Select(x => new { x.MOV_UNID, x.UNI_DESCRICAO }).GroupBy(x => new { UNI_ID = x.MOV_UNID, UNI_DESCRICAO = x.UNI_DESCRICAO }).ToList();
            var uniLocalizadas = db.Tr_Unidade.ToList();
            dre.Unidades = uniLocalizadas.Where(x => uniAgrupadas.Any(j => j.Key.UNI_ID == x.UNI_ID)).ToList();
            #endregion AgrupaUnidades
            return View(dre);
        }


        public ActionResult Grafico(int? pIdVisao, string pGrafico)
        {
            int idVisao = pIdVisao ??
                (db.Tr_CabViscao.Count(x => x.CAB_STATUS == (int)Enums.Ativo.Liberado) > 0 ? db.Tr_CabViscao.Where(x => x.CAB_STATUS != (int)Enums.Ativo.EmEdicao).FirstOrDefault().CAB_ID : 0);
            //Defini layout
            #region ViewDataLayout
            ViewData["body_class"] = "sidebar-collapse";
            ViewData["pagetopbar_class"] = "sidebar_shift";
            ViewData["pagesidebar_class"] = "collapseit";
            #endregion ViewDataLayout

            #region ViewBags
            ViewBag.pIdVisao = new SelectList(db.Tr_CabViscao.Where(x => x.CAB_STATUS == (int)SGI.Enums.Ativo.Liberado), "CAB_ID", "CAB_DESC", idVisao);
            #endregion ViewBags
            var dre = new Dre();
            //Busca dados da DRE
            dre.Tr_Visoes = db.Tr_Visoes.ToList();
            dre.DreView = Util.ExtratorDre.DrePadrao(idVisao, "2017");

            //Busca unidade de medida
            #region AgrupaUnidades
            var uniAgrupadas = dre.DreView.Select(x => new { x.MOV_UNID, x.UNI_DESCRICAO }).GroupBy(x => new { UNI_ID = x.MOV_UNID, UNI_DESCRICAO = x.UNI_DESCRICAO }).ToList();
            var uniLocalizadas = db.Tr_Unidade.ToList();
            dre.Unidades = uniLocalizadas.Where(x => uniAgrupadas.Any(j => j.Key.UNI_ID == x.UNI_ID)).ToList();
            #endregion AgrupaUnidades
            return View(dre);
        }
    }
}
