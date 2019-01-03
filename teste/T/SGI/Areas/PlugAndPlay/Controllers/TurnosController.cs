using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SGI.Areas.PlugAndPlay.Models;
using SGI.Context;
using SGI.Util;

namespace SGI.Areas.PlugAndPlay.Controllers
{
    [CustomAuthorize(Roles = "AdiminstradorTI, AdiminstradorPCP")]
    public class TurnosController : Controller
    {
        private JSgi db = new JSgi();

        // GET: PlugAndPlay/Turnos
        public ActionResult Index()
        {
            return View(db.Turno.ToList());
        }


        public void Validacoes(Turno turno)
        {

            if (turno.Id == null || turno.Id.Trim().Equals(""))
                ModelState.AddModelError("Id", "Informe o Id");

            if (turno.Descricao == null || turno.Descricao.Trim().Equals(""))
                ModelState.AddModelError("Descricao", "Informe a Descricao");

            /*ATE AGORA OS UNICOS CAMPOS EXISTENTES SAO `DESCRICAO` E `ID`, ENTAO NAO TEM NECESSIDADE
             * DE VALIDAR OS OUTRO CAMPOS AINDA 
             * 
             * 
            if (turno.Medicoes== null || turno.Medicoes.Count()>0)
                ModelState.AddModelError("Medicoes", "Informe a Medicao");*/
            
        }

        // GET: PlugAndPlay/Turnos/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turno turno = db.Turno.Find(id);
            if (turno == null)
            {
                return HttpNotFound();
            }
            return View(turno);
        }

        // GET: PlugAndPlay/Turnos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlugAndPlay/Turnos/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descricao")] Turno turno)
        {
            Validacoes(turno);
            if (ModelState.IsValid)
            {
                db.Turno.Add(turno);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(turno);
        }

        // GET: PlugAndPlay/Turnos/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turno turno = db.Turno.Find(id);
            if (turno == null)
            {
                return HttpNotFound();
            }
            return View(turno);
        }

        // POST: PlugAndPlay/Turnos/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descricao")] Turno turno)
        {
            if (ModelState.IsValid)
            {
                db.Entry(turno).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(turno);
        }

        // GET: PlugAndPlay/Turnos/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turno turno = db.Turno.Find(id);
            if (turno == null)
            {
                return HttpNotFound();
            }
            return View(turno);
        }

        // POST: PlugAndPlay/Turnos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Turno turno = db.Turno.Find(id);
            db.Turno.Remove(turno);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
