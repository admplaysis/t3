using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SGI.Areas.PlugAndPlay.Models;
using SGI.Context;
using SGI.Util;

namespace SGI.Areas.PlugAndPlay.Controllers
{
    [CustomAuthorize(Roles = "AdiminstradorTI, AdiminstradorPCP")]
    public class ProdutoController : Controller
    {
        private JSgi db = new JSgi();

        // GET: PlugAndPlay/Produto
        public ActionResult Index(string searchString, int? nPageSize, int? page)
        {
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            

            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            IPagedList<Produto> produto;
            if (searchString != null && searchString != "")
            {
                produto = db.Produto.Include(p => p.UnidadeMedida).Where(x => (x.Descricao).ToUpper().Contains(searchString.ToUpper())).OrderBy(x => x.Id).ToPagedList(_PageNumber, _PageSize);
            }
            else
            {
                produto = db.Produto.Include(p => p.UnidadeMedida).OrderBy(x => x.Id).ToPagedList(_PageNumber, _PageSize);
            }
            return View(produto);
        }

        // GET: PlugAndPlay/Produto/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produto produto = db.Produto.Find(id);
            if (produto == null)
            {
                return HttpNotFound();
            }
            ViewBag.UnidadeMedidaId = new SelectList(db.UnidadeMedida, "Id", "Descricao", produto.UnidadeMedidaId);
            return View(produto);
        }

        // GET: PlugAndPlay/Produto/Create
        public ActionResult Create()
        {

            ViewBag.UnidadeMedidaId = new SelectList(db.UnidadeMedida, "Id", "Descricao");
            return View();
        }

        // POST: PlugAndPlay/Produto/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descricao,Estoque,UnidadeMedidaId,PecasPorFardo,FardosPorPalet,TPIdentificador")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                db.Produto.Add(produto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UnidadeMedidaId = new SelectList(db.UnidadeMedida, "Id", "Descricao", produto.UnidadeMedidaId);
            return View(produto);
        }

        // GET: PlugAndPlay/Produto/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produto produto = db.Produto.Find(id);
            if (produto == null)
            {
                return HttpNotFound();
            }
            ViewBag.UnidadeMedidaId = new SelectList(db.UnidadeMedida, "Id", "Descricao", produto.UnidadeMedidaId);
            return View(produto);
        }

        // POST: PlugAndPlay/Produto/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descricao,Estoque,UnidadeMedidaId,PecasPorFardo,FardosPorPalet,TPIdentificador")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(produto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UnidadeMedidaId = new SelectList(db.UnidadeMedida, "Id", "Descricao", produto.UnidadeMedidaId);
            return View(produto);
        }

        // GET: PlugAndPlay/Produto/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produto produto = db.Produto.Find(id);
            if (produto == null)
            {
                return HttpNotFound();
            }
            return View(produto);
        }

        // POST: PlugAndPlay/Produto/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Produto produto = db.Produto.Find(id);
            db.Produto.Remove(produto);
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
