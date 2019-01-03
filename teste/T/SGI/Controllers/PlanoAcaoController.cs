using SGI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SGI.Context;
using SGI.Util;

namespace SGI.Controllers
{
    [CustomAuthorize(Roles = "AdiminstradorTI, AdiminstradorPCP")]
    public class PlanoAcaoController : Controller
    {
        private JSgi db = new JSgi();

            #region PlanosAcao
        public ActionResult PlanoAcao(int idIndicador, string periodo)
        {
            var planosAcao = db.T_PlanoAcao.Where(x => x.T_Metas.IND_ID == idIndicador).ToList();
            if (periodo != "" && planosAcao.Count() > 0)
                if (planosAcao.Count(x => x.PLA_REFERENCIA.Length == periodo.Length) > 0)
                    planosAcao = planosAcao.Where(x => x.PLA_REFERENCIA.Substring(0, periodo.Length) == periodo).ToList();
                else
                    planosAcao = new List<T_PlanoAcao>();
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
                var user = db.T_Usuario.First(x => x.EMAIL == HttpContext.User.Identity.Name);
                plano.USER_ID = user.ID_USUARIO;
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
            #region Validacoes
            if(plano.PLA_DESCRICAO == "" || plano.PLA_DESCRICAO == null)
            {
                ModelState.AddModelError("PLA_DESCRICAO","Obrigatório informar a descrição.");
                ViewBag.erro = "Obrigatório informar a descrição.";
            }
            #endregion Validacoes
            if (ModelState.IsValid)
            {
                db.Entry(plano).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });
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
            return Json(new { success = true });
        }
        #endregion PlanosAcao
    }
}
