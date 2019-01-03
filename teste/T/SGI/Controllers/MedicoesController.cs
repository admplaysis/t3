using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SGI.Models;
using SGI.Context;
using SGI.Util;

namespace SGI.Controllers
{
    [CustomAuthorize(Roles = "AdiminstradorTI, AdiminstradorPCP")]
    public class MedicoesController : Controller
    {
        //
        // GET: /Medicoes/
        private JSgi db = new JSgi();

        public ActionResult Index(string searchString, int? nPageSize, int? page, int? idNegocio, int? idGrupo,int? idUnidade,int? idDepartamento, string pAno)
        {
            #region Variaveis
            var grafico = new ViewMedicoes();
            #endregion Variaveis
            #region ViewBags
            ViewBag.idNegocio = new SelectList(db.T_Negocio, "NEG_ID", "NEG_DESCRICAO", idNegocio);
            ViewBag.idGrupo = new SelectList(db.T_Grupo, "GRU_ID", "NOME", idGrupo);
            ViewBag.idUnidade = new SelectList(db.T_Grupo, "UNI_ID", "UN", idUnidade);
            var anos = db.T_Medicoes.Select(x => new { Ano = x.MED_DATAMEDICAO.Substring(0, 4) }).GroupBy(x => new { x.Ano }).OrderByDescending(x => x.Key.Ano).ToList();
            pAno = (pAno == "" || pAno == null ? anos.FirstOrDefault().Key.Ano : pAno);
            ViewBag.anoAtual = pAno;
            ViewBag.negAtual = idNegocio;
            ViewBag.grpAtual = idGrupo;
            //Calcula ano anterior
            DateTime anoAtu = DateTime.Parse("01/01/" + ViewBag.anoAtual);
            ViewBag.anoAnterior = anoAtu.AddYears(-1).Year.ToString();
            ViewBag.pAno = new SelectList(anos, "Key.Ano", "Key.Ano", pAno);
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            #endregion ViewBags
            //Chama metódo para trazer dados agrupados por indicadores e metas
            var indicadores = Util.ExtratorMedidor.GetIndicadores(searchString, nPageSize, page, idNegocio, idGrupo, idUnidade, idDepartamento, pAno);

            //Chama metódo para poder buscar dados de medições
            grafico.Medicoes = Util.ExtratorMedidor.GetMedicao(indicadores.ToList(), pAno);

            //Busca Ano Anterior
            grafico.AnoAnterior = new List<vw_SGI_PARAMETRO_RELMEDICOES>();
            grafico.AnoAnterior = Util.QueryAnaliser.AnoAnterior(pAno); ;

            //Busca lista de indicadores
            grafico.Indicadores = indicadores;
            return View(grafico);
        }

        public ActionResult PorMes(string periodo, int id_meta, int id_indicador)
        {
            var viewMedicoes = new ViewMedicoes();
            viewMedicoes.Medicoes = db.T_Medicoes.Where(x => x.MET_ID == id_meta).ToList();
            viewMedicoes.Metas = db.T_Metas.Where(x => x.IND_ID == id_indicador).ToList();
            //viewMedicoes.SP_SGI_MEDICOES_MES_Result = db.SP_SGI_MEDICOES_MES(periodo, id_indicador).OrderBy(x => x.Mes).ToList();
            ViewBag.diasMes = DateTime.DaysInMonth(Int32.Parse(periodo.Substring(0, 4)), Int32.Parse(periodo.Substring(4, 2)));
            ViewBag.anoMes = periodo.Substring(0, 6);
            ViewBag.dataParametro = new DateTime(Int32.Parse(periodo.Substring(0, 4)), Int32.Parse(periodo.Substring(4, 2)), 1);
            viewMedicoes.Indicador = db.T_Indicadores.Find(id_indicador);
            return View(viewMedicoes);
        }

        #region PlanosAcao
        public ActionResult PlanoAcao(int idIndicador, string periodo)
        {
            var planosAcao = db.T_PlanoAcao.Where(x => x.T_Metas.IND_ID == idIndicador).ToList();
            if (periodo != "")
                planosAcao = planosAcao.Where(x => x.PLA_REFERENCIA.Substring(0, periodo.Length) == periodo).ToList();
            return View(planosAcao.OrderBy(x => x.PLA_DATA).ToList());
        }

        public ActionResult AddPlano(int idIndicador, string periodo)
        {
            var plano = new T_PlanoAcao();
            plano.PLA_DATA = DateTime.Now;
            plano.PLA_REFERENCIA = periodo;
            plano.PLA_STATUS = "P";
            plano.T_Metas = db.T_Metas.Where(x => x.IND_ID == idIndicador).First();
            plano.MET_ID = plano.T_Metas.MET_ID;
            return View(plano);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPlano(T_PlanoAcao plano)
        {
            if (plano.PLA_DESCRICAO == "" || plano.PLA_DESCRICAO == null)
                ModelState.AddModelError("PLA_DESCRICAO", "Obrigatório informar a descrição.");

            if (ModelState.IsValid)
            {
                db.T_PlanoAcao.Add(plano);
                db.SaveChanges();
                plano.T_Metas = db.T_Metas.Find(plano.MET_ID);
                return RedirectToAction("PlanoAcao", new { idIndicador = plano.T_Metas.IND_ID, periodo = plano.PLA_REFERENCIA });
            }
            plano.T_Metas = db.T_Metas.Find(plano.MET_ID);
            return View(plano);
        }

        public ActionResult Edit(int id)
        {
            var plano = db.T_PlanoAcao.Find(id);
            return View(plano);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(T_PlanoAcao plano)
        {
            if (ModelState.IsValid)
            {
                db.Entry(plano).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(plano);
        }

        public ActionResult Delete(int id)
        {
            var plano = db.T_PlanoAcao.Find(id);
            return View(plano);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            T_PlanoAcao plano = db.T_PlanoAcao.Find(id);
            db.T_PlanoAcao.Remove(plano);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion PlanosAcao
    }
}
