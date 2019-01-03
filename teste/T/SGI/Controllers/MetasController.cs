using SGI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Globalization;
using SGI.Context;
using SGI.Util;

namespace SGI.Controllers
{
    [CustomAuthorize(Roles = "AdiminstradorTI, AdiminstradorPCP")]
    public class MetasController : Controller
    {
        //
        // GET: /Metas/
        private JSgi db = new JSgi();

        public ActionResult Index(string searchString, int? nPageSize, int? page)
        {
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            var metas = db.T_Metas.OrderByDescending(x => x.MET_DTINICIO).ToList();

            //Valida Pesquisa
            if (searchString != null && searchString != "")
                metas = metas.Where(x => (x.T_Indicadores.IND_DESCRICAO + x.MET_ALVO + x.MET_DTINICIO.ToString() + x.MET_DTFIM.ToString()).ToUpper().Contains(searchString.ToUpper())).OrderByDescending(y => y.MET_DTFIM).ToList();

            return View(metas.OrderBy(x => x.MET_DTINICIO).ToPagedList(_PageNumber, _PageSize));
        }

        public ActionResult Details(int id)
        {
            var meta = db.T_Metas.Find(id);
            return View(meta);
        }

        public ActionResult Create()
        {
            var meta = new T_Metas();
            meta.MET_DTINICIO = DateTime.Now.ToString("dd/MM/yyyy");
            meta.MET_DTFIM = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.T_Indicadores_NEG_ID = new SelectList(db.T_Negocio, "NEG_ID", "NEG_DESCRICAO");
            ViewBag.IND_ID = new SelectList(new List<T_Indicadores>(), "IND_ID", "IND_DESCRICAO");
            return View(meta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(T_Metas meta)
        {
            Validacoes(meta);
            if (ModelState.IsValid)
            {
                meta.MET_DTINICIO = DateTime.ParseExact(meta.MET_DTINICIO, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd");
                meta.MET_DTFIM = DateTime.ParseExact(meta.MET_DTFIM, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd");

                db.T_Metas.Add(meta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (Request["T_Indicadores_NEG_ID"] != "")
            {
                int? idNegocio = Int32.Parse(Request["T_Indicadores_NEG_ID"]);
                ViewBag.IND_ID = new SelectList(db.T_Indicadores.Where(x => x.NEG_ID == idNegocio).ToList(), "IND_ID", "IND_DESCRICAO", meta.IND_ID);
                ViewBag.T_Indicadores_NEG_ID = new SelectList(db.T_Negocio, "NEG_ID", "NEG_DESCRICAO", Int32.Parse(Request["T_Indicadores_NEG_ID"]));
            }
            else
            {
                ViewBag.IND_ID = new SelectList(new List<T_Indicadores>(), "IND_ID", "IND_DESCRICAO");
                ViewBag.T_Indicadores_NEG_ID = new SelectList(db.T_Negocio, "NEG_ID", "NEG_DESCRICAO");
            }
            return View(meta);
        }

        public ActionResult Edit(int id)
        {
            var meta = db.T_Metas.Find(id);
            meta.MET_DTINICIO = DateTime.ParseExact(meta.MET_DTINICIO, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
            meta.MET_DTFIM = DateTime.ParseExact(meta.MET_DTFIM, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
            return View(meta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(T_Metas meta)
        {
            Validacoes(meta);
            if (ModelState.IsValid)
            {
                meta.MET_DTINICIO = DateTime.ParseExact(meta.MET_DTINICIO, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd");
                meta.MET_DTFIM = DateTime.ParseExact(meta.MET_DTFIM, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd");
                db.Entry(meta).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            meta.T_Indicadores = db.T_Indicadores.Find(meta.IND_ID);
            return View(meta);
        }

        public void Validacoes(T_Metas metas)
        {
            /*VALIDACAO DE DATA INICIO*/
            if (metas.MET_DTINICIO == null || metas.MET_DTINICIO.Trim().Equals(""))
            {
                ModelState.Remove("MET_DTINICIO");
                ModelState.AddModelError("MET_DTINICIO", "Data de início não informada.");
            }

            /*VALIDACAO DE DATA FIM*/
            if (metas.MET_DTFIM == null || metas.MET_DTFIM.Trim().Equals(""))
            {
                ModelState.Remove("MET_DTFIM");
                ModelState.AddModelError("MET_DTFIM", "Data de término não informada.");
            }
            else
            {
                if (metas.MET_DTINICIO != null && metas.MET_DTFIM != null)
                {


                    #region ValidaDataExistente
                    var MET_DTINICIO = DateTime.ParseExact(metas.MET_DTINICIO, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd");
                    var MET_DTFIM = DateTime.ParseExact(metas.MET_DTFIM, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd");
                    var sql = "select * from T_Metas ";
                    sql += "where IND_ID = '" + metas.IND_ID + "' AND (('" + MET_DTINICIO + "' BETWEEN MET_DTINICIO AND MET_DTFIM) or ('" + MET_DTFIM + "' BETWEEN MET_DTINICIO AND MET_DTFIM)) ";
                    if (metas.MET_ID > 0)
                        sql += "AND MET_ID != '" + metas.MET_ID + "' ";

                    var listaMetas = db.Database.SqlQuery<T_Metas>(sql).ToList();
                    //Valida se existe data nesse periodo
                    if (listaMetas.Count() > 0)
                    {
                        ModelState.Remove("MET_DTINICIO");
                        ModelState.AddModelError("MET_DTINICIO", "Já existe meta cadastrada para este periódo.");
                    }
                    #endregion ValidaDataExistente

                    //Valida data final menor que a inicial
                    if (Convert.ToDateTime(metas.MET_DTFIM) < Convert.ToDateTime(metas.MET_DTINICIO))
                    {
                        ModelState.Remove("MET_DTFIM");
                        ModelState.AddModelError("MET_DTFIM", "Data de término é menor que a data inicial.");
                    }
                }


                if (metas.MET_DTINICIO == null || metas.IND_ID <= 0)
                {
                    ModelState.Remove("IND_ID");
                    ModelState.AddModelError("IND_ID", "Selecione um indicador.");
                }
            }

         

        }

        public ActionResult Delete(int id)
        {
            var meta = db.T_Metas.Find(id);
            return View(meta);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            T_Metas meta = db.T_Metas.Find(id);
            #region Validacoes
            if (db.T_Informacoes_Complementares.Count(x => x.MET_ID == id) > 0)
            {
                ModelState.Remove("indNome");
                ModelState.AddModelError("indNome", "Não é possível excluir a meta, a mesma possui informações complementares.");
            }

            if (db.T_Medicoes.Count(x => x.MET_ID == id) > 0)
            {
                ModelState.Remove("indNome");
                ModelState.AddModelError("indNome", "Não é possível excluir a meta, a mesma possui medições cadastradas.");
            }
            #endregion Validacoes
            if (ModelState.IsValid)
            {
                db.T_Metas.Remove(meta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(meta);
        }

        #region Medicoes
        public ActionResult Medicao(int id, string searchString, int? nPageSize, int? page)
        {
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            var medicoes = db.T_Medicoes.Where(x => x.MET_ID == id).ToList();
            return View(medicoes.OrderByDescending(x => x.MED_DATAMEDICAO).ToPagedList(_PageNumber, _PageSize));
        }

        public ActionResult AddMedicao(int id_meta)
        {
            var medicoesManual = new T_Medicoes();
            medicoesManual.MET_ID = id_meta;
            medicoesManual.T_Metas = db.T_Metas.Find(id_meta);
            medicoesManual.MED_DATA = DateTime.Now;
            medicoesManual.MED_DATAMEDICAO = DateTime.Now.ToString();
            ViewBag.UNI_ID = new SelectList(new List<T_Unidade>(), "UNI_ID", "UN");
            return View(medicoesManual);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMedicao(T_Medicoes medicao)
        {
            #region Validacoes
            if (medicao.MED_VALOR == null || medicao.MED_VALOR == "")
                ModelState.AddModelError("MED_VALOR", "Campo obrigatório não informado!");

            if (medicao.MED_DATAMEDICAO == null || medicao.MED_DATAMEDICAO == "")
                ModelState.AddModelError("MED_DATAMEDICAO", "Data medição inválido!");
            #endregion Validacoes

            if (ModelState.IsValid)
            {
                medicao.MED_PONDERACAO = 0;
                medicao.MED_DATAMEDICAO = Convert.ToDateTime(medicao.MED_DATAMEDICAO).ToString("yyyyMMdd");
                medicao.MED_VALOR = medicao.MED_VALOR.Replace(",", "");
                db.T_Medicoes.Add(medicao);
                db.SaveChanges();
                return RedirectToAction("Medicao", new { id = medicao.MET_ID });
            }
            medicao.T_Metas = db.T_Metas.Find(medicao.MET_ID);
            ViewBag.UNI_ID = new SelectList(new List<T_Unidade>(), "UNI_ID", "UN", medicao.UNI_ID);
            return View(medicao);
        }

        public ActionResult EdtMedicao(int id)
        {
            var medicoesManual = db.T_Medicoes.Find(id);
            ViewBag.UNI_ID = new SelectList(new List<T_Unidade>(), "UNI_ID", "UN", medicoesManual.UNI_ID);
            return View(medicoesManual);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EdtMedicao(T_Medicoes medicao)
        {
            #region Validacoes
            if (medicao.MED_VALOR == null || medicao.MED_VALOR == "")
                ModelState.AddModelError("MED_VALOR", "Campo obrigatório não informado!");

            if (medicao.MED_DATAMEDICAO == null || medicao.MED_DATAMEDICAO == "")
                ModelState.AddModelError("MED_DATAMEDICAO", "Data medição inválido!");
            #endregion Validacoes

            if (ModelState.IsValid)
            {
                medicao.MED_PONDERACAO = 0;
                medicao.MED_DATAMEDICAO = Convert.ToDateTime(medicao.MED_DATAMEDICAO).ToString("yyyyMMdd");
                medicao.MED_VALOR = medicao.MED_VALOR.Replace(",", "");
                db.Entry(medicao).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Medicao", new { id = medicao.MET_ID });
            }
            ViewBag.UNI_ID = new SelectList(new List<T_Unidade>(), "UNI_ID", "UN", medicao.UNI_ID);
            medicao.T_Metas = db.T_Metas.Find(medicao.MET_ID);
            return View(medicao);
        }

        public ActionResult DelMedicao(int id)
        {
            var medicoes = db.T_Medicoes.Find(id);
            return View(medicoes);
        }

        [HttpPost, ActionName("DelMedicao")]
        [ValidateAntiForgeryToken]
        public ActionResult DelMedicaoConfirmed(int id)
        {
            T_Medicoes medicoes = db.T_Medicoes.Find(id);
            db.T_Medicoes.Remove(medicoes);
            db.SaveChanges();
            return RedirectToAction("Medicao", new { });
        }
        #endregion Medicoes

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public JsonResult GetIndicadores(int id_negocio)
        {
            return this.Json(new SelectList(db.T_Indicadores.Where(x => x.NEG_ID == id_negocio), "IND_ID", "IND_DESCRICAO"));
        }
    }
}