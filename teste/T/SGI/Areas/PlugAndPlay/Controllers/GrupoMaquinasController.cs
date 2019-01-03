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
    public class GrupoMaquinasController : Controller
    {
        private JSgi db = new JSgi();

        // GET: PlugAndPlay/GrupoMaqs
        public ActionResult Index()
        {
            return View(db.GrupoMaquina.ToList());
        }

        // GET: PlugAndPlay/GrupoMaqs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrupoMaquina grupoMaq = db.GrupoMaquina.Find(id);
            if (grupoMaq == null)
            {
                return HttpNotFound();
            }
            return View(grupoMaq);
        }

        // GET: PlugAndPlay/GrupoMaqs/Create
        public ActionResult Create()
        {
            return View();
        }

        public void Validacoes(GrupoMaquina grupo)
        {
            if (grupo.Id == null || grupo.Id.Trim().Equals(""))
                ModelState.AddModelError("Id", "Informe o Id");

            if (grupo.Descricao == null || grupo.Descricao.Trim().Equals(""))
                ModelState.AddModelError("Descricao", "Informe a Descricao");
        }


        // POST: PlugAndPlay/GrupoMaqs/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descricao")] GrupoMaquina grupoMaq)
        {


            Validacoes(grupoMaq);
                if (ModelState.IsValid)
                {
                    db.GrupoMaquina.Add(grupoMaq);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            

            return View(grupoMaq);
        }

        // GET: PlugAndPlay/GrupoMaqs/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrupoMaquina grupoMaq = db.GrupoMaquina.Find(id);
            if (grupoMaq == null)
            {
                return HttpNotFound();
            }
            return View(grupoMaq);
        }

        // POST: PlugAndPlay/GrupoMaqs/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descricao")] GrupoMaquina grupoMaq)
        {
            if (ModelState.IsValid)
            {
                db.Entry(grupoMaq).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(grupoMaq);
        }

        // GET: PlugAndPlay/GrupoMaqs/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrupoMaquina grupoMaq = db.GrupoMaquina.Find(id);
            if (grupoMaq == null)
            {
                return HttpNotFound();
            }
            return View(grupoMaq);
        }

        // POST: PlugAndPlay/GrupoMaqs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            GrupoMaquina grupoMaq = db.GrupoMaquina.Find(id);
            db.GrupoMaquina.Remove(grupoMaq);
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
