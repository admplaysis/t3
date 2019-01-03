using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SGI.Areas.PlugAndPlay.Models;
using SGI.Context;
using SGI.Util;

namespace SGI.Areas.PlugAndPlay.Controllers
{
    [CustomAuthorize(Roles = "AdiminstradorTI, AdiminstradorPCP")]
    public class TiposOcorrenciaController : Controller
    {
        private JSgi db = new JSgi();

        // GET: PlugAndPlay/TiposOcorrencia
        public ActionResult Index()
        {
            return View(db.TipoOcorrencia.ToList());
        }

        // GET: PlugAndPlay/TiposOcorrencia/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoOcorrencia tipoOcorrencia = db.TipoOcorrencia.Find(id);
            if (tipoOcorrencia == null)
            {
                return HttpNotFound();
            }
            return View(tipoOcorrencia);
        }

        // GET: PlugAndPlay/TiposOcorrencia/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlugAndPlay/TiposOcorrencia/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descricao")] TipoOcorrencia tipoOcorrencia)
        {
            if (ModelState.IsValid)
            {
                db.TipoOcorrencia.Add(tipoOcorrencia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoOcorrencia);
        }

        // GET: PlugAndPlay/TiposOcorrencia/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoOcorrencia tipoOcorrencia = db.TipoOcorrencia.Find(id);
            if (tipoOcorrencia == null)
            {
                return HttpNotFound();
            }
            return View(tipoOcorrencia);
        }

        // POST: PlugAndPlay/TiposOcorrencia/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descricao")] TipoOcorrencia tipoOcorrencia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoOcorrencia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoOcorrencia);
        }

        // GET: PlugAndPlay/TiposOcorrencia/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoOcorrencia tipoOcorrencia = db.TipoOcorrencia.Find(id);
            if (tipoOcorrencia == null)
            {
                return HttpNotFound();
            }
            return View(tipoOcorrencia);
        }

        // POST: PlugAndPlay/TiposOcorrencia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TipoOcorrencia tipoOcorrencia = db.TipoOcorrencia.Find(id);
            db.TipoOcorrencia.Remove(tipoOcorrencia);
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
