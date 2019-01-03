using SGI.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SGI.Models;
using SGI.Util;

namespace SGI.Controllers
{
    [CustomAuthorize(Roles = "AdiminstradorTI, AdiminstradorPCP")]
    public class VisaoController : Controller
    {
        private JSgi db = new JSgi();
        //
        // GET: /Visao/

        public ActionResult Index(string searchString, int? nPageSize, int? page)
        {
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            var visao = db.Tr_CabViscao.Where(x => x.CAB_STATUS != (int)SGI.Enums.Ativo.EmEdicao).OrderBy(x => x.CAB_DESC).ToList();
            //Valida Pesquisa
            if (searchString != null && searchString != "")
                visao = visao.Where(x => (x.CAB_DESC + x.CAB_ID.ToString()).ToUpper().Contains(searchString.ToUpper())).OrderBy(y => y.CAB_DESC).ToList();

            return View(visao.OrderBy(x => x.CAB_DESC).ToPagedList(_PageNumber, _PageSize));
        }

        public ActionResult Create()
        {
            var cabVisao = new Tr_CabViscao();
            //Valida se já existe registro salvo no banco de dados
            var usuario = db.T_Usuario.First(x => x.EMAIL == HttpContext.User.Identity.Name);
            if(db.Tr_CabViscao.Count(x => x.ID_USUARIO == usuario.ID_USUARIO && x.CAB_STATUS == (int)SGI.Enums.Ativo.EmEdicao) > 0)
            {
                cabVisao = db.Tr_CabViscao.First(x => x.ID_USUARIO == usuario.ID_USUARIO && x.CAB_STATUS == (int)SGI.Enums.Ativo.EmEdicao);
            }else
            {
                cabVisao.CAB_STATUS = (int)SGI.Enums.Ativo.EmEdicao;
                cabVisao.CAB_DESC = "PADRÃO PORTAL";
                cabVisao.ID_USUARIO = usuario.ID_USUARIO;
                db.Tr_CabViscao.Add(cabVisao);
                db.SaveChanges();
                cabVisao.Tr_Visoes = new List<Tr_Visoes>();
            }

            if (cabVisao.CAB_DESC == "PADRÃO PORTAL")
                cabVisao.CAB_DESC = "";
            return View(cabVisao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Tr_CabViscao Tr_CabViscao)
        {
            if(ModelState.IsValid)
            {
                Tr_CabViscao.CAB_STATUS = (int)SGI.Enums.Ativo.Liberado;
                db.Entry(Tr_CabViscao).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            Tr_CabViscao.Tr_Visoes = db.Tr_Visoes.Where(x => x.CAB_ID == Tr_CabViscao.CAB_ID).ToList();
            return View(Tr_CabViscao);
        }

        public ActionResult Details(int id)
        {
            var cabVisao = db.Tr_CabViscao.Find(id);
            cabVisao.Tr_Visoes = db.Tr_Visoes.Where(x => x.CAB_ID == cabVisao.CAB_ID).ToList();
            return View(cabVisao);
        }

        public ActionResult Edit(int id)
        {
            var cabVisao = db.Tr_CabViscao.Find(id);
            cabVisao.Tr_Visoes = db.Tr_Visoes.Where(x => x.CAB_ID == cabVisao.CAB_ID).ToList();
            return View(cabVisao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tr_CabViscao cabVisao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cabVisao).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            cabVisao.Tr_Visoes = db.Tr_Visoes.Where(x => x.CAB_ID == cabVisao.CAB_ID).ToList();
            return View(cabVisao);
        }

        public ActionResult Delete(int id)
        {
            var visao = db.Tr_CabViscao.Find(id);
            return View(visao);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tr_CabViscao visao = db.Tr_CabViscao.Find(id);
            db.Tr_Visoes.RemoveRange(visao.Tr_Visoes);
            db.Tr_CabViscao.Remove(visao);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #region ModalPopup
        public ActionResult TodasContas(int idCab)
        {
            return View();
        }

        public ActionResult AddConta(int idCab)
        {
            var visao = new Tr_Visoes();
            visao.CAB_ID = idCab;
            ViewBag.VIS_PLANID = new SelectList(db.Tr_PlanoContas, "PLA_ID", "PLA_DESCRICAO");
            return View(visao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult AddConta(Tr_Visoes Tr_Visoes)
        {
            bool valido = true;
            #region Validacoes
            if (Tr_Visoes.VIS_PLANID <= 0 || Tr_Visoes.VIS_PLANID == null)
            {
                ModelState.AddModelError("VIS_PLANID","Seleciona uma conta.");
                ViewBag.erro = "Selecione uma conta.";
            }

            if (db.Tr_Visoes.Count(x => x.CAB_ID == Tr_Visoes.CAB_ID && x.VIS_PLANID == Tr_Visoes.VIS_PLANID) > 0)
            {
                ModelState.AddModelError("VIS_PLANID", "Conta selecionada já está vinculada a está visão.");
                ViewBag.erro = "Conta selecionada já está vinculada a está visão.";
            }
            #endregion Validacoes

            if (ModelState.IsValid)
            {
                db.Tr_Visoes.Add(Tr_Visoes);
                db.SaveChanges();
                return Json(new { success = true });
            }
            ViewBag.VIS_PLANID = new SelectList(db.Tr_PlanoContas, "PLA_ID", "PLA_DESCRICAO", Tr_Visoes.VIS_PLANID);
            return View(Tr_Visoes);
        }

        public ActionResult DelVisao(int id)
        {
            var visao = db.Tr_Visoes.Find(id);
            ViewBag.VIS_PLANID = new SelectList(db.Tr_PlanoContas, "PLA_ID", "PLA_DESCRICAO", visao.VIS_PLANID);
            return View(visao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult DelVisao(Tr_Visoes visaoDel)
        {
            var visao = db.Tr_Visoes.Find(visaoDel.VIS_ID);
            db.Tr_Visoes.Remove(visao);
            db.SaveChanges();
            return Json(new { success = true });
        }
        #endregion ModalPopup
    }
}
