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
using SGI.ViewModels.JqueryDataTable;

namespace SGI.Areas.PlugAndPlay.Controllers
{
    [CustomAuthorize(Roles = "AdiminstradorTI, AdiminstradorPCP")]
    public class OrdersController : Controller
    {
        private JSgi db = new JSgi();

        // GET: PlugAndPlay/Orders
        public ActionResult Index(string searchString, int? nPageSize, int? page)
        {
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            IPagedList<Order> orders;
            if (searchString != null && searchString != "")
            {
                orders = db.Order.Include(o => o.Cliente).Where(x => (x.Id).ToUpper().Contains(searchString.ToUpper())).OrderBy(x => x.Id).ToPagedList(_PageNumber, _PageSize);
            }
            else
            {
                orders = db.Order.Include(o => o.Cliente).OrderBy(x => x.Id).ToPagedList(_PageNumber, _PageSize);
            }
            return View(orders);
        }

        // GET: PlugAndPlay/Orders/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: PlugAndPlay/Orders/Create
        public ActionResult Create(string url)
        {
            ViewBag.url = url != null ? HttpUtility.UrlDecode(url) : Url.Action("Index");
            TempData["url"] = url != null ? HttpUtility.UrlDecode(url) : Url.Action("Index");

            ViewBag.ClienteId = new SelectList(db.Clientes, "Id", "Nome");
            ViewBag.ProdutoId = new SelectList(db.Produto, "Id", "Descricao");
            return View();
        }

        // POST: PlugAndPlay/Orders/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DataEntregaDe,DataEntregaAte,Quantidade,PrecoUnitario,Tipo,QuantidadeProgramada,ToleranciaMais,ToleranciaMenos,ClienteId,ProdutoId")] Order order)
        {
            string url = TempData["url"] != null ? TempData["url"].ToString() : Url.Action("Index");
            ViewBag.url = url;

            if (ModelState.IsValid)
            {
                db.Order.Add(order);
                db.SaveChanges();
                return Redirect(url);
            }
            else
            {
                TempData["url"] = url;
            }
            ViewBag.ClienteId = new SelectList(db.Clientes, "Id", "Nome", order.ClienteId);
            ViewBag.ProdutoId = new SelectList(db.Produto, "Id", "Descricao", order.ProdutoId);
            return View(order);
        }

        // GET: PlugAndPlay/Orders/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClienteId = new SelectList(db.Clientes, "Id", "Nome", order.ClienteId);
            ViewBag.ProdutoId = new SelectList(db.Produto, "Id", "Descricao", order.ProdutoId);
            return View(order);
        }

        // POST: PlugAndPlay/Orders/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DataEntregaDe,DataEntregaAte,Quantidade,PrecoUnitario,Tipo,QuantidadeProgramada,ToleranciaMais,ToleranciaMenos,ClienteId,ProdutoId")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClienteId = new SelectList(db.Clientes, "Id", "Nome", order.ClienteId);
            ViewBag.ProdutoId = new SelectList(db.Produto, "Id", "Descricao", order.ProdutoId);
            return View(order);
        }

        // GET: PlugAndPlay/Orders/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: PlugAndPlay/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Order order = db.Order.Find(id);
            db.Order.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //ajax functions
        [HttpPost]
        public JsonResult GetOrders(JQDTParams param)
        {
            var orders = from x in db.Order select x;
            var nonfilteredcount = orders.Count();
            //filter 
            //-------------------------------------------------------------------
            //orders = orders.Where(x => x.Id.ToLower().Contains(param.search.value));
            //result
            //-------------------------------------------------------------------
            var filteredCount = orders.Count();
            orders = orders.OrderBy(o=> o.DataEntregaDe).Skip(param.start).Take(param.length);
            var data = orders.Include(o=>o.Cliente).Include(o=>o.Produto).ToList().Select(o => new[] {
                o.Id,
                o.Cliente.Nome,
                o.Produto.Descricao,
                o.DataEntregaDe.ToShortDateString(),
                o.DataEntregaAte.ToShortTimeString(),
                o.Quantidade.ToString(),
                o.PrecoUnitario.ToString(),
                o.Tipo,
                o.ToleranciaMais.ToString(),
                o.ToleranciaMenos.ToString()
            }).ToList();
            return Json(new
            {
                draw = param.draw,
                recordsTotal = nonfilteredcount,
                recordsFiltered = filteredCount,
                data = data
            }, JsonRequestBehavior.AllowGet);
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
