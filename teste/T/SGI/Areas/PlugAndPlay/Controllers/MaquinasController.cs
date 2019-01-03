using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PagedList;
using SGI.Areas.PlugAndPlay.Models;
using SGI.Context;
using SGI.Util;

namespace SGI.Areas.PlugAndPlay.Controllers
{
    [CustomAuthorize(Roles = "AdiminstradorTI, AdiminstradorPCP")]
    public class MaquinasController : Controller
    {
        private JSgi db = new JSgi();

        // GET: PlugAndPlay/Maquinas
        public ActionResult Index(string searchString, int? nPageSize, int? page)
        {
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            IPagedList<Maquina> maquina;
            if (searchString != null && searchString != "")
            {
                maquina = db.Maquina.Where(x => (x.Descricao).ToUpper().Contains(searchString.ToUpper())).OrderBy(x => x.Id).ToPagedList(_PageNumber, _PageSize);
            }
            else
            {
                maquina = db.Maquina.OrderBy(x => x.Id).ToPagedList(_PageNumber, _PageSize);
            }
            return View(maquina);
        }

        // GET: PlugAndPlay/Maquinas/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Maquina maquina = db.Maquina.Find(id);
            if (maquina == null)
            {
                return HttpNotFound();
            }
            return View(maquina);
        }

        // GET: PlugAndPlay/Maquinas/Create
        public ActionResult Create()
        {
            ViewBag.CalendarioId = new SelectList(db.Calendario, "Id", "Descricao") ;
            //ViewBag.GrupoMaquinaId = db.GrupoMaquina.ToList();
            return View();
        }

        // POST: PlugAndPlay/Maquinas/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Maquina maquina)
        {
            if (ModelState.IsValid)
            {
                db.Maquina.Add(maquina);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CalendarioId = new SelectList(db.Calendario, "Id", "Descricao");
            ViewBag.GrupoMaquinaId = db.GrupoMaquina.ToList();
            return View(maquina);
        }
        // GET: PlugAndPlay/Maquinas/Edit/5
        public ActionResult Edit(string id)
        {
            ViewBag.CalendarioId = new SelectList(db.Calendario, "Id", "Descricao");
            ViewBag.GrupoMaquinaId = db.GrupoMaquina.ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Maquina maquina = db.Maquina.Find(id);
            if (maquina == null)
            {
                return HttpNotFound();
            }
            return View(maquina);
        }

        // POST: PlugAndPlay/Maquinas/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Maquina maquina)
        {
            if (ModelState.IsValid)
            {
                if (maquina.CorSemafaro == null)
                    maquina.CorSemafaro = "";
                db.Entry(maquina).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CalendarioId = new SelectList(db.Calendario, "Id", "Descricao");
            ViewBag.GrupoMaquinaId = db.GrupoMaquina.ToList();
            return View(maquina);
        }

        // GET: PlugAndPlay/Maquinas/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Maquina maquina = db.Maquina.Find(id);
            if (maquina == null)
            {
                return HttpNotFound();
            }
            return View(maquina);
        }

        // POST: PlugAndPlay/Maquinas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Maquina maquina = db.Maquina.Find(id);
            db.Maquina.Remove(maquina);
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
        [HttpGet]
        public ActionResult RelatorioPerformace()
        {

            return View();
        }
        //[HttpPost]
        //public ActionResult GerarGraficos()
        //{
        //    List<object> dados = new List<object>();
        //    var maquinas = db.Maquina.Include(i => i.TargetsProduto).ToList();
        //    double percentual;
        //    foreach (var m in maquinas)
        //    {
        //        string opCorrente;
        //        double? meta;
        //        Order op;

        //        DateTime data = DateTime.Now.Subtract(new TimeSpan(0,5,0));
        //        var medicoes = db.ViewClpMedicoes.Where(x => x.MaquinaId == m.Id && x.DataFim >= data).ToList();
        //        List<object> pontos = new List<object>();
        //        opCorrente = medicoes[0].OrdemProducaoId;

        //        if (!string.IsNullOrEmpty(opCorrente))
        //        {
        //            op = db.Order.Find(opCorrente);//----------------------buscar
        //            meta = db.TargetProduto.Where(x => x.MaquinaId == m.Id && x.ProdutoId == op.ProdutoId).Select(x => x.ProximaMetaPerformace).FirstOrDefault();
        //        }
        //        else
        //        {
        //            meta = -1;
        //        }
        //        foreach (var med in medicoes)
        //        {
        //            if (opCorrente != med.OrdemProducaoId)
        //            {
        //                opCorrente = med.OrdemProducaoId;
        //                if (!string.IsNullOrEmpty(opCorrente))
        //                {
        //                    op = db.Order.Find(opCorrente);
        //                    meta = db.TargetProduto.Where(x => x.MaquinaId == m.Id && x.ProdutoId == op.ProdutoId).Select(x => x.ProximaMetaPerformace).FirstOrDefault();
        //                }
        //                else
        //                {
        //                    meta = -1;
        //                }
        //            }
        //            if (meta != -1)
        //            {
        //                double segundos = (med.DataFim - med.DataIni).Seconds;
        //                double qtdSeg = med.Quantidade / segundos;
        //                percentual = qtdSeg * 100 / (double)meta;
        //            }
        //            else
        //            {
        //                percentual = 0;
        //            }
        //            pontos.Add(new[] {percentual,(med.DataFim - new DateTime(1970, 01,01, 0, 0, 0)).Milliseconds});
        //        }
        //        dados.Add(new {
        //            pontos = pontos,
        //            maquina = m.Descricao
        //        });
        //    }
        //    return Json(dados);
        //}
        [HttpPost]
        public ActionResult AtualizarGraficos()
        {
            return Json(new { });
        }
    }
}
