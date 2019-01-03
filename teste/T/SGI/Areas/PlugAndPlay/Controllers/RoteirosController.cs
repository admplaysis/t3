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
    public class RoteirosController : Controller
    {
        private JSgi db = new JSgi();

        // GET: PlugAndPlay/Roteiros
        public ActionResult Index()
        {
            var roteiro = db.Roteiro.Include(r => r.Maquina).Include(r => r.Produto);
            return View(roteiro.ToList());
        }

        // GET: PlugAndPlay/Roteiros/Details/5
        public ActionResult Details(string maq, string prod, int seq)
        {
            if (maq == null || prod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Roteiro roteiro = db.Roteiro.Where(r => r.ProdutoId == prod && r.MaquinaId == maq && r.SequenciaTransformacao == seq).FirstOrDefault();
            if (roteiro == null)
            {
                return HttpNotFound();
            }
            return View(roteiro);
        }

        // GET: PlugAndPlay/Roteiros/Create
        public ActionResult Create(string url)
        {
            ViewBag.url = url  != null ? HttpUtility.UrlDecode(url) : Url.Action("Index");
            TempData["url"] = url != null ? HttpUtility.UrlDecode(url) : Url.Action("Index");

            ViewBag.MaquinaId = new SelectList(db.Maquina, "Id", "Descricao");
            ViewBag.ProdutoId = new SelectList(db.Produto, "Id", "Descricao");
            return View();
        }

        // POST: PlugAndPlay/Roteiros/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaquinaId,ProdutoId,SequenciaTransformacao,Padrao,ConsideraGrupoMaquina,PecasPorPulso")] Roteiro roteiro)
        {
            string url = TempData["url"] != null ? TempData["url"].ToString() : Url.Action("Index");
            ViewBag.url = url;

            if (ModelState.IsValid)
            {
                db.Roteiro.Add(roteiro);
                db.SaveChanges();
                return Redirect(url);
            }
            else
            {
                TempData["url"] = url;
            }
            ViewBag.MaquinaId = new SelectList(db.Maquina, "Id", "Descricao", roteiro.MaquinaId);
            ViewBag.ProdutoId = new SelectList(db.Produto, "Id", "Descricao", roteiro.ProdutoId);
            return View(roteiro);
        }

        // GET: PlugAndPlay/Roteiros/Edit/5
        public ActionResult Edit(string maq, string prod, int seq)
        {
            if (maq == null || prod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Roteiro roteiro = db.Roteiro.Where(r=>r.ProdutoId == prod && r.MaquinaId == maq && r.SequenciaTransformacao == seq).FirstOrDefault();
            if (roteiro == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaquinaId = new SelectList(db.Maquina, "Id", "Descricao", roteiro.MaquinaId);
            ViewBag.ProdutoId = new SelectList(db.Produto, "Id", "Descricao", roteiro.ProdutoId);
            return View(roteiro);
        }

        // POST: PlugAndPlay/Roteiros/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaquinaId,ProdutoId,SequenciaTransformacao,Padrao,ConsideraGrupoMaquina,PecasPorPulso")] Roteiro roteiro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roteiro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaquinaId = new SelectList(db.Maquina, "Id", "Descricao", roteiro.MaquinaId);
            ViewBag.ProdutoId = new SelectList(db.Produto, "Id", "Descricao", roteiro.ProdutoId);
            return View(roteiro);
        }

        // GET: PlugAndPlay/Roteiros/Delete/5
        public ActionResult Delete(string maq, string prod, int seq)
        {
            if (maq == null || prod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Roteiro roteiro = db.Roteiro.Where(r => r.ProdutoId == prod && r.MaquinaId == maq && r.SequenciaTransformacao == seq).FirstOrDefault();
            if (roteiro == null)
            {
                return HttpNotFound();
            }
            return View(roteiro);
        }

        // POST: PlugAndPlay/Roteiros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string maq, string prod, int seq)
        {
            Roteiro roteiro = db.Roteiro.Where(r => r.ProdutoId == prod && r.MaquinaId == maq && r.SequenciaTransformacao == seq).FirstOrDefault();
            db.Roteiro.Remove(roteiro);
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
