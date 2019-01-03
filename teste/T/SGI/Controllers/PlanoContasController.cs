using SGI.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SGI.Models;
using SGI.Util;

namespace SGI.Controllers
{
    [CustomAuthorize(Roles = "AdiminstradorTI, AdiminstradorPCP")]
    public class PlanoContasController : Controller
    {
        private JSgi db = new JSgi();
        //
        // GET: /PlanoConta/

        public ActionResult Index(string searchString, int? nPageSize, int? page)
        {
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            var plano = db.Tr_PlanoContas.OrderBy(x => x.PLA_CODIGO).ToList();
            //Valida Pesquisa
            if (searchString != null && searchString != "")
                plano = plano.Where(x => (x.PLA_DESCRICAO + x.PLA_CODIGO).ToUpper().Contains(searchString.ToUpper())).OrderBy(y => y.PLA_CODIGO).ToList();

            return View(plano.OrderBy(x => x.PLA_CODIGO).ToPagedList(_PageNumber, _PageSize));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Tr_PlanoContas PlanoContas)
        {
            if(ModelState.IsValid)
            {
                db.Tr_PlanoContas.Add(PlanoContas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(PlanoContas);
        }

        public ActionResult Edit(int id)
        {
            var planoContas = db.Tr_PlanoContas.Find(id);
            return View(planoContas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(Tr_PlanoContas PlanoContas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(PlanoContas).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(PlanoContas);
        }

        public ActionResult Details(int id)
        {
            var planoContas = db.Tr_PlanoContas.Find(id);
            return View(planoContas);
        }

        public ActionResult Delete(int id)
        {
            var planoContas = db.Tr_PlanoContas.Find(id);
            return View(planoContas);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tr_PlanoContas planoContas = db.Tr_PlanoContas.Find(id);
            db.Tr_PlanoContas.Remove(planoContas);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
