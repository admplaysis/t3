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
    public class IndicadorController : Controller
    {
        //
        // GET: /Indicador/
        private JSgi db = new JSgi();

        public ActionResult Index(string searchString, int? nPageSize, int? page)
        {
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            var indicadores = db.T_Indicadores.OrderBy(x => x.IND_DESCRICAO).ToList();

            //Valida Pesquisa
            if (searchString != null && searchString != "")
                indicadores = indicadores.Where(x => (x.IND_DESCRICAO + x.T_Negocio.NEG_DESCRICAO).ToUpper().Contains(searchString.ToUpper())).OrderBy(y => y.IND_DESCRICAO).ToList();

            return View(indicadores.ToPagedList(_PageNumber, _PageSize));
        }

        public ActionResult Details(int id)
        {
            var indicador = db.T_Indicadores.Find(id);
            return View(indicador);
        }

        public ActionResult Create()
        {
            ViewBag.NEG_ID = new SelectList(db.T_Negocio, "NEG_ID", "NEG_DESCRICAO");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(T_Indicadores indicador)
        {
            Validacoes(indicador);
            if (ModelState.IsValid)
            {
                db.T_Indicadores.Add(indicador);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NEG_ID = new SelectList(db.T_Negocio, "NEG_ID", "NEG_DESCRICAO",indicador.NEG_ID);
            return View(indicador);
        }

        public ActionResult Edit(int id)
        {
            var indicador = db.T_Indicadores.Find(id);
            ViewBag.NEG_ID = new SelectList(db.T_Negocio, "NEG_ID", "NEG_DESCRICAO", indicador.NEG_ID);
            return View(indicador);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(T_Indicadores indicador)
        {
            if (ModelState.IsValid)
            {
                db.Entry(indicador).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NEG_ID = new SelectList(db.T_Negocio, "NEG_ID", "NEG_DESCRICAO", indicador.NEG_ID);
            return View(indicador);
        }

        public void Validacoes(T_Indicadores indicadores)
        {
            if (indicadores.IND_DESCRICAO == null || indicadores.IND_DESCRICAO.Trim().Equals(""))
                ModelState.AddModelError("IND_DESCRICAO", "Informe a descrição do indicador");

            if (indicadores.NEG_ID == null || indicadores.NEG_ID <= 0 )
                ModelState.AddModelError("NEG_ID", "Selecione o negócio");
        }

        public ActionResult Delete(int id)
        {
            var indicador = db.T_Indicadores.Find(id);
            return View(indicador);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            T_Indicadores indicador = db.T_Indicadores.Find(id);
            db.T_Indicadores.Remove(indicador);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #region Grupo
        public ActionResult ListaGrupos(string searchString, int? nPageSize, int? page, int idInd)
        {
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            var grupos = db.T_Grupo_Indicador.Where(x => x.IND_ID == idInd).ToList();
            if (searchString != "" && searchString != null)
            {
                grupos = grupos.Where(x => x.T_Grupo.NOME.ToUpper().Contains(searchString.ToUpper())).ToList();
            }
            return View(grupos.OrderBy(x => x.T_Grupo.NOME).ToPagedList(_PageNumber, _PageSize));
        }

        public ActionResult AddGrupo(int id)
        {
            var grupoIndicador = new T_Grupo_Indicador();
            grupoIndicador.T_Indicadores = db.T_Indicadores.Find(id);
            grupoIndicador.IND_ID = id;
            ViewBag.GRU_ID = new SelectList(db.T_Grupo.OrderBy(x => x.NOME), "GRU_ID", "NOME");
            return View(grupoIndicador);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult AddGrupo(T_Grupo_Indicador grupo)
        {
            #region Validacao
            //Validação
            ModelState.Remove("GRU_ID");
            if (db.T_Grupo_Indicador.Count(x => x.GRU_ID == grupo.GRU_ID && x.IND_ID == grupo.IND_ID) > 0)
            {
                ModelState.AddModelError("GRU_ID", "Grupo já se encontra vinculado a este indicador!");
            }

            //Validação
            if (grupo.GRU_ID <= 0 || grupo.GRU_ID == null)
            {
                ModelState.AddModelError("GRU_ID", "Selecione o grupo!");
            }

            //Validação
            ModelState.Remove("IND_ID");
            if (grupo.IND_ID <= 0 || grupo.IND_ID == null)
            {
                ModelState.AddModelError("IND_ID", "Indicador não informado!");
            }
            #endregion Validacao

            if (ModelState.IsValid)
            {
                grupo.T_Indicadores = null;
                db.T_Grupo_Indicador.Add(grupo);
                db.SaveChanges();
                return RedirectToAction("ListaGrupos", new { idInd = grupo.IND_ID });
            }
            ViewBag.GRU_ID = new SelectList(db.T_Grupo.OrderBy(x => x.NOME), "GRU_ID", "NOME", grupo.GRU_ID);
            return View(grupo);
        }


        public ActionResult GrupoDel(int id)
        {
            var grupo = db.T_Grupo_Indicador.Find(id);
            return View(grupo);
        }

        [HttpPost]
        public ActionResult GrupoDel(T_Grupo_Indicador grupo)
        {
            bool isSuccess = true;
            grupo = db.T_Grupo_Indicador.Find(Int32.Parse(Request["GRU_IND_ID"].ToString()));
            db.T_Grupo_Indicador.Remove(grupo);
            db.SaveChanges();
            return Json(isSuccess);
        }
        #endregion Grupo

        #region Departamento
        public ActionResult ListaDepartamentos(string searchString, int? nPageSize, int? page, int idInd)
        {
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            var departamentos = db.T_Indicadores_Departamentos.Where(x => x.IND_ID == idInd).ToList();
            if (searchString != "" && searchString != null)
            {
                departamentos = departamentos.Where(x => x.T_Departamentos.DEP_NOME.ToUpper().Contains(searchString.ToUpper())).ToList();
            }
            return View(departamentos.OrderBy(x => x.T_Departamentos.DEP_NOME).ToPagedList(_PageNumber, _PageSize));
        }

        public ActionResult AddDepartamento(int id)
        {
            var departamentoIndicador = new T_Indicadores_Departamentos();
            departamentoIndicador.T_Indicadores = db.T_Indicadores.Find(id);
            departamentoIndicador.IND_ID = id;
            ViewBag.DEP_ID = new SelectList(db.T_Departamentos, "DEP_ID", "DEP_NOME");
            return View(departamentoIndicador);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult AddDepartamento(T_Indicadores_Departamentos departamento)
        {
            #region Validacao
            //Validação
            ModelState.Remove("DEP_ID");
            if (db.T_Indicadores_Departamentos.Count(x => x.DEP_ID == departamento.DEP_ID && x.IND_ID == departamento.IND_ID) > 0)
            {
                ModelState.AddModelError("DEP_ID", "Departamento já se encontra vinculado a este indicador!");
            }

            //Validação
            if (departamento.DEP_ID <= 0 || departamento.DEP_ID == null)
            {
                ModelState.AddModelError("DEP_ID", "Selecione o departamento!");
            }

            //Validação
            ModelState.Remove("IND_ID");
            if (departamento.IND_ID <= 0 || departamento.IND_ID == null)
            {
                ModelState.AddModelError("IND_ID", "Indicador não informado!");
            }
            #endregion Validacao

            if (ModelState.IsValid)
            {
                departamento.T_Indicadores = null;
                db.T_Indicadores_Departamentos.Add(departamento);
                db.SaveChanges();
                return RedirectToAction("ListaDepartamentos", new { idInd = departamento.IND_ID });
            }
            ViewBag.DEP_ID = new SelectList(db.T_Departamentos, "DEP_ID", "DEP_NOME", departamento.DEP_ID);
            return View(departamento);
        }


        public ActionResult DepartamentoDel(int id)
        {
            var grupo = db.T_Indicadores_Departamentos.Find(id);
            return View(grupo);
        }

        [HttpPost]
        public ActionResult DepartamentoDel(T_Indicadores_Departamentos departamento)
        {
            bool isSuccess = true;
            departamento = db.T_Indicadores_Departamentos.Find(Int32.Parse(Request["INDDEP_ID"].ToString()));
            db.T_Indicadores_Departamentos.Remove(departamento);
            db.SaveChanges();
            return Json(isSuccess);
        }
        #endregion Departamento

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
