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
    public class GrupoController : Controller
    {
        private JSgi db = new JSgi();

        public ActionResult Index(string searchString, int? nPageSize, int? page)
        {
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            var grupos = db.T_Grupo.OrderBy(x => x.NOME).ToList();

            //Valida Pesquisa
            if (searchString != null && searchString != "")
                grupos = grupos.Where(x => (x.NOME).ToUpper().Contains(searchString.ToUpper())).OrderBy(y => y.NOME).ToList();

            return View(grupos.ToPagedList(_PageNumber, _PageSize));
        }

        public ActionResult Details(int id)
        {
            var grupo = db.T_Grupo.Find(id);
            return View(grupo);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(T_Grupo grupo)
        {
            Validacoes(grupo);
            if (ModelState.IsValid)
            {
                db.T_Grupo.Add(grupo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(grupo);
        }

        public ActionResult Edit(int id)
        {
            var grupo = db.T_Grupo.Find(id);
            return View(grupo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(T_Grupo grupo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(grupo).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(grupo);
        }

        public void Validacoes(T_Grupo grupos)
        {
            if (grupos.NOME == "")
                ModelState.AddModelError("NOME", "Informe o nome do grupo");
        }

        public ActionResult Delete(int id)
        {
            var grupo = db.T_Grupo.Find(id);   
            return View(grupo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            T_Grupo grupo = db.T_Grupo.Find(id);
            db.T_Grupo.Remove(grupo);
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