using SGI.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SGI.Models.Custom;
using System.Data.SqlClient;
using SGI.Models;
using SGI.Util;

namespace SGI.Controllers
{
    [CustomAuthorize(Roles = "AdiminstradorTI, AdiminstradorPCP")]
    public class WorkFlowController : Controller
    {
        private JProtheus dbProtheus = new JProtheus();
        private JSgi dbSgi = new JSgi();
        //
        // GET: /WorkFlow/

        #region PedidoDeCompras
        public ActionResult PedCompra(string searchString, int? nPageSize, int? page, string pStatus, string pEmpresa = "")
        {
            #region DeclaracoesVariaveis
            int userId =  Convert.ToInt32(HttpContext.User.Identity.Name);
            var usuario = dbSgi.T_Usuario.Find(userId);
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            ViewBag.Status = new SelectList(new List<string> { "Aguardando Aprovação", "Aprovado", "Rejeitado", "Todos" }, selectedValue: pStatus);
            ViewBag.Empresa = new SelectList(new List<string> { "Jaepel", "Florestal" }, selectedValue: pEmpresa);
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 20;
            var pedido = new List<PedidoCustom>();
            if (String.IsNullOrEmpty(usuario.CODERP))
                return View(pedido.ToPagedList(_PageNumber, _PageSize));
            List<SqlParameter> parametros = null;
            #endregion DeclaracoesVariaveis
            //Executa busca de dados query customizada
            #region Query
            var query = "SELECT Empresa,Moeda,StatusSCR,Emissao,DtLiberacao,Pedido,Fornecedor,Loja,ForNome,Total FROM( ";
            query += "select 'Jaepel' Empresa,CR_MOEDA Moeda,RTRIM(CR_STATUS) StatusSCR,RTRIM(CR_USER) Usuario,RTRIM(CR_EMISSAO) Emissao,RTRIM(CR_DATALIB) DtLiberacao,RTRIM(CR_NUM) Pedido,RTRIM(C7_FORNECE) Fornecedor,RTRIM(C7_LOJA) Loja,RTRIM(A2_NOME) ForNome,CR_TOTAL Total from SCR010 scr ";
            query += "LEFT JOIN SC7010 sc7 ON C7_NUM = CR_NUM AND sc7.D_E_L_E_T_ = '' ";
            query += "inner join SA2010 sa2 on A2_COD = C7_FORNECE AND A2_LOJA = C7_LOJA and sa2.D_E_L_E_T_ = '' ";
            query += "WHERE  scr.D_E_L_E_T_ = '' and (CR_NIVEL = (SELECT TOP 1 CR_NIVEL FROM SCR010 cr2 WHERE cr2.CR_NUM = scr.CR_NUM and cr2.CR_DATALIB = '' and cr2.D_E_L_E_T_= '' ORDER BY cr2.CR_NIVEL) or scr.CR_DATALIB != '') ";
            query += "GROUP BY CR_NUM,CR_MOEDA,CR_STATUS,C7_FORNECE,C7_LOJA,A2_NOME,CR_TOTAL,CR_EMISSAO,CR_DATALIB,CR_USER ";
            query += "UNION ALL ";
            query += "select 'Florestal' Empresa,CR_MOEDA Moeda,RTRIM(CR_STATUS) StatusSCR,RTRIM(CR_USER) Usuario,RTRIM(CR_EMISSAO) Emissao,RTRIM(CR_DATALIB) DtLiberacao,RTRIM(CR_NUM) Pedido,RTRIM(C7_FORNECE) Fornecedor,RTRIM(C7_LOJA) Loja,RTRIM(A2_NOME) ForNome,CR_TOTAL Total from SCR020 scr ";
            query += "LEFT JOIN SC7020 sc7 ON C7_NUM = CR_NUM AND sc7.D_E_L_E_T_ = '' ";
            query += "inner join SA2020 sa2 on A2_COD = C7_FORNECE AND A2_LOJA = C7_LOJA and sa2.D_E_L_E_T_ = '' ";
            query += "WHERE  scr.D_E_L_E_T_ = '' and (CR_NIVEL = (SELECT TOP 1 CR_NIVEL FROM SCR020 cr2 WHERE cr2.CR_NUM = scr.CR_NUM and cr2.CR_DATALIB = '' and cr2.D_E_L_E_T_= '' ORDER BY cr2.CR_NIVEL) or scr.CR_DATALIB != '') ";
            query += "GROUP BY CR_NUM,CR_MOEDA,CR_STATUS,C7_FORNECE,C7_LOJA,A2_NOME,CR_TOTAL,CR_EMISSAO,CR_DATALIB,CR_USER ";
            query += ") AS ped ";
            query += "where ped.Usuario in('" + usuario.CODERP.Replace(",", "','") + "') ";

            //#### Filtros
            #region Filtros
            parametros = new List<SqlParameter>();
            //Realiza pesquisa
            if (!String.IsNullOrEmpty(searchString))
            {
                query += " and upper(Pedido+Fornecedor+Loja+ForNome) LIKE @search ";
                parametros.Add(new SqlParameter("@search", "%" + searchString.ToUpper() + "%"));
            }

            //Filtra por empresa
            if (!string.IsNullOrEmpty(pEmpresa))
            {
                query += " and ped.Empresa = @Empresa ";
                parametros.Add(new SqlParameter("@Empresa", pEmpresa));
            }

            //Realiza filtro por status
            if (!String.IsNullOrEmpty(pStatus))
            {
                switch (pStatus)
                {
                    case "Aguardando Aprovação":
                        query += " and ped.StatusSCR = '02' ";
                        break;
                    case "Aprovado":
                        query += " and ped.StatusSCR = '03' ";
                        break;
                    case "Rejeitado":
                        query += " and ped.StatusSCR = '04' ";
                        break;
                }
            }
            else
            {
                query += " and ped.StatusSCR = '02' ";
            }
            query += "group by Empresa,Moeda,StatusSCR,Emissao,DtLiberacao,Pedido,Fornecedor,Loja,ForNome,Total ";
            query += "order by Emissao desc,Pedido desc";
            #endregion Filtros
            #endregion Query

            pedido = dbProtheus.Database.SqlQuery<PedidoCustom>(query, parametros.ToArray()).ToList();
            return View(pedido.OrderByDescending(x => x.Empresa + x.Emissao).ToPagedList(_PageNumber, _PageSize));
        }

        public ActionResult PedDetails(string pedido, string pEmpresa, string searchString, int? nPageSize, int? page, string pStatus)
        {
            #region DeclaracaoVariaveis
            var usuario = dbSgi.T_Usuario.First(x => x.EMAIL == HttpContext.User.Identity.Name);
            var pedidoCustom = new PedidoCustom();
            #endregion DeclaracaoVariaveis

            pedidoCustom = GetDadosAprovacaoPedido(pEmpresa, pedido);
            return View(pedidoCustom);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult PedDetails(PedidoCustom pedidoCustom)
        {
            var usuario = dbSgi.T_Usuario.First(x => x.EMAIL == HttpContext.User.Identity.Name);
            bool valido = true;
            string status = Request["aprovacao"] == "N" ? "04" : "03";
            //Registra auditoria
            #region RegistraAuditoria
            var auditoria = new T_Auditoria();
            auditoria.ID_USUARIO = usuario.ID_USUARIO;
            auditoria.DATA = DateTime.Now;
            auditoria.ROTINA = "Aprovação Pedido de Compras";
            auditoria.CHAVE = pedidoCustom.Pedido + pedidoCustom.Loja + pedidoCustom.Fornecedor;
            auditoria.HISTORICO = "Empresa: " + pedidoCustom.Empresa + " Pedido: " + pedidoCustom.Pedido;
            auditoria.HISTORICO += " Fornecedor: " + pedidoCustom.Fornecedor + pedidoCustom.Loja + pedidoCustom.ForNome;
            #endregion RegistraAuditoria

            //Validações da rotina
            #region Validacoes
            if (Request["aprovacao"] == "N" && String.IsNullOrEmpty(pedidoCustom.MotivoRejeicao))
            {
                ModelState.AddModelError("MotivoRejeicao", "Obrigatório informar o motivo da rejeição.");
                ViewBag.alerta = "Por favor preencha a justificativa referente a não aprovação.";
                valido = false;
            }
            #endregion Validacoes

            if (valido)
            {
                //Confirmação de aprovação
                #region ConfirmaAprovacao
                var tabela = pedidoCustom.Empresa == "Jaepel" ? "SCR010" : "SCR020";
                var query = "UPDATE " + tabela + " set CR_STATUS = @Status,CR_DATALIB = @DtLib ";
                query += ",CR_XOBSAPT = CONVERT(VARBINARY(MAX), @ObsAprovacao) ";
                query += ",CR_XJUSREJ = CONVERT(VARBINARY(MAX), @Justificativa) ";
                query += "WHERE D_E_L_E_T_ = '' AND CR_NUM = @Pedido and CR_USER in('" + usuario.CODERP.Replace(",", "','") + "') ";
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@Status", status));//Seta status como Liberado pelo usuário
                parametros.Add(new SqlParameter("@DtLib", DateTime.Now.ToString("yyyyMMdd")));
                parametros.Add(new SqlParameter("@Pedido", pedidoCustom.Pedido));
                parametros.Add(new SqlParameter("@ObsAprovacao", string.IsNullOrEmpty(pedidoCustom.ObsAprovacao) ? "" : pedidoCustom.ObsAprovacao));
                parametros.Add(new SqlParameter("@Justificativa", string.IsNullOrEmpty(pedidoCustom.MotivoRejeicao) ? "" : pedidoCustom.MotivoRejeicao));
                //Executa comando no banco de dados
                dbProtheus.Database.ExecuteSqlCommand(query, parametros.ToArray());
                //Chama rotina para poder realizar aprovação de pedidos
                if (AprovacaoTotalPedido(pedidoCustom.Pedido, pedidoCustom.Empresa))
                {
                    string eMails = "";

                    //Obtem e-mail do comprador
                    if (pedidoCustom.Comprador != null)
                        if (!string.IsNullOrEmpty(pedidoCustom.Comprador.Email))
                            eMails += pedidoCustom.Comprador.Email + ";";

                    //Obtem e-mail do solicitante
                    if (pedidoCustom.Solicitante != null)
                        if (!string.IsNullOrEmpty(pedidoCustom.Solicitante.Email))
                            eMails += pedidoCustom.Solicitante.Email + ";";

                    if (!string.IsNullOrEmpty(eMails))
                        Util.EnviaEmail.EnviarEmail("Aprovação Pedido de compras", eMails, Util.EnviaEmail.TextoEmailPedido(pedidoCustom), pedidoCustom.Empresa);
                }
                Util.Auditoria.Registrar(auditoria);
                #endregion ConfirmaAprovacao
                ///Criar metódo para avaliar se o pedido está 100% aprovado alterar SC7 C7_CONAPRO = 'L'
                return RedirectToAction("PedCompra");
            }
            else
            {
                pedidoCustom = GetDadosAprovacaoPedido(pedidoCustom.Empresa, pedidoCustom.Pedido);
                return View(pedidoCustom);
            }
        }

        public ActionResult AprovPed(string pedido, string empresa)
        {
            var pedidoCustom = new PedidoCustom();
            var usuario = dbSgi.T_Usuario.First(x => x.EMAIL == HttpContext.User.Identity.Name);
            List<SqlParameter> parametros = null;
            #region Query
            var query = "SELECT * FROM( ";
            query += "select 'Jaepel' Empresa,CR_MOEDA Moeda,CR_STATUS StatusSCR,C7_CONAPRO StatusSC7,CR_USER Usuario,CR_EMISSAO Emissao,CR_DATALIB DtLiberacao,CR_NUM Pedido,C7_FORNECE Fornecedor,C7_LOJA Loja,A2_NOME ForNome,CR_TOTAL Total from SCR010 scr ";
            query += "LEFT JOIN SC7010 sc7 ON C7_NUM = CR_NUM AND sc7.D_E_L_E_T_ = '' ";
            query += "inner join SA2010 sa2 on A2_COD = C7_FORNECE AND A2_LOJA = C7_LOJA and sa2.D_E_L_E_T_ = '' ";
            query += "WHERE  scr.D_E_L_E_T_ = '' ";
            query += "GROUP BY CR_NUM,CR_MOEDA,CR_STATUS,C7_CONAPRO,C7_FORNECE,C7_LOJA,A2_NOME,CR_TOTAL,CR_EMISSAO,CR_DATALIB,CR_USER ";
            query += "UNION ALL ";
            query += "select 'Florestal' Empresa,CR_MOEDA Moeda,CR_STATUS StatusSCR,C7_CONAPRO StatusSC7,CR_USER USUARIO,CR_EMISSAO EMISSAO,CR_DATALIB DTLIBERACAO,CR_NUM PEDIDO,C7_FORNECE FORNECEDOR,C7_LOJA LOJA,A2_NOME FORNOME,CR_TOTAL TOTAL from SCR020 scr ";
            query += "LEFT JOIN SC7020 sc7 ON C7_NUM = CR_NUM AND sc7.D_E_L_E_T_ = '' ";
            query += "inner join SA2020 sa2 on A2_COD = C7_FORNECE AND A2_LOJA = C7_LOJA and sa2.D_E_L_E_T_ = '' ";
            query += "WHERE scr.D_E_L_E_T_ = '' ";
            query += "GROUP BY CR_NUM,CR_MOEDA,CR_STATUS,C7_CONAPRO,C7_FORNECE,C7_LOJA,A2_NOME,CR_TOTAL,CR_EMISSAO,CR_DATALIB,CR_USER ";
            query += ") AS ped ";
            query += "where ped.Empresa = @Empresa and ped.Usuario in('" + usuario.CODERP.Replace(",", "','") + "') and ped.Pedido = @Pedido ";

            //#### Filtros
            parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@Empresa", empresa));
            parametros.Add(new SqlParameter("@Pedido", pedido));
            #endregion Query
            pedidoCustom = dbProtheus.Database.SqlQuery<PedidoCustom>(query, parametros.ToArray()).First();
            return View(pedidoCustom);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult AprovPed(PedidoCustom pedidoCustom)
        {
            var usuario = dbSgi.T_Usuario.First(x => x.EMAIL == HttpContext.User.Identity.Name);
            bool valido = true;
            string status = Request["aprovacao"] == "N" ? "04" : "03";
            #region RegistraAuditoria
            var auditoria = new T_Auditoria();
            auditoria.ID_USUARIO = usuario.ID_USUARIO;
            auditoria.DATA = DateTime.Now;
            auditoria.ROTINA = "Aprovação Pedido de Compras";
            auditoria.CHAVE = pedidoCustom.Pedido + pedidoCustom.Loja + pedidoCustom.Fornecedor;
            auditoria.HISTORICO = "Empresa: " + pedidoCustom.Empresa + " Pedido: " + pedidoCustom.Pedido;
            auditoria.HISTORICO += " Fornecedor: " + pedidoCustom.Fornecedor + pedidoCustom.Loja + pedidoCustom.ForNome;
            #endregion RegistraAuditoria

            #region Validacoes
            if (Request["aprovacao"] == "N" && String.IsNullOrEmpty(pedidoCustom.MotivoRejeicao))
            {
                ModelState.AddModelError("MotivoRejeicao", "Obrigatório informar o motivo da rejeição.");
                ViewBag.alerta = "Por favor preencha a justificativa referente a não aprovação.";
                valido = false;
            }
            #endregion Validacoes

            if (valido)
            {
                //Confirmação de aprovação
                #region ConfirmaAprovacao
                var tabela = pedidoCustom.Empresa == "Jaepel" ? "SCR010" : "SCR020";
                var query = "UPDATE " + tabela + " set CR_STATUS = @Status,CR_DATALIB = @DtLib ";
                query += ",CR_XOBSAPT = CONVERT(VARBINARY(MAX), @ObsAprovacao) ";
                query += ",CR_XJUSREJ = CONVERT(VARBINARY(MAX), @Justificativa) ";
                query += "WHERE D_E_L_E_T_ = '' AND CR_NUM = @Pedido and CR_USER in('" + usuario.CODERP.Replace(",", "','") + "')";
                List<SqlParameter> parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@Status", status));//Seta status como Liberado pelo usuário
                parametros.Add(new SqlParameter("@DtLib", DateTime.Now.ToString("yyyyMMdd")));
                parametros.Add(new SqlParameter("@Pedido", pedidoCustom.Pedido));
                parametros.Add(new SqlParameter("@ObsAprovacao", string.IsNullOrEmpty(pedidoCustom.ObsAprovacao) ? "" : pedidoCustom.ObsAprovacao));
                parametros.Add(new SqlParameter("@Justificativa", string.IsNullOrEmpty(pedidoCustom.MotivoRejeicao) ? "" : pedidoCustom.MotivoRejeicao));
                //Executa comando no banco de dados
                dbProtheus.Database.ExecuteSqlCommand(query, parametros.ToArray());
                //Chama rotina para poder realizar aprovação de pedidos
                if (AprovacaoTotalPedido(pedidoCustom.Pedido, pedidoCustom.Empresa))
                {
                    string eMails = "";

                    //Obtem e-mail do comprador
                    if (pedidoCustom.Comprador != null)
                        if (string.IsNullOrEmpty(pedidoCustom.Comprador.Email))
                            eMails += pedidoCustom.Aprovador.Email + ";";

                    //Obtem e-mail do solicitante
                    if (pedidoCustom.Solicitante != null)
                        if (string.IsNullOrEmpty(pedidoCustom.Solicitante.Email))
                            eMails += pedidoCustom.Solicitante.Email + ";";

                    if (!string.IsNullOrEmpty(eMails))
                        Util.EnviaEmail.EnviarEmail("Aprovação Pedido de compras", eMails, Util.EnviaEmail.TextoEmailPedido(pedidoCustom), pedidoCustom.Empresa);
                }
                Util.Auditoria.Registrar(auditoria);
                #endregion ConfirmaAprovacao
                ///Criar metódo para avaliar se o pedido está 100% aprovado alterar SC7 C7_CONAPRO = 'L'
                return Json(new { success = true });
            }
            else
            {
                pedidoCustom.Emissao = DateTime.ParseExact(pedidoCustom.Emissao, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyyMMdd");
                return View(pedidoCustom);
            }
        }
        #endregion PedidoDeCompras

        #region NotasFiscais
        public ActionResult NfEntrada(string searchString, int? nPageSize, int? page, string pStatus, string pEmpresa = "")
        {
            #region DeclaracoesVariaveis
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            ViewBag.Status = new SelectList(new List<string> { "Pendente", "Aprovado", "Todos" }, selectedValue: pStatus);
            ViewBag.Empresa = new SelectList(new List<string> { "Jaepel", "Florestal" }, selectedValue: pEmpresa);
            T_Usuario usuario = dbSgi.T_Usuario.First(x => x.EMAIL == HttpContext.User.Identity.Name);
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 20;
            var nfe = new List<NfeCustom>();
            List<SqlParameter> parametros = new List<SqlParameter>();
            #endregion DeclaracoesVariaveis
            //Executa busca de dados query customizada
            #region Querys
            var query = "select Empresa,Origem,Status,Emissao,Numero,Serie,Fornecedor,Loja,NomeFor,VlrBruto,VlrMerc,Recno ";
            query += "from( select 'Jaepel' Empresa,'Compras' Origem,rtrim(ZC8_STATUS) Status,RTRIM(ZC8_NIVEL) Nivel,rtrim(AL_USER)Usuario,F1_EMISSAO Emissao,rtrim(F1_DOC) Numero, ";
            query += "rtrim(F1_SERIE) Serie,rtrim(F1_FORNECE) Fornecedor,F1_LOJA Loja,rtrim(A2_NOME) NomeFor,F1_VALBRUT VlrBruto,F1_VALMERC VlrMerc,sf1.R_E_C_N_O_ Recno ";
            query += "from SF1010 sf1 ";
            query += "inner join ZC8010 zc8 on ZC8_NFISCA = F1_DOC AND ZC8_SERIE = F1_SERIE AND ZC8_FORNEC = F1_FORNECE AND ZC8_LOJA = F1_LOJA AND zc8.D_E_L_E_T_ = '' and ZC8_FILIAL = '' AND zc8.ZC8_ORIGEM = 'COMPRAS' ";
            query += "inner join SA2010 sa2 on A2_COD = F1_FORNECE and A2_LOJA = F1_LOJA AND sa2.D_E_L_E_T_ = ' '  and A2_FILIAL = '' ";
            query += "inner join SAL010 sal on AL_APROV = ZC8_APROV AND sal.D_E_L_E_T_ = '' and AL_FILIAL = '01' ";
            query += "where sf1.D_E_L_E_T_ = ' ' and sf1.F1_FILIAL = '01' ";
            query += "and (zc8.ZC8_NIVEL = (SELECT TOP 1 zc.ZC8_NIVEL FROM ZC8010 zc WHERE zc.ZC8_NFISCA = F1_DOC AND zc.ZC8_SERIE = F1_SERIE AND zc.ZC8_FORNEC = F1_FORNECE AND ZC8_LOJA = F1_LOJA AND zc.D_E_L_E_T_ = '' AND zc.ZC8_STATUS = 'N' and zc.ZC8_FILIAL = '' order by ZC8_NIVEL) or zc8.ZC8_STATUS = 'S') ";
            query += "union all ";
            //Query buscar financeiro Jaepel 
            query += "select 'Jaepel' Empresa, 'Financeiro' Origem,rtrim(ZC8_STATUS) Status,RTRIM(ZC8_NIVEL) Nivel,rtrim(AL_USER)Usuario,rtrim(E2_EMISSAO) Emissao,rtrim(E2_NUM) Numero,RTRIM(E2_PREFIXO) Serie ";
            query += ",RTRIM(E2_FORNECE) Fornecedor,rtrim(E2_LOJA) Loja,RTRIM(E2_NOMFOR)NomeFor,E2_VALOR VlrBruto,E2_VALOR VlrMerc,se2.R_E_C_N_O_ Recno ";
            query += "from SE2010 se2 ";
            query += "inner join ZC8010 zc8 on ZC8_TIPO = E2_TIPO and ZC8_NFISCA = E2_NUM AND ZC8_SERIE = E2_PREFIXO AND ZC8_FORNEC = E2_FORNECE AND ZC8_LOJA = E2_LOJA AND zc8.D_E_L_E_T_ = '' and ZC8_FILIAL = '' AND zc8.ZC8_ORIGEM = 'FINANCEIRO' ";
            query += "inner join SAL010 sal on AL_APROV = ZC8_APROV AND sal.D_E_L_E_T_ = '' and AL_FILIAL = '01' ";
            query += "WHERE se2.D_E_L_E_T_ = '' ";
            query += "and (zc8.ZC8_NIVEL = (SELECT TOP 1 zc.ZC8_NIVEL FROM ZC8010 zc WHERE zc.ZC8_TIPO = E2_TIPO and zc.ZC8_NFISCA = E2_NUM AND zc.ZC8_SERIE = E2_PREFIXO AND zc.ZC8_FORNEC = E2_FORNECE AND ZC8_LOJA = E2_LOJA AND zc.D_E_L_E_T_ = '' AND zc.ZC8_STATUS = 'N' and zc.ZC8_FILIAL = '' order by ZC8_NIVEL) or zc8.ZC8_STATUS = 'S') ";
            query += "union all ";
            //Query buscar Compras Florestal 
            query += "select 'Florestal' Empresa,'Compras' Origem,rtrim(ZC8_STATUS) Status,RTRIM(ZC8_NIVEL) Nivel,rtrim(AL_USER)Usuario,F1_EMISSAO Emissao,rtrim(F1_DOC) Numero,rtrim(F1_SERIE) Serie, ";
            query += "rtrim(F1_FORNECE) Fornecedor,rtrim(F1_LOJA) Loja,rtrim(A2_NOME) NomeFor,F1_VALBRUT VlrBruto,F1_VALMERC VlrMerc,sf1.R_E_C_N_O_ Recno ";
            query += "from SF1020 sf1  ";
            query += "inner join ZC8020 zc8 on ZC8_NFISCA = F1_DOC AND ZC8_SERIE = F1_SERIE AND ZC8_FORNEC = F1_FORNECE AND ZC8_LOJA = F1_LOJA AND zc8.D_E_L_E_T_ = '' AND ZC8_FILIAL = '' AND zc8.ZC8_ORIGEM = 'COMPRAS' ";
            query += "inner join SA2020 sa2 on A2_COD = F1_FORNECE and A2_LOJA = F1_LOJA AND sa2.D_E_L_E_T_ = ' ' and A2_FILIAL = '' ";
            query += "inner join SAL020 sal on AL_APROV = ZC8_APROV AND sal.D_E_L_E_T_ = '' and AL_FILIAL = '01' ";
            query += "where sf1.D_E_L_E_T_ = ' ' and sf1.F1_FILIAL = '01' ";
            query += "and (zc8.ZC8_NIVEL = (SELECT TOP 1 zc.ZC8_NIVEL FROM ZC8020 zc WHERE zc.ZC8_NFISCA = F1_DOC AND zc.ZC8_SERIE = F1_SERIE AND zc.ZC8_FORNEC = F1_FORNECE AND ZC8_LOJA = F1_LOJA AND zc.D_E_L_E_T_ = '' AND zc.ZC8_STATUS = 'N' and zc.ZC8_FILIAL = '' order by ZC8_NIVEL) or zc8.ZC8_STATUS = 'S') ";
            query += "union all ";
            //Query buscar Financeiro Florestal
            query += "select 'Florestal' Empresa, 'Financeiro' Origem,rtrim(ZC8_STATUS) Status,RTRIM(ZC8_NIVEL) Nivel,rtrim(AL_USER)Usuario,rtrim(E2_EMISSAO) Emissao,rtrim(E2_NUM) Numero,RTRIM(E2_PREFIXO) Serie ";
            query += ",RTRIM(E2_FORNECE) Fornecedor,rtrim(E2_LOJA) Loja,RTRIM(E2_NOMFOR)NomeFor,E2_VALOR VlrBruto,E2_VALOR VlrMerc,se2.R_E_C_N_O_ Recno ";
            query += "from SE2020 se2 ";
            query += "inner join ZC8020 zc8 on ZC8_TIPO = E2_TIPO and ZC8_NFISCA = E2_NUM AND ZC8_SERIE = E2_PREFIXO AND ZC8_FORNEC = E2_FORNECE AND ZC8_LOJA = E2_LOJA AND zc8.D_E_L_E_T_ = '' and ZC8_FILIAL = '' AND zc8.ZC8_ORIGEM = 'FINANCEIRO' ";
            query += "inner join SAL020 sal on AL_APROV = ZC8_APROV AND sal.D_E_L_E_T_ = '' and AL_FILIAL = '01'  ";
            query += "WHERE se2.D_E_L_E_T_ = '' ";
            query += "and (zc8.ZC8_NIVEL = (SELECT TOP 1 zc.ZC8_NIVEL FROM ZC8020 zc WHERE zc.ZC8_TIPO = E2_TIPO and zc.ZC8_NFISCA = E2_NUM AND zc.ZC8_SERIE = E2_PREFIXO AND zc.ZC8_FORNEC = E2_FORNECE AND ZC8_LOJA = E2_LOJA AND zc.D_E_L_E_T_ = '' AND zc.ZC8_STATUS = 'N' and zc.ZC8_FILIAL = '' order by ZC8_NIVEL) or zc8.ZC8_STATUS = 'S') ";
            query += ") as p  ";
            query += "where Usuario in('" + (!String.IsNullOrEmpty(usuario.CODERP) ? usuario.CODERP.Replace(",", "','") : "") + "') ";

            //Filtra pesquisa
            if (!String.IsNullOrEmpty(searchString))
            {
                query += " and upper(Numero+Serie+Fornecedor+Loja+ForNome) LIKE @search ";
                parametros.Add(new SqlParameter("@search", "%" + searchString.ToUpper() + "%"));
            }

            //Filtra por empresa
            if (!string.IsNullOrEmpty(pEmpresa))
            {
                query += " and Empresa = @Empresa ";
                parametros.Add(new SqlParameter("@Empresa", pEmpresa));
            }

            //Filtra por status
            if (!string.IsNullOrEmpty(pStatus))
            {
                if (pStatus == "Pendente")
                {
                    query += " and Status = @Status ";
                    parametros.Add(new SqlParameter("@Status", "N"));
                }
                else if (pStatus == "Aprovado")
                {
                    query += " and Status = @Status ";
                    parametros.Add(new SqlParameter("@Status", "S"));
                }
            }
            else
            {
                query += " and Status = @Status ";
                parametros.Add(new SqlParameter("@Status", "N"));
            }

            query += "GROUP BY Empresa,Origem,Status,Emissao,Numero,Serie,Fornecedor,Loja,NomeFor,VlrBruto,VlrMerc,Recno ";
            #endregion Querys

            nfe = dbProtheus.Database.SqlQuery<NfeCustom>(query, parametros.ToArray()).ToList();
            return View(nfe.OrderByDescending(x => x.Empresa + x.Emissao).ToPagedList(_PageNumber, _PageSize));
        }

        public ActionResult NfeDetails(int Recno, string empresa, string searchString, int? nPageSize, int? page, string pStatus)
        {
            var nfe = new NfeCustom();
            nfe = GetDadosAprovacaoNf(empresa, Recno);
            return View(nfe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult NfeDetails(NfeCustom nfeCustom)
        {
            var usuario = dbSgi.T_Usuario.First(x => x.EMAIL == HttpContext.User.Identity.Name);
            //Confirmação de aprovação
            #region ConfirmaAprovacao
            var tabela = nfeCustom.Empresa == "Jaepel" ? "ZC8010" : "ZC8020";
            var query = "";
            if (nfeCustom.Origem == "Compras")
            {
                query = "UPDATE " + tabela + " set ZC8_STATUS = @Status,ZC8_DTAPRO = @DtLib from " + tabela + " zc8 ";
                query += "inner join " + (nfeCustom.Empresa == "Jaepel" ? "SF1010" : "SF1020") + " sf1 on ZC8_NFISCA = F1_DOC AND ZC8_SERIE = F1_SERIE AND ZC8_FORNEC = F1_FORNECE AND ZC8_LOJA = F1_LOJA AND sf1.D_E_L_E_T_ = '' and ZC8_FILIAL = '' ";
                query += "inner join " + (nfeCustom.Empresa == "Jaepel" ? "SAL010" : "SAL020") + " sal on AL_APROV = ZC8_APROV AND sal.D_E_L_E_T_ = '' and AL_FILIAL = '01' ";
                query += "where zc8.D_E_L_E_T_ = '' and se2.R_E_C_N_O_ = @Recno and AL_USER in('" + usuario.CODERP.Replace(",", "','") + "') AND zc8.ZC8_ORIGEM = 'COMPRAS' ";
            }
            else if (nfeCustom.Origem == "Financeiro")
            {
                query = "UPDATE " + tabela + " set ZC8_STATUS = @Status,ZC8_DTAPRO = @DtLib from " + tabela + " zc8 ";
                query += "inner join " + (nfeCustom.Empresa == "Jaepel" ? "SE2010" : "SE2020") + " se2 on ZC8_TIPO = E2_TIPO AND ZC8_NFISCA = E2_NUM AND ZC8_SERIE = E2_PREFIXO AND ZC8_FORNEC = E2_FORNECE AND ZC8_LOJA = E2_LOJA AND se2.D_E_L_E_T_ = '' and E2_FILIAL = '01' ";
                query += "inner join " + (nfeCustom.Empresa == "Jaepel" ? "SAL010" : "SAL020") + " sal on AL_APROV = ZC8_APROV AND sal.D_E_L_E_T_ = '' and AL_FILIAL = '01' ";
                query += "where zc8.D_E_L_E_T_ = '' and se2.R_E_C_N_O_ = @Recno and AL_USER in('" + usuario.CODERP.Replace(",", "','") + "') AND zc8.ZC8_ORIGEM = 'FINANCEIRO' ";
            }
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@Status", "S"));//Seta status como Liberado pelo usuário
            parametros.Add(new SqlParameter("@DtLib", DateTime.Now.ToString("yyyyMMdd")));
            parametros.Add(new SqlParameter("@Recno", nfeCustom.Recno));
            //Executa comando no banco de dados
            dbProtheus.Database.ExecuteSqlCommand(query, parametros.ToArray());
            //Chama rotina para poder realizar aprovação de pedidos
            if (AprovacaoTotalNf(nfeCustom.Recno, nfeCustom.Origem, nfeCustom.Empresa))
            {
                //Colocar aqui e-mails para aprovação
            }
            //Util.Auditoria.Registrar(auditoria);
            #endregion ConfirmaAprovacao
            ///Criar metódo para avaliar se o pedido está 100% aprovado alterar SC7 C7_CONAPRO = 'L'
            return RedirectToAction("NfEntrada");
        }

        public ActionResult AprovNf(int Recno, string Empresa)
        {
            NfeCustom nfe = new NfeCustom();
            List<SqlParameter> parametros = new List<SqlParameter>();
            string query = "";

            //Query Busca cabeçalho da nota fiscal
            #region CabecNf
            query = "select * from ( ";
            query += "select 'Jaepel' Empresa,'Compras' Origem,sf1.R_E_C_N_O_ Recno,sf1.F1_MOEDA Moeda,rtrim(sf1.F1_MSBLQL) Status,rtrim(sf1.F1_EMISSAO) Emissao,rtrim(sf1.F1_DOC) Numero, ";
            query += "rtrim(sf1.F1_SERIE) Serie,rtrim(sf1.F1_FORNECE) Fornecedor,rtrim(sf1.F1_LOJA) Loja,rtrim(A2_NOME) NomeFor,sf1.F1_VALBRUT VlrBruto,sf1.F1_VALMERC VlrMerc ";
            query += ",sf1.F1_DESCONT Desconto,sf1.F1_FRETE Frete,rtrim(se4.E4_DESCRI) CondPag,rtrim(sf1.F1_TPFRETE) TipoFrete ";
            query += "from SF1010 sf1 ";
            query += "inner join SA2010 sa2 on A2_COD = F1_FORNECE AND A2_LOJA = F1_LOJA and sa2.D_E_L_E_T_ = '' ";
            query += "inner join SE4010 se4 on E4_CODIGO = sf1.F1_COND and se4.D_E_L_E_T_ = '' ";
            query += "union all  ";
            //Busca dados financeiro Jaepel
            query += "select 'Jaepel'Empresa,'Financeiro' Origem,se2.R_E_C_N_O_ Recno,E2_MOEDA Moeda,E2_MSBLQL Status,RTRIM(E2_EMISSAO)Emissao ";
            query += ",RTRIM(E2_NUM) Numero,RTRIM(E2_PREFIXO)Serie,RTRIM(E2_FORNECE)Fornecedor,RTRIM(E2_LOJA)Loja,RTRIM(E2_NOMFOR)NomeFor,E2_VALOR VlrBruto,E2_VALOR VlrMerc, ";
            query += "E2_DESCONT Desconto,0 Frete,RTRIM(E2_TIPO)CondPag,'' TipoFrete ";
            query += "from SE2010 se2 ";
            query += "where se2.D_E_L_E_T_ = '' ";
            //Busca dados compras Florestal
            query += "union all  ";
            query += "select 'Florestal' Empresa,'Compras' Origem,sf1.R_E_C_N_O_ Recno,sf1.F1_MOEDA Moeda,rtrim(sf1.F1_MSBLQL) Status,rtrim(sf1.F1_EMISSAO) Emissao,rtrim(sf1.F1_DOC) Numero, ";
            query += "rtrim(sf1.F1_SERIE) Serie,rtrim(sf1.F1_FORNECE) Fornecedor,rtrim(sf1.F1_LOJA) Loja,rtrim(A2_NOME) NomeFor,sf1.F1_VALBRUT VlrBruto,sf1.F1_VALMERC VlrMerc ";
            query += ",sf1.F1_DESCONT Desconto,sf1.F1_FRETE Frete,rtrim(se4.E4_DESCRI) CondPag,rtrim(sf1.F1_TPFRETE) TipoFrete ";
            query += "from SF1020 sf1 ";
            query += "inner join SA2020 sa2 on A2_COD = F1_FORNECE AND A2_LOJA = F1_LOJA and sa2.D_E_L_E_T_ = '' ";
            query += "inner join SE4020 se4 on E4_CODIGO = sf1.F1_COND and se4.D_E_L_E_T_ = '' ";
            query += "union all  ";
            //Busca dados financeiro Florestal
            query += "select 'Florestal'Empresa,'Financeiro' Origem,se2.R_E_C_N_O_ Recno,E2_MOEDA Moeda,E2_MSBLQL Status,RTRIM(E2_EMISSAO)Emissao ";
            query += ",RTRIM(E2_NUM) Numero,RTRIM(E2_PREFIXO)Serie,RTRIM(E2_FORNECE)Fornecedor,RTRIM(E2_LOJA)Loja,RTRIM(E2_NOMFOR)NomeFor,E2_VALOR VlrBruto,E2_VALOR VlrMerc, ";
            query += "E2_DESCONT Desconto,0 Frete,RTRIM(E2_TIPO)CondPag,'' TipoFrete ";
            query += "from SE2020 se2 ";
            query += "where se2.D_E_L_E_T_ = '' ";
            query += ") as nf  ";
            query += "where nf.Recno = @Recno and nf.Empresa = @Empresa ";
            //Adiciona filtros
            parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@Recno", Recno));
            parametros.Add(new SqlParameter("@Empresa", Empresa));
            //Executa query no banco de dados
            nfe = dbProtheus.Database.SqlQuery<NfeCustom>(query, parametros.ToArray()).FirstOrDefault();
            if (nfe == null)
                nfe = new NfeCustom();
            #endregion CabecNf
            return View(nfe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult AprovNf(NfeCustom nfeCustom)
        {
            var usuario = dbSgi.T_Usuario.First(x => x.EMAIL == HttpContext.User.Identity.Name);
            ////Confirmação de aprovação
            #region ConfirmaAprovacao
            var tabela = nfeCustom.Empresa == "Jaepel" ? "ZC8010" : "ZC8020";
            var query = "UPDATE " + tabela + " set ZC8_STATUS = @Status,ZC8_DTAPRO = @DtLib from " + tabela + " zc8 ";
            if (nfeCustom.Origem == "Compras")
            {
                query += "inner join " + (nfeCustom.Empresa == "Jaepel" ? "SF1010" : "SF1020") + " sf1 on ZC8_NFISCA = F1_DOC AND ZC8_SERIE = F1_SERIE AND ZC8_FORNEC = F1_FORNECE AND ZC8_LOJA = F1_LOJA AND zc8.D_E_L_E_T_ = '' and ZC8_FILIAL = '' AND zc8.ZC8_ORIGEM = 'COMPRAS' ";
            }
            else if (nfeCustom.Origem == "Financeiro")
            {
                query += "inner join " + (nfeCustom.Empresa == "Jaepel" ? "SE2010" : "SE2020") + " sf1 on ZC8_TIPO = E2_TIPO AND ZC8_NFISCA = E2_NUM AND ZC8_SERIE = E2_PREFIXO AND ZC8_FORNEC = E2_FORNECE AND ZC8_LOJA = E2_LOJA AND zc8.D_E_L_E_T_ = '' and ZC8_FILIAL = '' AND zc8.ZC8_ORIGEM = 'FINANCEIRO' ";
            }
            query += "inner join " + (nfeCustom.Empresa == "Jaepel" ? "SAL010" : "SAL020") + " sal on AL_APROV = ZC8_APROV AND sal.D_E_L_E_T_ = '' and AL_FILIAL = '01' ";
            query += "where sf1.R_E_C_N_O_ = @Recno and AL_USER in('" + usuario.CODERP.Replace(",", "','") + "') ";
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@Status", "S"));//Seta status como Liberado pelo usuário
            parametros.Add(new SqlParameter("@DtLib", DateTime.Now.ToString("yyyyMMdd")));
            parametros.Add(new SqlParameter("@Recno", nfeCustom.Recno));
            //Executa comando no banco de dados
            dbProtheus.Database.ExecuteSqlCommand(query, parametros.ToArray());
            //Chama rotina para poder realizar aprovação de pedidos
            if (AprovacaoTotalNf(nfeCustom.Recno, nfeCustom.Origem, nfeCustom.Empresa))
            {
                //Colocar aqui e-mails para aprovação
            }
            //Util.Auditoria.Registrar(auditoria);
            #endregion ConfirmaAprovacao
            return Json(new { success = true });
        }
        #endregion NotasFiscais

        #region MetodosUteis
        /// <summary>
        /// Metódo para retornar objeto pedido preenchido com dados obtidos do banco
        /// </summary>
        /// <param name="empresa">Empresa Jaepel ou Florestal?</param>
        /// <param name="pedido">Nº do pedido</param>
        /// <returns>Objeto do tipo PedidoCustom</returns>
        public PedidoCustom GetDadosAprovacaoPedido(string empresa, string pedido)
        {
            PedidoCustom pedidoCustom = new PedidoCustom();
            T_Usuario usuario = dbSgi.T_Usuario.First(x => x.EMAIL == HttpContext.User.Identity.Name);
            string query = "";
            var parametros = new List<SqlParameter>();
            #region MontaDados
            //Filtra pedido
            #region QueryPedido
            query = "SELECT * FROM( ";
            query += "select 'Jaepel' Empresa,C7_MOEDA Moeda,rtrim(C7_USER)UserAprovador,rtrim(CR_STATUS) StatusSCR,rtrim(C7_CONAPRO) StatusSC7,rtrim(CR_USER) Usuario,CR_EMISSAO Emissao,rtrim(CR_DATALIB) DtLiberacao,rtrim(CR_NUM) Pedido,rtrim(C7_FORNECE) Fornecedor,";
            query += "rtrim(C7_LOJA) Loja,rtrim(A2_NOME) ForNome,CR_TOTAL Total,rtrim(E4_DESCRI) CondPag,C1_CONAPRO AprovadorSc ";
            query += "from SCR010 scr ";
            query += "LEFT JOIN SC7010 sc7 ON C7_NUM = CR_NUM AND sc7.D_E_L_E_T_ = '' ";
            query += "inner join SA2010 sa2 on A2_COD = C7_FORNECE AND A2_LOJA = C7_LOJA and sa2.D_E_L_E_T_ = '' ";
            query += "left join SE4010 se4 on E4_CODIGO = C7_COND and se4.D_E_L_E_T_ = '' ";
            query += "left join SC1010 sc1 on C1_PEDIDO = CR_NUM AND sc1.D_E_L_E_T_ = '' ";
            query += "WHERE  scr.D_E_L_E_T_ = '' ";
            query += "GROUP BY CR_NUM,C7_USER,C7_MOEDA,CR_STATUS,C7_CONAPRO,C7_FORNECE,C7_LOJA,A2_NOME,CR_TOTAL,CR_EMISSAO,CR_DATALIB,CR_USER,E4_DESCRI,C1_CONAPRO ";
            query += "UNION ALL ";
            query += "select 'Florestal' Empresa,C7_MOEDA Moeda,rtrim(C7_USER)UserAprovador,rtrim(CR_STATUS) StatusSCR,rtrim(C7_CONAPRO) StatusSC7,rtrim(CR_USER) Usuario,CR_EMISSAO EMISSAO,rtrim(CR_DATALIB) DTLIBERACAO,rtrim(CR_NUM) PEDIDO,rtrim(C7_FORNECE) FORNECEDOR,";
            query += "rtrim(C7_LOJA) LOJA,rtrim(A2_NOME) FORNOME,CR_TOTAL TOTAL,rtrim(E4_DESCRI) CondPag,C1_CONAPRO AprovadorSc ";
            query += "from SCR020 scr ";
            query += "LEFT JOIN SC7020 sc7 ON C7_NUM = CR_NUM AND sc7.D_E_L_E_T_ = '' ";
            query += "inner join SA2020 sa2 on A2_COD = C7_FORNECE AND A2_LOJA = C7_LOJA and sa2.D_E_L_E_T_ = '' ";
            query += "left join SE4020 se4 on E4_CODIGO = C7_COND and se4.D_E_L_E_T_ = '' ";
            query += "left join SC1020 sc1 on C1_PEDIDO = CR_NUM AND sc1.D_E_L_E_T_ = '' ";
            query += "WHERE scr.D_E_L_E_T_ = '' ";
            query += "GROUP BY CR_NUM,C7_USER,C7_MOEDA,CR_STATUS,C7_CONAPRO,C7_FORNECE,C7_LOJA,A2_NOME,CR_TOTAL,CR_EMISSAO,CR_DATALIB,CR_USER,E4_DESCRI,C1_CONAPRO ";
            query += ") AS ped ";
            query += "where ped.Empresa = @Empresa and ped.Usuario in('" + usuario.CODERP.Replace(",", "','") + "') and ped.Pedido = @Pedido ";

            //#### Filtros
            parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@Empresa", empresa));
            parametros.Add(new SqlParameter("@Pedido", pedido));
            pedidoCustom = dbProtheus.Database.SqlQuery<PedidoCustom>(query, parametros.ToArray()).FirstOrDefault();
            if (pedidoCustom == null)
                pedidoCustom = new PedidoCustom();
            #endregion QueryPedido

            //Filtra itens do pedido
            #region QueryItensPedido
            query = "select * from ( ";
            query += "select 'Jaepel' Empresa,RTRIM(C7_NUM) Pedido,RTRIM(C7_ITEM)Item,RTRIM(C7_PRODUTO)CodProduto,RTRIM(B1_DESC)Produto,RTRIM(C7_UM) UN,C7_QUANT Qtd,C7_PRECO Preco,C7_IPI Ipi ";
            query += ",C7_TOTAL Total,C7_DESC ValDesc,C7_DESPESA ValDesp,RTRIM(C7_DATPRF)DtEntrega,B1_UPRC UltPrcCopra,rtrim(B1_UCOM)DtUlPrc,RTRIM(C7_CC) CCusto,RTRIM(C7_NUMSC)NumSc ";
            query += ",ISNULL(CONVERT(VARCHAR(MAX), CONVERT(VARBINARY(MAX), C7_JUSPE2)),'') Justificativa,ISNULL(CONVERT(VARCHAR(MAX), CONVERT(VARBINARY(MAX), C7_XJUSSOL)),'') EspTecnica,RTRIM(C7_OBS)Obs ";
            query += "from SC7010 sc7 ";
            query += "inner join SB1010 sb1 on B1_COD = C7_PRODUTO AND sb1.D_E_L_E_T_ = '' ";
            query += "INNER JOIN SCR010 scr on CR_NUM = C7_NUM AND scr.D_E_L_E_T_ = '' AND CR_USER in('" + usuario.CODERP.Replace(",", "','") + "') ";
            query += "inner join SAL010 sal on AL_APROV = CR_APROV and AL_USER = CR_USER AND sal.D_E_L_E_T_ = '' ";
            query += "where sc7.D_E_L_E_T_ = '' AND (C7_CC = AL_X_CC OR B1_GRUPO = AL_GRPTEC) ";
            query += "UNION ALL ";
            query += "select 'Florestal' Empresa,RTRIM(C7_NUM) Pedido,RTRIM(C7_ITEM)Item,RTRIM(C7_PRODUTO)CodProduto,RTRIM(B1_DESC)Produto,RTRIM(C7_UM) UN,C7_QUANT Qtd,C7_PRECO Preco,C7_IPI Ipi ";
            query += ",C7_TOTAL Total,C7_DESC ValDesc,C7_DESPESA ValDesp,RTRIM(C7_DATPRF)DtEntrega,B1_UPRC UltPrcCopra,rtrim(B1_UCOM)DtUlPrc,RTRIM(C7_CC) CCusto,RTRIM(C7_NUMSC)NumSc ";
            query += ",ISNULL(CONVERT(VARCHAR(MAX), CONVERT(VARBINARY(MAX), C7_JUSPE2)),'') Justificativa,ISNULL(CONVERT(VARCHAR(MAX), CONVERT(VARBINARY(MAX), C7_XJUSSOL)),'') EspTecnica,RTRIM(C7_OBS)Obs ";
            query += "from SC7020 sc7 ";
            query += "inner join SB1020 sb1 on B1_COD = C7_PRODUTO AND sb1.D_E_L_E_T_ = '' ";
            query += "INNER JOIN SCR020 scr on CR_NUM = C7_NUM AND scr.D_E_L_E_T_ = '' AND CR_USER in('" + usuario.CODERP.Replace(",", "','") + "') ";
            query += "inner join SAL020 sal on AL_APROV = CR_APROV and AL_USER = CR_USER AND sal.D_E_L_E_T_ = '' ";
            query += "where sc7.D_E_L_E_T_ = '' AND (C7_CC = AL_X_CC OR B1_GRUPO = AL_GRPTEC)) as It ";
            query += "where It.Empresa = @Empresa and It.Pedido = @Pedido ";
            query += "group by Empresa,Pedido,Item,CodProduto,Produto,UN,Qtd,Preco,Ipi,Total,ValDesc,ValDesp,DtEntrega,DtUlPrc,UltPrcCopra,CCusto,NumSc,Justificativa,EspTecnica,Obs ";
            query += "order by It.Empresa,It.Pedido,It.Item ";

            //#### Filtros
            parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@Empresa", empresa));
            parametros.Add(new SqlParameter("@Pedido", pedido));
            pedidoCustom.ItensPedCompra = dbProtheus.Database.SqlQuery<ItensPedCompraCustom>(query, parametros.ToArray()).ToList();
            #endregion QueryItensPedido

            //Busca comprador
            #region QueryComprador
            query = "select * from ( ";
            query += "select rtrim(Y1_USER)Usuario,rtrim(Y1_COD) Codigo,rtrim(Y1_NOME) Nome,rtrim(Y1_EMAIL)Email from SY1010 ";
            query += "WHERE D_E_L_E_T_ = '' ";
            query += "union all ";
            query += "select rtrim(Y1_USER)Usuario,rtrim(Y1_COD) Codigo,rtrim(Y1_NOME) Nome,rtrim(Y1_EMAIL)Email from SY1020 ";
            query += "WHERE D_E_L_E_T_ = '') as aprov ";
            query += "where Usuario = @UserComprador ";

            parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@UserComprador", pedidoCustom.UserAprovador));
            pedidoCustom.Comprador = dbProtheus.Database.SqlQuery<CompradorCustom>(query, parametros.ToArray()).FirstOrDefault();
            #endregion QueryComprador

            //Filtra cotações
            #region QueryCotacao
            query = "SELECT * FROM (select C8_ITEM Item,C8_NUMPED Numero,C8_PRODUTO CodProduto,B1_DESC Produto,C8_UM Un,C8_QUANT Qtd,C8_PRECO Preco,C8_TOTAL Total,C8_VALIPI ValIpi,";
            query += "C8_VALICM ValIcms,C8_VALFRE ValFrete,C8_FORNECE Fornecedor,C8_LOJA Loja,A2_NOME ForNome ";
            query += "from SC8010 sc8 ";
            query += "INNER JOIN SB1010 sb1 on B1_COD = C8_PRODUTO AND sb1.D_E_L_E_T_ = '' ";
            query += "inner join SA2010 sa2 on A2_COD = C8_FORNECE AND A2_LOJA = C8_LOJA and sa2.D_E_L_E_T_ = '' ";
            query += "where sc8.D_E_L_E_T_ = '' ";
            query += "UNION ALL ";
            query += "select C8_ITEM Item,C8_NUMPED Numero,C8_PRODUTO CodProduto,B1_DESC Produto,C8_UM Un,C8_QUANT Qtd,C8_PRECO Preco,C8_TOTAL Total,C8_VALIPI ValIpi,";
            query += "C8_VALICM ValIcms,C8_VALFRE ValFrete,C8_FORNECE Fornecedor,C8_LOJA Loja,A2_NOME ForNome ";
            query += "from SC8020 sc8 ";
            query += "INNER JOIN SB1020 sb1 on B1_COD = C8_PRODUTO AND sb1.D_E_L_E_T_ = '' ";
            query += "inner join SA2020 sa2 on A2_COD = C8_FORNECE AND A2_LOJA = C8_LOJA and sa2.D_E_L_E_T_ = '' ";
            query += "where sc8.D_E_L_E_T_ = '') as cot ";
            query += "where cot.Numero = @Pedido ";
            query += "";

            //#### Filtros
            parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@Pedido", pedido));
            pedidoCustom.Cotacoes = dbProtheus.Database.SqlQuery<CotacaoCustom>(query, parametros.ToArray()).ToList();
            #endregion QueryCotacao

            //Filtra processo de aprovação
            #region QueryAprovadores
            query = "select * from (select 'Jaepel' Empresa,rtrim(CR_NUM) Pedido,rtrim(CR_USER) Codigo,rtrim(AK_NOME) Nome,CR_NIVEL Nivel,rtrim(X5_DESCRI) Tipo,rtrim(CR_DATALIB) DtLib,CR_STATUS Status,isnull(UPPER(convert(VarChar(max), convert(VarBinary(max), CR_XJUSREJ))),'') Obs from SCR010 scr ";
            query += "inner join SC7010 sc7 on C7_NUM = CR_NUM and sc7.D_E_L_E_T_ = '' ";
            query += "inner join SAK010 sak on AK_COD = CR_APROV AND sak.D_E_L_E_T_ = '' ";
            query += "INNER JOIN SX5010 sx5 on X5_TABELA = 'ZY' AND X5_FILIAL = '' AND X5_CHAVE = CR_TPAPROV AND sx5.D_E_L_E_T_ = '' ";
            query += "WHERE scr.D_E_L_E_T_ = '' ";
            query += "GROUP BY CR_USER,AK_NOME,CR_NIVEL,CR_NUM,X5_DESCRI,CR_DATALIB,CR_STATUS,UPPER(convert(VarChar(max), convert(VarBinary(max), CR_XJUSREJ))) ";
            query += "union all ";
            query += "select 'Florestal' Empresa,rtrim(CR_NUM) Pedido,rtrim(CR_USER) Codigo,rtrim(AK_NOME) Nome,rtrim(CR_NIVEL) Nivel,rtrim(X5_DESCRI) Tipo,rtrim(CR_DATALIB) DtLib,CR_STATUS Status,isnull(UPPER(convert(VarChar(max), convert(VarBinary(max), CR_XJUSREJ))),'') Obs from SCR020 scr ";
            query += "inner join SC7020 sc7 on C7_NUM = CR_NUM and sc7.D_E_L_E_T_ = '' ";
            query += "inner join SAK020 sak on AK_COD = CR_APROV AND sak.D_E_L_E_T_ = '' ";
            query += "INNER JOIN SX5020 sx5 on X5_TABELA = 'ZY' AND X5_FILIAL = '' AND X5_CHAVE = CR_TPAPROV AND sx5.D_E_L_E_T_ = '' ";
            query += "WHERE scr.D_E_L_E_T_ = '' ";
            query += "GROUP BY CR_USER,AK_NOME,CR_NIVEL,CR_NUM,X5_DESCRI,CR_DATALIB,CR_STATUS,UPPER(convert(VarChar(max), convert(VarBinary(max), CR_XJUSREJ)))) as us ";
            query += "where us.Pedido = @Pedido and us.Empresa = @Empresa ";
            query += "order by us.Empresa,us.Pedido,us.Nivel,us.DtLib ";
            //#### Filtros
            parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@Empresa", empresa));
            parametros.Add(new SqlParameter("@Pedido", pedido));
            pedidoCustom.LstAprovacoes = dbProtheus.Database.SqlQuery<AprovacoesCustom>(query, parametros.ToArray()).ToList();
            #endregion QueryAprovadores
            #endregion MontaDados

            return pedidoCustom;
        }

        /// <summary>
        /// Metódo para retornar objeto nota fiscal preenchido com dados obtidos do banco
        /// </summary>
        /// <param name="empresa">Empresa Jaepel ou Florestal?</param>
        /// <param name="Recno">Id unico da nota</param>
        /// <returns>Objeto do tipo NfeCustom</returns>
        public NfeCustom GetDadosAprovacaoNf(string empresa, int Recno)
        {
            NfeCustom nfe = new NfeCustom();
            List<SqlParameter> parametros = new List<SqlParameter>();
            string query = "";

            #region Querys

            //Query Busca cabeçalho da nota fiscal
            #region CabecNf
            query = "select * from ( ";
            query += "select 'Jaepel' Empresa,'Compras' Origem,sf1.R_E_C_N_O_ Recno,sf1.F1_MOEDA Moeda,rtrim(sf1.F1_MSBLQL) Status,rtrim(sf1.F1_EMISSAO) Emissao,rtrim(sf1.F1_DOC) Numero, ";
            query += "rtrim(sf1.F1_SERIE) Serie,rtrim(sf1.F1_FORNECE) Fornecedor,rtrim(sf1.F1_LOJA) Loja,rtrim(A2_NOME) NomeFor,sf1.F1_VALBRUT VlrBruto,sf1.F1_VALMERC VlrMerc ";
            query += ",sf1.F1_DESCONT Desconto,sf1.F1_FRETE Frete,rtrim(se4.E4_DESCRI) CondPag,rtrim(sf1.F1_TPFRETE) TipoFrete ";
            query += "from SF1010 sf1 ";
            query += "inner join SA2010 sa2 on A2_COD = F1_FORNECE AND A2_LOJA = F1_LOJA and sa2.D_E_L_E_T_ = '' ";
            query += "inner join SE4010 se4 on E4_CODIGO = sf1.F1_COND and se4.D_E_L_E_T_ = '' ";
            query += "union all  ";
            //Busca dados financeiro
            query += "select 'Jaepel'Empresa,'Financeiro' Origem,se2.R_E_C_N_O_ Recno,E2_MOEDA Moeda,E2_MSBLQL Status,RTRIM(E2_EMISSAO)Emissao ";
            query += ",RTRIM(E2_NUM) Numero,RTRIM(E2_PREFIXO)Serie,RTRIM(E2_FORNECE)Fornecedor,RTRIM(E2_LOJA)Loja,RTRIM(E2_NOMFOR)NomeFor,E2_VALOR VlrBruto,E2_VALOR VlrMerc, ";
            query += "E2_DESCONT Desconto,0 Frete,RTRIM(E2_TIPO)CondPag,'' TipoFrete ";
            query += "from SE2010 se2 ";
            query += "where se2.D_E_L_E_T_ = '' ";
            query += "union all  ";
            //Busca dados compras
            query += "select 'Florestal' Empresa,'Compras' Origem,sf1.R_E_C_N_O_ Recno,sf1.F1_MOEDA Moeda,rtrim(sf1.F1_MSBLQL) Status,rtrim(sf1.F1_EMISSAO) Emissao,rtrim(sf1.F1_DOC) Numero, ";
            query += "rtrim(sf1.F1_SERIE) Serie,rtrim(sf1.F1_FORNECE) Fornecedor,rtrim(sf1.F1_LOJA) Loja,rtrim(A2_NOME) NomeFor,sf1.F1_VALBRUT VlrBruto,sf1.F1_VALMERC VlrMerc ";
            query += ",sf1.F1_DESCONT Desconto,sf1.F1_FRETE Frete,rtrim(se4.E4_DESCRI) CondPag,rtrim(sf1.F1_TPFRETE) TipoFrete ";
            query += "from SF1020 sf1 ";
            query += "inner join SA2020 sa2 on A2_COD = F1_FORNECE AND A2_LOJA = F1_LOJA and sa2.D_E_L_E_T_ = '' ";
            query += "inner join SE4020 se4 on E4_CODIGO = sf1.F1_COND and se4.D_E_L_E_T_ = '' ";
            query += "union all  ";
            //Busca dados financeiro
            query += "select 'Jaepel'Empresa,'Financeiro' Origem,se2.R_E_C_N_O_ Recno,E2_MOEDA Moeda,E2_MSBLQL Status,RTRIM(E2_EMISSAO)Emissao ";
            query += ",RTRIM(E2_NUM) Numero,RTRIM(E2_PREFIXO)Serie,RTRIM(E2_FORNECE)Fornecedor,RTRIM(E2_LOJA)Loja,RTRIM(E2_NOMFOR)NomeFor,E2_VALOR VlrBruto,E2_VALOR VlrMerc, ";
            query += "E2_DESCONT Desconto,0 Frete,RTRIM(E2_TIPO)CondPag,'' TipoFrete ";
            query += "from SE2020 se2 ";
            query += "where se2.D_E_L_E_T_ = '' ";
            query += ") as nf  ";
            query += "where nf.Recno = @Recno and nf.Empresa = @Empresa ";
            //Adiciona filtros
            parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@Recno", Recno));
            parametros.Add(new SqlParameter("@Empresa", empresa));
            //Executa query no banco de dados
            nfe = dbProtheus.Database.SqlQuery<NfeCustom>(query, parametros.ToArray()).FirstOrDefault();
            if (nfe == null)
                nfe = new NfeCustom();
            #endregion CabecNf

            //Busca itens da Nf
            #region ItensNf
            query = "select * from ( ";
            query += "select 'Jaepel' Empresa,rtrim(D1_COD)CodProduto,rtrim(D1_UM)Un,rtrim(B1_DESC)Produto,rtrim(D1_ITEM)Item,D1_QUANT Qtd,RTRIM(B1_UCOM)DtUtmComp,B1_UPRC UltPrcCompra,D1_VUNIT VlrUnit, ";
            query += "D1_TOTAL VrlTotal,D1_VALICM VlrIcms,D1_VALIPI VlrIpi,RTRIM(D1_PEDIDO)Pedido,sf1.R_E_C_N_O_ RecF1,rtrim(D1_CC ) CC ";
            query += "from SF1010 sf1 ";
            query += "inner join SD1010 sd1 on F1_DOC = D1_DOC and F1_SERIE = D1_SERIE and F1_FORNECE = D1_FORNECE and F1_LOJA = D1_LOJA and sf1.D_E_L_E_T_ = '' ";
            query += "inner join SB1010 sb1 on B1_COD = D1_COD AND sb1.D_E_L_E_T_ = '' ";
            query += "union all ";
            query += "select 'Florestal' Empresa,rtrim(D1_COD)CodProduto,rtrim(D1_UM)Un,rtrim(B1_DESC)Produto,rtrim(D1_ITEM)Item,D1_QUANT Qtd,RTRIM(B1_UCOM)DtUtmComp,B1_UPRC UltPrcCompra,D1_VUNIT VlrUnit, ";
            query += "D1_TOTAL VrlTotal,D1_VALICM VlrIcms,D1_VALIPI VlrIpi,RTRIM(D1_PEDIDO)Pedido,sf1.R_E_C_N_O_ RecF1,rtrim(D1_CC ) CC ";
            query += "from SF1020 sf1 ";
            query += "inner join SD1020 sd1 on F1_DOC = D1_DOC and F1_SERIE = D1_SERIE and F1_FORNECE = D1_FORNECE and F1_LOJA = D1_LOJA and sf1.D_E_L_E_T_ = '' ";
            query += "inner join SB1020 sb1 on B1_COD = D1_COD AND sb1.D_E_L_E_T_ = '' ";
            query += ") as it ";
            query += "where it.RecF1 = @Recno and it.Empresa = @Empresa ";
            query += "order by it.Item ";
            //Adiciona filtros
            parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@Recno", Recno));
            parametros.Add(new SqlParameter("@Empresa", empresa));
            //Executa query no banco de dados
            nfe.ItensNfe = dbProtheus.Database.SqlQuery<ItensNfCustom>(query, parametros.ToArray()).ToList();
            #endregion ItensNf

            //Busca aprovadores da nota fiscal
            #region Aprovadores
            query = "select * from ( ";
            query += "select 'Jaepel' Empresa,'Compras' Origem,sf1.R_E_C_N_O_ Recno,rtrim(ZC8_NIVEL)Nivel,rtrim(AK_NOME)Nome,RTRIM(ZC8_STATUS)Status,rtrim(ZC8_DTAPRO) DtLib,ISNULL(CONVERT(VARCHAR(MAX), CONVERT(VARBINARY(MAX), ZC8_LOG)),'') OBS from SF1010 sf1 ";
            query += "inner join ZC8010 zc8 on ZC8_NFISCA = F1_DOC AND ZC8_SERIE = F1_SERIE AND ZC8_FORNEC = F1_FORNECE AND ZC8_LOJA = F1_LOJA AND zc8.D_E_L_E_T_ = '' and ZC8_FILIAL = '' ";
            query += "inner join SAK010 sak on AK_COD = ZC8_APROV AND sak.D_E_L_E_T_ = '' and AK_FILIAL = '01' ";
            query += "where sf1.D_E_L_E_T_ = '' ";
            query += "union all ";
            //Busca dados financeiro
            query += "select 'Jaepel' Empresa,'Financeiro' Origem,se2.R_E_C_N_O_ Recno,rtrim(ZC8_NIVEL)Nivel,rtrim(AK_NOME)Nome,RTRIM(ZC8_STATUS)Status,rtrim(ZC8_DTAPRO) DtLib,ISNULL(CONVERT(VARCHAR(MAX), CONVERT(VARBINARY(MAX), ZC8_LOG)),'') OBS from SE2010 se2 ";
            query += "inner join ZC8010 zc8 on ZC8_NFISCA = E2_NUM AND ZC8_SERIE = E2_PREFIXO AND ZC8_FORNEC = E2_FORNECE AND ZC8_LOJA = E2_LOJA AND ZC8_TIPO = E2_TIPO AND zc8.D_E_L_E_T_ = '' and ZC8_FILIAL = '' ";
            query += "inner join SAK010 sak on AK_COD = ZC8_APROV AND sak.D_E_L_E_T_ = '' and AK_FILIAL = '01' ";
            query += "where se2.D_E_L_E_T_ = '' ";
            query += "union all ";
            //Busca dados de compra
            query += "select 'Florestal' Empresa,'Compras' Origem,sf1.R_E_C_N_O_ Recno,rtrim(ZC8_NIVEL)Nivel,rtrim(AK_NOME)Nome,RTRIM(ZC8_STATUS)Status,rtrim(ZC8_DTAPRO) DtLib,ISNULL(CONVERT(VARCHAR(MAX), CONVERT(VARBINARY(MAX), ZC8_LOG)),'') OBS from SF1010 sf1  ";
            query += "inner join ZC8020 zc8 on ZC8_NFISCA = F1_DOC AND ZC8_SERIE = F1_SERIE AND ZC8_FORNEC = F1_FORNECE AND ZC8_LOJA = F1_LOJA AND zc8.D_E_L_E_T_ = '' and ZC8_FILIAL = '' ";
            query += "inner join SAK020 sak on AK_COD = ZC8_APROV AND sak.D_E_L_E_T_ = '' and AK_FILIAL = '01' ";
            query += "where sf1.D_E_L_E_T_ = '' ";
            query += "union all ";
            //Busca dados financeiro
            query += "select 'Jaepel' Empresa,'Financeiro' Origem,se2.R_E_C_N_O_ Recno,rtrim(ZC8_NIVEL)Nivel,rtrim(AK_NOME)Nome,RTRIM(ZC8_STATUS)Status,rtrim(ZC8_DTAPRO) DtLib,ISNULL(CONVERT(VARCHAR(MAX), CONVERT(VARBINARY(MAX), ZC8_LOG)),'') OBS from SE2010 se2 ";
            query += "inner join ZC8020 zc8 on ZC8_NFISCA = E2_NUM AND ZC8_SERIE = E2_PREFIXO AND ZC8_FORNEC = E2_FORNECE AND ZC8_LOJA = E2_LOJA AND ZC8_TIPO = E2_TIPO AND zc8.D_E_L_E_T_ = '' and ZC8_FILIAL = '' ";
            query += "inner join SAK020 sak on AK_COD = ZC8_APROV AND sak.D_E_L_E_T_ = '' and AK_FILIAL = '01' ";
            query += "where se2.D_E_L_E_T_ = '' ";
            query += ") as ap ";
            query += "where ap.Empresa = @Empresa and ap.Recno = @Recno and Origem = @Origem ";
            query += "order by ap.Nivel ";
            //Adiciona filtros
            parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("@Recno", Recno));
            parametros.Add(new SqlParameter("@Empresa", empresa));
            parametros.Add(new SqlParameter("@Origem", nfe.Origem));
            //Executa query no banco de dados
            nfe.LstAprovacoes = dbProtheus.Database.SqlQuery<AprovacoesCustom>(query, parametros.ToArray()).ToList();
            #endregion Aprovadores

            #endregion Querys

            return nfe;
        }

        /// <summary>
        /// Metódo para avaliar se o pedido de compra foi totalmente aprovado
        /// </summary>
        /// <param name="pedido">Nº do pedido de compra.</param>
        /// <param name="empresa">Empresa a ser consultada.</param>
        public bool AprovacaoTotalPedido(string pedido, string empresa)
        {
            bool aprovado = false;
            string query = "";
            List<SqlParameter> parametros = new List<SqlParameter>();
            query = "select count(CR_NUM)TOTAL from ( ";
            query += "select 'Jaepel' Empresa,CR_NUM from SCR010 ";
            query += "WHERE CR_DATALIB = '' AND D_E_L_E_T_ = '' ";
            query += "union all ";
            query += "select 'Florestal' Empresa,CR_NUM from SCR020 ";
            query += "WHERE CR_DATALIB = '' AND D_E_L_E_T_ = '') as ped ";
            query += "WHERE ped.CR_NUM = @Pedido and ped.Empresa = @Empresa ";
            parametros.Add(new SqlParameter("@Empresa", empresa));
            parametros.Add(new SqlParameter("@Pedido", pedido));
            var result = dbProtheus.Database.SqlQuery<int>(query, parametros.ToArray()).First();
            if (result == 0)
            {
                aprovado = true;
                var tabela = empresa == "Jaepel" ? "SC7010" : "SC7020";
                query = "UPDATE " + tabela + " set C7_CONAPRO  = @Status ";
                query += "WHERE D_E_L_E_T_ = '' AND C7_NUM = @Pedido ";
                parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@Status", "L"));//Seta status como Liberado pelo usuário
                parametros.Add(new SqlParameter("@Pedido", pedido));
                //Executa comando no banco de dados
                dbProtheus.Database.ExecuteSqlCommand(query, parametros.ToArray());
            }
            return aprovado;
        }

        /// <summary>
        /// Metódo para avaliar se a nota fiscal foi totalmente aprovada
        /// </summary>
        /// <param name="Recno">Recno da nota fiscal.</param>
        /// <param name="empresa">Empresa a ser consultada.</param>
        public bool AprovacaoTotalNf(int Recno, string Origem, string empresa)
        {
            bool aprovado = false;
            string query = "";
            List<SqlParameter> parametros = new List<SqlParameter>();
            query = "select COUNT(*)TOTAL from ( ";
            query += "SELECT 'Jaepel' Empresa,'Compras' Origem,sf1.R_E_C_N_O_ Recno FROM SF1010 sf1 ";
            query += "inner join ZC8010 zc8 on ZC8_NFISCA = F1_DOC AND ZC8_SERIE = F1_SERIE AND ZC8_FORNEC = F1_FORNECE AND ZC8_LOJA = F1_LOJA AND zc8.D_E_L_E_T_ = '' and ZC8_FILIAL = ''  ";
            query += "where sf1.D_E_L_E_T_ = '' and ZC8_STATUS = 'N' ";
            query += "union all ";
            //Busca dados financeiro jaepel
            query += "SELECT 'Jaepel' Empresa,'Financeiro' Origem,se2.R_E_C_N_O_ Recno FROM SE2010 se2 ";
            query += "inner join ZC8010 zc8 on ZC8_TIPO = E2_TIPO AND ZC8_NFISCA = E2_NUM AND ZC8_SERIE = E2_PREFIXO AND ZC8_FORNEC = E2_FORNECE AND ZC8_LOJA = E2_LOJA AND zc8.D_E_L_E_T_ = '' and ZC8_FILIAL = '' ";
            query += "where se2.D_E_L_E_T_ = '' and ZC8_STATUS = 'N' ";
            query += "union all ";
            //Busca dados compras florestal
            query += "SELECT 'Florestal' Empresa,'Compras' Origem,sf1.R_E_C_N_O_ Recno FROM SF1020 sf1 ";
            query += "inner join ZC8020 zc8 on ZC8_NFISCA = F1_DOC AND ZC8_SERIE = F1_SERIE AND ZC8_FORNEC = F1_FORNECE AND ZC8_LOJA = F1_LOJA AND zc8.D_E_L_E_T_ = '' and ZC8_FILIAL = ''  ";
            query += "where sf1.D_E_L_E_T_ = '' and ZC8_STATUS = 'N' ";
            query += "union all ";
            //Busca dados financeiro florestal
            query += "SELECT 'Florestal' Empresa,'Financeiro' Origem,se2.R_E_C_N_O_ Recno FROM SE2020 se2 ";
            query += "inner join ZC8020 zc8 on ZC8_TIPO = E2_TIPO AND ZC8_NFISCA = E2_NUM AND ZC8_SERIE = E2_PREFIXO AND ZC8_FORNEC = E2_FORNECE AND ZC8_LOJA = E2_LOJA AND zc8.D_E_L_E_T_ = '' and ZC8_FILIAL = '' ";
            query += "where se2.D_E_L_E_T_ = '' and ZC8_STATUS = 'N' ";
            query += ") as nf ";
            query += "WHERE nf.Recno = @Recno and nf.Empresa = @Empresa ";
            parametros.Add(new SqlParameter("@Empresa", empresa));
            parametros.Add(new SqlParameter("@Recno", Recno));
            var result = dbProtheus.Database.SqlQuery<int>(query, parametros.ToArray()).First();
            if (result == 0)
            {
                aprovado = true;
                var tabela = empresa == "Jaepel" ? "SF1010" : "SF1020";
                if (Origem == "Compras")
                {
                    query = "UPDATE " + tabela + " set F1_MSBLQL = @Status ";
                    query += "WHERE D_E_L_E_T_ = '' AND R_E_C_N_O_ = @Recno ";
                }
                else if (Origem == "Financeiro")
                {
                    tabela = empresa == "Jaepel" ? "SE2010" : "SE2020";
                    query = "UPDATE " + tabela + " set E2_MSBLQL = @Status ";
                    query += "WHERE D_E_L_E_T_ = '' AND R_E_C_N_O_ = @Recno ";
                }
                parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@Status", "2"));//Seta status como Liberado
                parametros.Add(new SqlParameter("@Recno", Recno));
                //Executa comando no banco de dados
                dbProtheus.Database.ExecuteSqlCommand(query, parametros.ToArray());
            }
            return aprovado;
        }
        #endregion MetodosUteis
    }
}