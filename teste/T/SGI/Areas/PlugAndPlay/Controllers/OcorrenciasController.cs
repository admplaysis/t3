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
    public class OcorrenciasController : Controller
    {
        private JSgi db = new JSgi();

        // GET: PlugAndPlay/Ocorrencias
        public ActionResult Index()
        {
            var ocorrencia = db.Ocorrencia.Include(o => o.TipoOcorrencia);
            return View(ocorrencia.ToList());
        }

        // GET: PlugAndPlay/Ocorrencias/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ocorrencia ocorrencia = db.Ocorrencia.Find(id);
            if (ocorrencia == null)
            {
                return HttpNotFound();
            }
            return View(ocorrencia);
        }


        public void Validacoes(Ocorrencia ocorrencia)
        {
            if(ocorrencia.Id == null || ocorrencia.Id.Trim().Equals(""))
            {
                ModelState.AddModelError("Id", "Informe o Id");
            }
           if(ocorrencia.Descricao == null || ocorrencia.Descricao.Trim().Equals(""))
            {
                ModelState.AddModelError("Descricao", "Informe a Descricao");
            }
        }


        // GET: PlugAndPlay/Ocorrencias/Create
        public ActionResult Create()
        {
            ViewBag.TipoOcorrenciaId = new SelectList(db.TipoOcorrencia, "Id", "Descricao");
            return View();
        }

        // POST: PlugAndPlay/Ocorrencias/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descricao,TipoOcorrenciaId")] Ocorrencia ocorrencia)
        {
            Validacoes(ocorrencia);
            if (ModelState.IsValid)
            {
                db.Ocorrencia.Add(ocorrencia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TipoOcorrenciaId = new SelectList(db.TipoOcorrencia, "Id", "Descricao", ocorrencia.TipoOcorrenciaId);
            return View(ocorrencia);
        }

        // GET: PlugAndPlay/Ocorrencias/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ocorrencia ocorrencia = db.Ocorrencia.Find(id);
            if (ocorrencia == null)
            {
                return HttpNotFound();
            }
            ViewBag.TipoOcorrenciaId = new SelectList(db.TipoOcorrencia, "Id", "Descricao", ocorrencia.TipoOcorrenciaId);
            return View(ocorrencia);
        }

        // POST: PlugAndPlay/Ocorrencias/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descricao,TipoOcorrenciaId")] Ocorrencia ocorrencia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ocorrencia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TipoOcorrenciaId = new SelectList(db.TipoOcorrencia, "Id", "Descricao", ocorrencia.TipoOcorrenciaId);
            return View(ocorrencia);
        }

        // GET: PlugAndPlay/Ocorrencias/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ocorrencia ocorrencia = db.Ocorrencia.Find(id);
            if (ocorrencia == null)
            {
                return HttpNotFound();
            }
            return View(ocorrencia);
        }

        // POST: PlugAndPlay/Ocorrencias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Ocorrencia ocorrencia = db.Ocorrencia.Find(id);
            db.Ocorrencia.Remove(ocorrencia);
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
