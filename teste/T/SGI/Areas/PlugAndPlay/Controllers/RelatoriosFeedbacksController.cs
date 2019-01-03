using SGI.Areas.PlugAndPlay.Models;
using SGI.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using SGI.ViewModels.JqueryDataTable;

namespace SGI.Areas.PlugAndPlay.Controllers
{
    public class RelatoriosFeedbacksController : Controller
    {
        // GET: PlugAndPlay/DesempenhoProducao
        public ActionResult FeedbacksDesempenho(string maquina, string strDataDe, string strDataAte, string tipo, string status)
        {
            DateTime? dataDe = !string.IsNullOrEmpty(strDataDe) ? (DateTime?)DateTime.ParseExact(strDataDe, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture) : null;
            DateTime? dataAte = !string.IsNullOrEmpty(strDataAte) ? (DateTime?)DateTime.ParseExact(strDataAte, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture) : null;
            using (var db = new JSgi())
            {
                ViewBag.maquinaId = new SelectList(db.Maquina.ToList(), "Id", "Descricao", maquina);
                ViewBag.dataDe = dataDe == null ? DateTime.Now.ToString("dd/MM/yyyy") : ((DateTime)dataDe).ToString("dd/MM/yyyy");
                ViewBag.dataAte = dataAte == null ? DateTime.Now.ToString("dd/MM/yyyy") : ((DateTime)dataAte).ToString("dd/MM/yyyy");
                ViewBag.tipo = new SelectList(new[] {
                    new { id = "T", desc = "Todos"},
                    new { id = "P", desc = "Performance" },
                    new { id = "S", desc = "Setup" },
                    new { id = "SA", desc = "Setup Ajuste" },
                    new { id = "SS", desc = "Setup e Setup Ajuste" }
                }, "id", "desc", tipo);
                ViewBag.status = new SelectList(new[] {
                    new { id = "T", desc = "Todos"},
                    new { id = "G", desc = "Bom" },
                    new { id = "B", desc = "Ruim" }
                }, "Id", "desc", status);
            }
            return View();
        }
        [HttpPost]
        public ActionResult ObterFeedbacksDeDesenpenho(string maquina, DateTime dataDe, DateTime dataAte,
            string tipo, string status)
        {

            List<string[]> jsonReturn = null;

            using (var db = new JSgi())
            {
                IQueryable<V_MOTIVO_FEEDBACKS_DESEMPENHO> query = db.Set<V_MOTIVO_FEEDBACKS_DESEMPENHO>();

                if (!string.IsNullOrEmpty(maquina))
                    query = query.Where(t => t.MAQ_ID == maquina);
                if (dataDe != null)
                    query = query.Where(t => t.TAR_DIA_TURMA_D >= dataDe.Date);
                if (dataAte != null)
                    query = query.Where(t => t.TAR_DIA_TURMA_D <= dataAte.Date);

                string filtro = "";
                if (status != "" && tipo != "" && tipo != "SS")
                {
                    if (tipo == "T" && status == "T")
                        filtro = "";
                    else if (tipo == "T" && status != "T")
                        filtro = status;
                    else if (status == "T" && tipo != "T")
                        filtro = tipo;
                    else if (tipo != "T" && status != "T")
                        filtro = status + tipo;
                }
                if (tipo == "SS")
                {
                    if (status == "B")
                            query = query.Where(t => (new string[] { "BSA", "BS" }).Contains(t.TIPO) );
                    if (status == "G")
                            query = query.Where(t => (new string[] { "GSA", "GS" }).Contains(t.TIPO) );
                    if (status != "B" && status != "G")
                            query = query.Where(t => (new string[] { "BSA", "BS", "GSA", "GS" }).Contains(t.TIPO));
                }
                if (filtro != "")
                {
                    if (filtro == "P" || filtro == "S" || filtro == "SA")
                        query = query.Where(t => System.Data.Entity.DbFunctions.Right(t.TIPO_ID, filtro.Length) == filtro);
                    else if(filtro == "GS" || filtro == "BS")
                        query = query.Where(t => t.TIPO_ID == filtro);
                    else
                        query = query.Where(t => System.Data.Entity.DbFunctions.Left(t.TIPO_ID, filtro.Length) == filtro);
                }
                jsonReturn = query
                   .OrderBy(t => t.TAR_DIA_TURMA_D).ToList().Select(t => new[] {
                     //t.ID.ToString(),
                     t.TIPO.ToString(),
                     t.TAR_DIA_TURMA.ToString(),
                     t.OCO_ID_PERFORMANCE.ToString(),
                     t.OCO_DESCRICAO,
                     t.TAR_OBS_PERFORMANCE,
                     t.NOME,
                     t.MAQ_ID,
                     t.ORD_ID,
                     t.PRO_ID,
                     t.FPR_SEQ_REPETICAO.ToString(),
                     t.ROT_SEQ_TRANFORMACAO.ToString(),
                     t.TURM_ID,
                     t.TURN_ID,
                     t.TAR_DIA_TURMA_D != null ? ((DateTime)t.TAR_DIA_TURMA_D).ToString("dd/MM/yyyy") : ""
               }).ToList();
            }
            return Json(new
            {
                data = jsonReturn
            }, JsonRequestBehavior.AllowGet);
        }
    }
}