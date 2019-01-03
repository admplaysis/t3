using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using PagedList;
using SGI.Models;
using SGI.Context;
using SGI.Util;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SGI.Controllers
{
    [CustomAuthorize(Roles = "AdiminstradorTI, AdiminstradorPCP")]
    public class NegocioController : Controller
    {
        private JSgi db = new JSgi();

        public ActionResult Index(string searchString, int? nPageSize, int? page)
        {
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            var negocios = db.T_Negocio.OrderBy(x => x.NEG_DESCRICAO).ToList();

            //Valida Pesquisa
            if (searchString != null && searchString != "")
                negocios = negocios.Where(x => (x.NEG_DESCRICAO).ToUpper().Contains(searchString.ToUpper())).OrderBy(y => y.NEG_DESCRICAO).ToList();

            return View(negocios.ToPagedList(_PageNumber, _PageSize));
        }

        public ActionResult Details(int id)
        {
            var negocio = db.T_Negocio.Find(id);
            return View(negocio);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(T_Negocio negocio)
        {
            Validacoes(negocio);
            if (ModelState.IsValid)
            {
                db.T_Negocio.Add(negocio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(negocio);
        }

        public ActionResult Edit(int id)
        {
            var negocio = db.T_Negocio.Find(id);
            return View(negocio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(T_Negocio negocio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(negocio).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(negocio);
        }

        public void Validacoes(T_Negocio negocios)
        {
            if (negocios.NEG_DESCRICAO == "" || negocios.NEG_DESCRICAO == null)
                ModelState.AddModelError("NEG_DESCRICAO", "Informe a descrição do negócio");
        }

        public ActionResult Delete(int id)
        {
            var negocio = db.T_Negocio.Find(id);
            return View(negocio);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            T_Negocio negocio = db.T_Negocio.Find(id);
            db.T_Negocio.Remove(negocio);
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