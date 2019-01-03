using SGI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using PagedList;
using SGI.Context;
using SGI.Util;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SGI.Controllers
{
    [CustomAuthorize(Roles = "AdiminstradorTI, AdiminstradorPCP")]
    public class DepartamentoController : Controller
    {
        private JSgi db = new JSgi();

        public ActionResult Index(string searchString, int? nPageSize, int? page)
        {
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            var departamentos = db.T_Departamentos.OrderBy(x => x.DEP_NOME).ToList();

            //Valida Pesquisa
            if (searchString != null && searchString != "")
                departamentos = departamentos.Where(x => (x.DEP_NOME).ToUpper().Contains(searchString.ToUpper())).OrderBy(y => y.DEP_NOME).ToList();

            return View(departamentos.ToPagedList(_PageNumber, _PageSize));
        }

        public ActionResult Details(int id)
        {
            var departamento = db.T_Departamentos.Find(id);
            return View(departamento);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(T_Departamentos departamento)
        {
            Validacoes(departamento);
            if (ModelState.IsValid)
            {
                db.T_Departamentos.Add(departamento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(departamento);
        }

        public ActionResult Edit(int id)
        {
            var departamento = db.T_Departamentos.Find(id);
            return View(departamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(T_Departamentos departamento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(departamento).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(departamento);
        }

        public void Validacoes(T_Departamentos departamentos)
        {
            if (departamentos.DEP_NOME == "" || departamentos.DEP_NOME == null)
                ModelState.AddModelError("DEP_NOME", "Informe o nome do departamento");
        }

        public ActionResult Delete(int id)
        {
            var departamento = db.T_Departamentos.Find(id);
            return View(departamento);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            T_Departamentos departamento = db.T_Departamentos.Find(id);
            db.T_Departamentos.Remove(departamento);
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