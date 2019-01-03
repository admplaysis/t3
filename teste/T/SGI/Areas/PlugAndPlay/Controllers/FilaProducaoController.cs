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
    public class FilaProducaoController : Controller
    {
        private JSgi db = new JSgi();
        // GET: PlugAndPlay/FilaProducao
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult ObterFila() {
            var fila = db.VwFilaProducao.OrderBy(f => new {f.RotMaqId, f.FprDataInicioPrevista, f.RotSeqTransformacao }).ToList()
                .Select(f=>new {
                    maquina = f.RotMaqId,
                    maquinaId = f.RotMaqId,
                    produtoId = f.PaProId,
                    pedidoId = f.OrdId,
                    produto = f.PaProDescricao,
                    inicioPrevisto = f.FprDataInicioPrevista.ToString("dd/MM/yyyy HH:mm:ss"),
                    fimPrevisto = f.FprDataFimPrevista.ToString("dd/MM/yyyy HH:mm:ss"),
                    seqTransform = f.RotSeqTransformacao,
                    seqRepet = f.FprSeqRepeticao,
                    qtd = f.FprQuantidadePrevista }).ToList();
            return Json(fila, JsonRequestBehavior.AllowGet);
        }
        // GET: PlugAndPlay/FilaProducao/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilaProducao filaProducao = db.FilaProducao.Find(id);
            if (filaProducao == null)
            {
                return HttpNotFound();
            }
            return View(filaProducao);
        }
        [ChildActionOnly]
        public PartialViewResult Create()
        {
            ViewData["Roteiro.MaquinaId"] = new SelectList(db.Maquina, "Id", "Descricao");
            //ViewBag.MaquinaId = new SelectList(db.Maquina, "Id", "Descricao");
            return PartialView("_Create");
        }

        // POST: PlugAndPlay/FilaProducao/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FilaProducao filaProducao)
        {
            List<string> msg = new List<string>(); int maxSeqRep = 0; int ok = 1;

            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    if (db.Produto.Count(p => p.Id == filaProducao.Roteiro.ProdutoId) < 1)
                    {
                        ok = -1;
                        msg.Add("O produto informado não está cadastrado.");
                    }
                    if (db.Order.Count(o => o.Id == filaProducao.OrderId) < 1)
                    {
                        ok = -1;
                        msg.Add("O pedido informado não está cadastrado.");
                    }

                    filaProducao.MaquinaId = filaProducao.Roteiro.MaquinaId;
                    filaProducao.ProdutoId = filaProducao.Roteiro.ProdutoId;
                    filaProducao.SequenciaTransformacao = filaProducao.Roteiro.SequenciaTransformacao;

                    if (db.FilaProducao.Count(x => x.OrderId == filaProducao.OrderId
                      && x.ProdutoId == filaProducao.ProdutoId
                      && x.SequenciaTransformacao == filaProducao.SequenciaTransformacao
                      && x.SequenciaRepeticao == filaProducao.SequenciaRepeticao
                      && x.MaquinaId == filaProducao.MaquinaId) > 0)
                    {
                        ok = -2;
                        maxSeqRep = (int)db.FilaProducao.Where(x => x.OrderId == filaProducao.OrderId
                         && x.ProdutoId == filaProducao.ProdutoId
                         && x.SequenciaTransformacao == filaProducao.SequenciaTransformacao
                         && x.MaquinaId == filaProducao.MaquinaId).Max(f => f.SequenciaRepeticao);
                    }
                    else if (!ModelState.IsValid)
                        ok = -1;
                    if (ok == 1)
                    {
                        if (db.Roteiro.Count(r => r.MaquinaId == filaProducao.Roteiro.MaquinaId
                            && r.ProdutoId == filaProducao.Roteiro.ProdutoId
                            && r.SequenciaTransformacao == filaProducao.Roteiro.SequenciaTransformacao) > 0)
                        {
                          //  db.Roteiro.Attach(filaProducao.Roteiro);
                        }

                        filaProducao.Status = "AB";
                        db.FilaProducao.Add(filaProducao);
                        db.SaveChanges();
                        db.Database.ExecuteSqlCommand("SP_PLUG_CALCULA_FILAS " + filaProducao.MaquinaId);
                    }
                    tran.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    ok = -1;
                }
            }

            return Json(new { ok = ok, msg = msg, maxSeq = maxSeqRep });
        }

        // GET: PlugAndPlay/FilaProducao/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilaProducao filaProducao = db.FilaProducao.Find(id);
            if (filaProducao == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaquinaId = new SelectList(db.Maquina, "Id", "Descricao", filaProducao.MaquinaId);
            ViewBag.OrderId = new SelectList(db.Order, "Id", "Tipo", filaProducao.OrderId);
            ViewBag.ProdutoId = new SelectList(db.Produto, "Id", "Descricao", filaProducao.ProdutoId);
            return View(filaProducao);
        }

        // POST: PlugAndPlay/FilaProducao/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult Edit(FilaProducao filaProducao)
        {
            bool ok = true;
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        //db.FilaProducao.Attach(filaProducao);
                        //var entry =  db.Entry(filaProducao);
                        //entry.Property(x => x.DataInicioPrevista).IsModified = true;
                        //entry.Property(x => x.DataFimMaxima).IsModified = true;
                        var fila = db.FilaProducao.Where(f => f.OrderId == filaProducao.OrderId
                                    && f.MaquinaId == filaProducao.MaquinaId
                                    && f.ProdutoId == filaProducao.ProdutoId
                                    && f.SequenciaTransformacao == filaProducao.SequenciaTransformacao
                                    && f.SequenciaRepeticao == filaProducao.SequenciaRepeticao).FirstOrDefault();
                        fila.DataInicioPrevista = filaProducao.DataInicioPrevista;
                        fila.DataFimPrevista = filaProducao.DataFimPrevista;
                        db.SaveChanges();
                        db.Database.ExecuteSqlCommand(" SP_PLUG_CALCULA_FILAS '" + filaProducao.MaquinaId + "'");
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        ok = false;
                    }
                }
            }
            else
                ok = false;

            return Json(ok);
        }

        // GET: PlugAndPlay/FilaProducao/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FilaProducao filaProducao = db.FilaProducao.Find(id);
            if (filaProducao == null)
            {
                return HttpNotFound();
            }
            return View(filaProducao);
        }

        // POST: PlugAndPlay/FilaProducao/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string ord, string prod, int seqTran, int seqRep, string maq)
        {
            FilaProducao filaProducao = db.FilaProducao.Where(x => x.OrderId == ord && x.ProdutoId == prod && x.SequenciaTransformacao == seqTran && x.SequenciaRepeticao == seqRep).FirstOrDefault();
            db.FilaProducao.Remove(filaProducao);
            db.SaveChanges();
            return Json(true);
        }
        //ajax fuctions
        [HttpPost]
        public JsonResult BuscarMaquinas(string proId)
        {
            var maquinas = db.Roteiro.Include(r => r.Maquina).Where(r => r.ProdutoId == proId).Select(r => new { descricao = r.Maquina.Descricao, id = r.Maquina.Id }).Distinct();
            return Json(maquinas);
        }
        [HttpPost]
        public JsonResult BuscarSequenciasTranformacao(string maqId, string proId)
        {
            var seqsTransf = db.Roteiro.Where(r => r.MaquinaId == maqId && r.ProdutoId == proId).Select(r => new { SequanciaTransformacao = r.SequenciaTransformacao }).Distinct();
            return Json(seqsTransf);
        }
        [HttpPost]
        public JsonResult ObterRoteiroProduto(string pedidoId)
        {
            object roteiros = new List<object>();
            var pedido = db.Order.Find(pedidoId);
            if (pedido != null)
            {
                roteiros = (from e in db.EstruturaProduto
                            join r in db.Roteiro on e.ProdutoPaiId equals r.ProdutoId
                            where e.ProdutoPaiId == pedido.ProdutoId
                            select new
                            {
                                maquinaId = r.MaquinaId,
                                produtoId = r.ProdutoId,
                                sequenciaTransformacao = r.SequenciaTransformacao,
                                pecasPulso = r.PecasPorPulso
                            }).Union(from e in db.EstruturaProduto
                                     join r in db.Roteiro on e.ProdutoFilhoId equals r.ProdutoId
                                     where e.ProdutoPaiId == pedido.ProdutoId
                                     select new
                                     {
                                         maquinaId = r.MaquinaId,
                                         produtoId = r.ProdutoId,
                                         sequenciaTransformacao = r.SequenciaTransformacao,
                                         pecasPulso = r.PecasPorPulso
                                     }).OrderBy(x => x.sequenciaTransformacao).ToList();
            }

            return Json(roteiros);
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
