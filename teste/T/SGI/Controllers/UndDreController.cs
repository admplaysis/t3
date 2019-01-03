using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SGI.Models;
using SGI.Context;
using SGI.Util;

namespace SGI.Controllers
{
    [CustomAuthorize(Roles = "AdiminstradorTI, AdiminstradorPCP")]
    public class UndDreController : Controller
    {
        private JSgi db = new JSgi();

        public ActionResult Index(string searchString, int? nPageSize, int? page)
        {
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            var unidades = db.Tr_Unidade.OrderBy(x => x.UNI_DESCRICAO).ToList();

            //Valida Pesquisa
            if (searchString != null && searchString != "")
                unidades = unidades.Where(x => (x.UNI_DESCRICAO).ToUpper().Contains(searchString.ToUpper())).OrderBy(y => y.UNI_DESCRICAO).ToList();

            return View(unidades.ToPagedList(_PageNumber, _PageSize));
        }

        public ActionResult Details(int id)
        {
            var unidade = db.Tr_Unidade.Find(id);
            return View(unidade);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tr_Unidade unidade)
        {
            //Validações
            #region Validacoes
            if (unidade.UNI_DESCRICAO == "" || unidade.UNI_DESCRICAO == null)
                ModelState.AddModelError("UNI_DESCRICAO", "Obrigatório informar a descrição.");
            #endregion Validacoes
            if (ModelState.IsValid)
            {
                db.Tr_Unidade.Add(unidade);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(unidade);
        }

        public ActionResult Edit(int id)
        {
            var unidade = db.Tr_Unidade.Find(id);
            return View(unidade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tr_Unidade unidade)
        {
            //Validações
            #region Validacoes

            if (unidade.UNI_DESCRICAO == "" || unidade.UNI_DESCRICAO == null)
                ModelState.AddModelError("UNI_DESCRICAO", "Obrigatório informar a descrição.");
            #endregion Validacoes
            if (ModelState.IsValid)
            {
                db.Entry(unidade).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(unidade);
        }

        public ActionResult Delete(int id)
        {
            var unidade = db.Tr_Unidade.Find(id);
            return View(unidade);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tr_Unidade unidade = db.Tr_Unidade.Find(id);
            db.Tr_Unidade.Remove(unidade);
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