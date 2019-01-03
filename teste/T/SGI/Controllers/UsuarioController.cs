using SGI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.Security;
using System.Data.Entity;
using SGI.Context;
using SGI.Util;

namespace SGI.Controllers
{
    [CustomAuthorize(Roles = "AdiminstradorTI")]
    public class UsuarioController : Controller
    {
        //
        // GET: /Usuario/
        private JSgi db = new JSgi();

        public ActionResult Index(string searchString, int? nPageSize, int? page)
        {
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            var metas = db.T_Usuario.OrderByDescending(x => x.NOME).ToList();

            //Valida Pesquisa
            if (searchString != null && searchString != "")
                metas = metas.Where(x => (x.EMAIL + x.NOME).ToUpper().Contains(searchString.ToUpper())).OrderByDescending(y => y.NOME).ToList();

            return View(metas.ToPagedList(_PageNumber, _PageSize));
        }

        public ActionResult Create()
        {
            ViewBag.id_perfil = new SelectList(db.T_Perfil, "PER_ID", "PER_NOME");
            return View();
        }

        //
        // POST: /Usuario/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(T_Usuario usuario)
        {
            #region Validações
            //Valida E-mail
            if (db.T_Usuario.Count(x => x.EMAIL == usuario.EMAIL) > 0)
                ModelState.AddModelError("EMAIL", "E-mail informado já existe");

            //Valida Nome
            if (db.T_Usuario.Count(x => x.NOME == usuario.NOME) > 0)
                ModelState.AddModelError("NOME", "Nome informado já existe");

            if (usuario.NOME == "" || usuario.NOME == null)
                ModelState.AddModelError("NOME", "Obrigatório informar o nome");

            if (usuario.EMAIL == "" || usuario.EMAIL == null)
                ModelState.AddModelError("EMAIL", "Obrigatório informar o login");

            if (usuario.SENHA == "" || usuario.SENHA == null)
                ModelState.AddModelError("SENHA", "Obrigatório informar a senha");
            #endregion Validações

            if (ModelState.IsValid)
            {
                usuario.SENHA = FormsAuthentication.HashPasswordForStoringInConfigFile(usuario.SENHA, "SHA1");
                db.T_Usuario.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_perfil = new SelectList(db.T_Perfil, "PER_ID", "PER_NOME", usuario.ID_PERFIL);
            return View(usuario);
        }


        public ActionResult Details(int id = 0, string name = "")
        {
            T_Usuario usuario = db.T_Usuario.Find(id);
            if (name != "")
                usuario = db.T_Usuario.First(x => x.EMAIL == name);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        //
        // GET: /Usuario/Edit/5

        public ActionResult Edit(int id = 0, string name = "")
        {
            T_Usuario usuario = db.T_Usuario.Find(id);
            if (name != "")
                usuario = db.T_Usuario.First(x => x.EMAIL == name);
            if (usuario == null)
            {
                //usuario.senha = FormsAuthentication.HashPasswordForStoringInConfigFile(usuario.senha, "SHA1");
                return HttpNotFound();
            }
            ViewBag.id_perfil = new SelectList(db.T_Perfil, "PER_ID", "PER_NOME", usuario.ID_PERFIL);
            return View(usuario);
        }

        //
        // POST: /Usuario/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(T_Usuario usuario)
        {
            #region Validações
            //Valida E-mail
            if (db.T_Usuario.Count(x => x.ID_USUARIO != usuario.ID_USUARIO && x.EMAIL == usuario.EMAIL) > 0)
                ModelState.AddModelError("EMAIL", "E-mail informado já existe");

            //Valida Nome
            if (db.T_Usuario.Count(x => x.ID_USUARIO != usuario.ID_USUARIO && x.NOME == usuario.NOME) > 0)
                ModelState.AddModelError("NOME", "Nome informado já existe");
            #endregion Validações
            if (ModelState.IsValid)
            {
                usuario.SENHA = FormsAuthentication.HashPasswordForStoringInConfigFile(usuario.SENHA, "SHA1");
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_perfil = new SelectList(db.T_Perfil, "PER_ID", "PER_NOME", usuario.ID_PERFIL);
            return View(usuario);
        }

        //
        // GET: /Usuario/Delete/5

        public ActionResult Delete(int id = 0)
        {
            T_Usuario usuario = db.T_Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        //
        // POST: /Usuario/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            T_Usuario usuario = db.T_Usuario.Find(id);
            if(usuario.T_USER_GRUPO.Count > 0)
            {
                ModelState.AddModelError("EMAIL", "Desculpe, usuário não pode ser excluído pois possui grupos associdados.");
            }
            if (ModelState.IsValid)
            {
                db.T_Usuario.Remove(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        public ActionResult Senha(int id = 0, string name = "")
        {
            T_Usuario usuario = db.T_Usuario.Find(id);
            if (name != "")
                usuario = db.T_Usuario.First(x => x.EMAIL == name);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_perfil = new SelectList(db.T_Perfil, "PER_ID", "PER_NOME", usuario.ID_PERFIL);
            return View(usuario);
        }

        //
        // POST: /Usuario/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Senha(T_Usuario usuario)
        {
            if (usuario.SENHA == "" || usuario.SENHA == null)
                ModelState.AddModelError("SENHA", "Obrigatório informar a senha");
            if (ModelState.IsValid)
            {
                usuario.T_Perfil = null;
                usuario.SENHA = FormsAuthentication.HashPasswordForStoringInConfigFile(usuario.SENHA, "SHA1");
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_perfil = new SelectList(db.T_Perfil, "PER_ID", "PER_NOME", usuario.ID_PERFIL);
            return View(usuario);
        }

        [HttpPost]
        public ActionResult PostMethod()
        {
            System.Threading.Thread.Sleep(5000);

            return Json("Message from Post action method.");
        }

        public ActionResult GrupoUser(string searchString, int? nPageSize, int? page, int idUsuario)
        {
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            var grupos = db.T_USER_GRUPO.Where(x => x.ID_USUARIO == idUsuario).ToList();
            if (searchString != "" && searchString != null)
            {
                grupos = grupos.Where(x => x.T_Grupo.NOME.ToUpper().Contains(searchString.ToUpper())).ToList();
            }
            return View(grupos.OrderBy(x => x.T_Grupo.NOME).ToPagedList(_PageNumber, _PageSize));
        }

        public ActionResult GrupoUserDel(int id)
        {
            var grupo = db.T_USER_GRUPO.Find(id);
            return View(grupo);
        }

        [HttpPost]
        public ActionResult GrupoUserDel(T_USER_GRUPO grupo)
        {
            grupo = db.T_USER_GRUPO.Find(grupo.GRPUSER_ID);
            db.T_USER_GRUPO.Remove(grupo);
            db.SaveChanges();
            return Json(new { success = true });
        }

        public ActionResult AddGrupo(int idUsuario)
        {
            var grupometa = new T_USER_GRUPO();
            ViewBag.GRU_ID = new SelectList(db.T_Grupo, "GRU_ID", "NOME");
            grupometa.ID_USUARIO = idUsuario;
            return View(grupometa);
        }

        [HttpPost]
        public ActionResult AddGrupo(T_USER_GRUPO grupo)
        {
            if (grupo.GRU_ID <= 0)
            {
                ModelState.AddModelError("GRU_ID", "Selecione um grupo.");
                ViewBag.erro = "Selecione um grupo.";
            }
            if (ModelState.IsValid)
            {
                db.T_USER_GRUPO.Add(grupo);
                db.SaveChanges();
                return Json(new { success = true });
            }
            ViewBag.GRU_ID = new SelectList(db.T_Grupo, "GRU_ID", "NOME",grupo.GRU_ID);
            return View(grupo);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
