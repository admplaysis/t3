using SGI.Context;
using SGI.Models;
using SGI.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FormsAuthenticationExtensions;
using System.Collections.Specialized;
using SGI.Autenticacao;
using System.Web.Script.Serialization;

namespace SGI.Controllers
{
    public class AcessoController : Controller
    {
        private JSgi db = new JSgi();
        //
        // GET: /Acesso/

        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize]
        public ActionResult SemAcesso()
        {
            return View();
        }


        public ActionResult Login(string ReturnUrl)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Login(T_Usuario usuario, string ReturnUrl)
        {
            var valido = false;
            //valido = Autentica("10.0.10.2", usuario.EMAIL, usuario.SENHA);
            if (valido == false)
            {
                if (usuario.EMAIL == "" || usuario.SENHA == "" || usuario.EMAIL == null || usuario.SENHA == null)
                    ViewBag.alerta = "Usuário ou Senha não informados!";
                else
                {
                    usuario.SENHA = FormsAuthentication.HashPasswordForStoringInConfigFile(usuario.SENHA, "SHA1");
                    if (usuario.EMAIL == null || usuario.SENHA == null || AutenticarUsuario(usuario) == false)
                    {
                        ViewBag.alerta = "Usuário ou Senha Inválidos!";
                    }
                    else
                    {
                        var user = db.T_Usuario.First(x => x.EMAIL == usuario.EMAIL && x.SENHA == usuario.SENHA);
                        
                        CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel()
                        {
                            Id = user.ID_USUARIO,
                            Name = user.NOME,
                            Roles = new[] { user.T_Perfil.PER_NOME }
                        };
                        string userData = new JavaScriptSerializer().Serialize(serializeModel);
                        var authTicket = new FormsAuthenticationTicket(1, user.ID_USUARIO.ToString(), DateTime.Now, DateTime.Now.AddMinutes(1000), true, userData);
                        string cookieContents = FormsAuthentication.Encrypt(authTicket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieContents)
                        {
                            Expires = authTicket.Expiration,
                            Path = FormsAuthentication.FormsCookiePath
                        };
                        System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                        Response.Cookies.Add(cookie);

                        if (Url.IsLocalUrl(ReturnUrl) && ReturnUrl.Length > 1 && ReturnUrl.StartsWith("/") && !ReturnUrl.StartsWith("//") && !ReturnUrl.StartsWith("/\\"))
                            return Redirect(ReturnUrl);
                        else
                            return RedirectToAction("Index", "Home");
                    }
                }
            }
            //Acesso pelo AD
            else
            {
                if (db.T_Usuario.Count(x => x.EMAIL == usuario.EMAIL) <= 0)
                {
                    ViewBag.alerta = "Usuário encontrado no Activity Directory, porém o mesmo não possui perfil associado na ferramenta de indicadores";
                    return View(usuario);
                }

                var user = db.T_Usuario.First(x => x.EMAIL == usuario.EMAIL);
                var role = user.T_Perfil.PER_NOME;

                var authTicket = new FormsAuthenticationTicket(1, user.EMAIL, DateTime.Now, DateTime.Now.AddMinutes(1000), true, role);
                string cookieContents = FormsAuthentication.Encrypt(authTicket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieContents)
                {
                    Expires = authTicket.Expiration,
                    Path = FormsAuthentication.FormsCookiePath
                };
                System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                Response.Cookies.Add(cookie);

                if (Url.IsLocalUrl(ReturnUrl) && ReturnUrl.Length > 1 && ReturnUrl.StartsWith("/") && !ReturnUrl.StartsWith("//") && !ReturnUrl.StartsWith("/\\"))
                    return Redirect(ReturnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }
            return View(usuario);
        }

        public ActionResult Forgot()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Forgot(T_Usuario usuario)
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Acesso");
        }

        //Metódo Validar Usuário
        public bool AutenticarUsuario(T_Usuario usuario)
        {
            if (db.T_Usuario.Count(x => x.EMAIL == usuario.EMAIL && x.SENHA == usuario.SENHA) > 0)
                return true;
            else
                return false;
        }

        public ActionResult Perfil()
        {
            var user = new T_Usuario();
            int id = Convert.ToInt32(HttpContext.User.Identity.Name);
            user = db.T_Usuario.Find(id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Perfil(T_Usuario usuario)
        {
            ModelState.Remove("SENHA");
            if (ModelState.IsValid)
            {
                var user = db.T_Usuario.Find(usuario.ID_USUARIO);
                user.NOME = usuario.NOME;
                if (usuario.SENHA != "" && usuario.SENHA != null)
                    user.SENHA = FormsAuthentication.HashPasswordForStoringInConfigFile(usuario.SENHA, "SHA1");
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(usuario);
        }

        /// <summary>
        /// Autentica acesso pelo AD
        /// </summary>
        /// <param name="IpServer">Endereço de ip do servidor</param>
        /// <param name="User">Usuário de acesso</param>
        /// <param name="Senha">Senha de acesso</param>
        /// <returns>Retorna true se autenticado e false se não foi autenticado</returns>
        public bool Autentica(string IpServer, string User, string Senha)
        {
            bool valido = false;
            try
            {
                DirectoryEntry objAD = new DirectoryEntry("LDAP://" + IpServer, User, Senha);
                var grupos = new List<string>();
                var grupoSgi = new List<T_Grupo>();
                var perfisAcesso = new List<T_Perfil>();
                if (objAD.Name != "")
                    valido = true;
                //Valida se autentiou usuário no AD
                if (valido)
                {
                    grupos = BuscaListadeGrupo(objAD);
                    grupoSgi = db.T_Grupo.Where(x => grupos.Any(j => j.ToUpper() == x.NOME.ToUpper())).ToList();
                    perfisAcesso = db.T_Perfil.Where(x => grupos.Any(j => j.ToUpper().Replace("PSGI_", "") == x.PER_NOME.ToUpper())).ToList();

                    //Valida se usuário esta cadastrado no SGI
                    if (db.T_Usuario.Count(x => x.EMAIL == User) > 0)
                    {
                        var usuario = db.T_Usuario.First(x => x.EMAIL == User);
                        if (grupos.Count <= 0)
                            usuario.ATIVO = (int)Enums.Ativo.Bloqueada;
                        else
                        {
                            var gruposUsuario = db.T_USER_GRUPO.Where(x => x.ID_USUARIO == usuario.ID_USUARIO).ToList();
                            db.T_USER_GRUPO.RemoveRange(gruposUsuario);
                            foreach (var item in grupoSgi)
                            {
                                if (usuario.T_USER_GRUPO.Count(x => x.GRU_ID == item.GRU_ID) <= 0)
                                {
                                    usuario.T_USER_GRUPO.Add(new T_USER_GRUPO() { GRU_ID = item.GRU_ID, ID_USUARIO = usuario.ID_USUARIO });
                                }
                            }
                        }
                        if (perfisAcesso.Count > 0)
                            usuario.ID_PERFIL = perfisAcesso.FirstOrDefault().PER_ID;
                        db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();//Salva usuário
                    }
                    else if (grupoSgi.Count > 0)//Cadastra usuário
                    {
                        var usuario = new T_Usuario();
                        if (perfisAcesso.Count > 0)
                            usuario.ID_PERFIL = perfisAcesso.FirstOrDefault().PER_ID;
                        else
                            usuario.ID_PERFIL = db.T_Perfil.FirstOrDefault(x => x.PER_NOME == "Padrão").PER_ID;
                        usuario.EMAIL = User;
                        usuario.NOME = User;
                        usuario.SENHA = FormsAuthentication.HashPasswordForStoringInConfigFile(Senha, "SHA1");
                        foreach (var item in grupoSgi)
                        {
                            if (usuario.T_USER_GRUPO.Count(x => x.GRU_ID == item.GRU_ID) <= 0)
                            {
                                usuario.T_USER_GRUPO.Add(new T_USER_GRUPO() { GRU_ID = item.GRU_ID, ID_USUARIO = usuario.ID_USUARIO });
                            }
                        }
                        db.T_Usuario.Add(usuario);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception(ex.Message);
            }

            return valido;
        }

        /// <summary>
        /// Busca grupos de usuários no AD
        /// </summary>
        /// <param name="de">Objeto com o usuário autenticado.</param>
        /// <returns>Retorna lista de grupos</returns>
        public List<string> BuscaListadeGrupo(DirectoryEntry de)
        {

            var objSearchADAM = new DirectorySearcher(de);
            objSearchADAM.Filter = "(SAMAccountName=" + de.Username + ")";
            objSearchADAM.SearchScope = SearchScope.Subtree;
            var objSearchResults = objSearchADAM.FindOne();
            List<string> grupos = new List<string>();
            foreach (object oMember in objSearchResults.Properties["memberOf"])
            {
                var grupo = oMember.ToString().Split(',')[0].Replace("CN=", "");
                var filter = string.Format("(&(objectClass=group)(name={0}))", grupo);
                var ds = new DirectorySearcher(de, filter);
                var result = ds.FindOne();
                grupos.Add(grupo.Replace("CSGI_", ""));
                //Busca Sub Grupo);
                foreach (var subGrupo in result.Properties["memberOf"])
                {
                    grupos.Add(subGrupo.ToString().Split(',')[0].Replace("CN=", "").Replace("CSGI_", ""));
                }
            }
            return grupos;
        }

        #region Favoritos
        public ActionResult AddFavorito()
        {
            ViewBag.ID_INDICADOR = new SelectList(db.T_Indicadores, "IND_ID", "IND_DESCRICAO");
            var favorito = new T_Favoritos();
            int id = Convert.ToInt32(HttpContext.User.Identity.Name);
            var usuario = db.T_Usuario.Find(id);
            favorito.ID_USUARIO = usuario.ID_USUARIO;
            return View(favorito);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult AddFavorito(T_Favoritos Favorito)
        {
            if (db.T_Favoritos.Count(x => x.ID_INDICADOR == Favorito.ID_INDICADOR) > 0)
                ModelState.AddModelError("ID_INDICADOR", "Indicador já encontra-se no favoritos.");

            if (Favorito.ID_INDICADOR <= 0)
            {
                ModelState.Remove("ID_INDICADOR");
                ModelState.AddModelError("ID_INDICADOR", "Selecione o indicador.");
            }

            if (ModelState.IsValid)
            {
                db.T_Favoritos.Add(Favorito);
                db.SaveChanges();
                return Json(new { success = true });
            }
            ViewBag.ID_INDICADOR = new SelectList(db.T_Indicadores, "IND_ID", "IND_DESCRICAO");
            return View(Favorito);
        }

        public ActionResult DelFavorito(int idIndicador)
        {
            var favorito = db.T_Favoritos.Find(idIndicador);
            return View(favorito);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult DelFavorito(T_Favoritos favorito)
        {
            favorito = db.T_Favoritos.Find(favorito.IDFAVORITO);
            db.T_Favoritos.Remove(favorito);
            db.SaveChanges();
            return Json(new { success = true });
        }
        #endregion Favoritos
    }
}
