using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.SqlClient;
using SGI.Util;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using SGI.Models;
using SGI.Context;
using SGI.Enums;
using System.Data;

namespace SGI.Controllers
{
    [CustomAuthorize(Roles = "Gerente, AdiminstradorTI, AdiminstradorPCP")]
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private JSgi db = new JSgi();

        public ActionResult Index(int? nPageSize, int? page, int? idGrupo, int? idUnidade, int? idNegocio, int? idDepartamento, string pAno, string pGrafico, string search)
        {
            db.Database.CommandTimeout = 999;
            int userId = Convert.ToInt32(HttpContext.User.Identity.Name);
            var usuario = db.T_Usuario.Find(userId);
            //Defini layout
            #region ViewDataLayout
            ViewData["mainmenu_scroll"] = "fixedscroll"; //pagescroll , fixedscroll
            ViewData["body_class"] = "sidebar-collapse";
            ViewData["html_class"] = "";
            ViewData["pagetopbar_class"] = "sidebar_shift";
            ViewData["maincontent_class"] = "sidebar_shift";
            ViewData["maincontent_type"] = "";
            ViewData["pagesidebar_class"] = "collapseit";
            ViewData["pagechatapi_class"] = "";
            ViewData["chatapiwindows_demo"] = "";
            ViewData["chatapiwindows_class"] = "";
            #endregion ViewDataLayout
            #region Variaveis
            var grafico = new Graficos();
            #endregion Variaveis
            #region ViewBags
            ViewBag.idNegocio = new SelectList(db.T_Negocio.OrderBy(x => x.NEG_DESCRICAO), "NEG_ID", "NEG_DESCRICAO", idNegocio);
            ViewBag.idGrupo = new SelectList(db.T_Grupo.Where(x => x.EXIBELISTA == (int)YesNo.Sim).OrderBy(x => x.NOME), "GRU_ID", "NOME", idGrupo);
            ViewBag.idUnidade = new SelectList(db.T_Unidade.OrderBy(x => x.DESCRICAO), "UNI_ID", "UN", idUnidade);
            ViewBag.idDepartamento = new SelectList(db.T_Departamentos.OrderBy(x => x.DEP_NOME), "DEP_ID", "DEP_NOME", idDepartamento);
            var anos = db.T_Medicoes.Select(x => new { Ano = x.MED_DATAMEDICAO.Substring(0, 4) }).GroupBy(x => new { x.Ano }).OrderByDescending(x => x.Key.Ano).ToList();
            pAno = (pAno == "" || pAno == null && anos.Count > 0 ? anos.FirstOrDefault().Key.Ano : pAno);
            ViewBag.anoAtual = pAno;
            ViewBag.search = search;
            ViewBag.negAtual = idNegocio;
            ViewBag.grpAtual = idGrupo;
            ViewBag.unAtual = idUnidade;
            ViewBag.grafico = pGrafico ?? "G";
            ViewBag.departamentoAtu = idDepartamento;
            ViewBag.pAno = new SelectList(anos, "Key.Ano", "Key.Ano", pAno);
            #endregion ViewBags
            //Busca favoritos
            grafico.Favoritos = db.T_Favoritos.Where(x => x.ID_USUARIO == usuario.ID_USUARIO).ToList();
            //Chama metódo para trazer dados agrupados por indicadores e metas
            var indicadores = Util.ExtratorMedidor.GetIndicadores(nPageSize, page, idNegocio, idGrupo, idUnidade, idDepartamento, pAno, search, grafico.Favoritos);

            if (!string.IsNullOrEmpty(pAno))
            {
                //Chama metódo para poder buscar dados de medições
                grafico.Medicoes = Util.ExtratorMedidor.GetMedicoes(indicadores.ToList(), pAno);
                grafico.AnoAnterior = Util.QueryAnaliser.AnoAnterior(pAno);
                ViewBag.anoAnterior = grafico.AnoAnterior.Count > 0 ? grafico.AnoAnterior.FirstOrDefault().Mes.Substring(0, 4) : "";
            }
            else
            {
                grafico.Medicoes = new List<vw_SGI_PARAMETRO_RELMEDICOES>();
                grafico.AnoAnterior = new List<vw_SGI_PARAMETRO_RELMEDICOES>();
                ViewBag.anoAnterior = null;
            }

            //Busca lista de indicadores
            grafico.Indicadores = indicadores;
            if (grafico.Medicoes.Count > 0)
            {
                //Busca informações complementares
                var infComplementares = db.T_Informacoes_Complementares.ToList();
                infComplementares = infComplementares.Where(x => grafico.Medicoes.Any(j => j.MET_ID == x.MET_ID)).ToList();
                grafico.Complementares = infComplementares;
            }
            //Busca planos de ações
            var planos = db.T_PlanoAcao.ToList();
            planos = planos.Where(x => indicadores.Any(i => i.IND_ID == x.T_Metas.IND_ID)).ToList();
            grafico.PlanoAcoes = planos;
            var listaMetas = "";
            if (pGrafico=="M")
            {
                ViewBag.listaMetas = db.Database.SqlQuery<V_PAINEL_LISTA_METAS>("select * from V_PAINEL_LISTA_METAS ").ToList();

            }
            
            return View(grafico);
        }

        public ActionResult Detalhado(int idIndicador, int? nPageSize, int? page, string searchString)
        {
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            ViewBag.ItensPageSize = new SelectList(new List<int> { 5, 10, 25, 50, 100 }, selectedValue: 10);
            ///Busca registros no banco de dados
            string query = "SELECT object_id Id,Replace(Replace(UPPER(NAME),'VW_SGI_" + idIndicador.ToString() + "_',''),'_',' ') Nome FROM ";
            query += "(SELECT object_id,name FROM SYS.views ";
            query += "union ";
            query += "SELECT object_id,name FROM SYS.procedures) as p ";
            query += "WHERE UPPER(NAME) LIKE '%VW_SGI_" + idIndicador.ToString() + "_%' ";
            var views = db.Database.SqlQuery<ViewsNome>(query).ToList();
            //Filtro
            if (searchString != null && searchString != "")
                views = views.Where(x => x.Nome.ToUpper().Contains(searchString.ToUpper())).ToList();

            return View(views.OrderBy(x => x.Nome).ToPagedList(_PageNumber, _PageSize));
        }

        public ActionResult DetalhadoView(int id, int idIndicador, string data1, string data2)
        {
            var views = new ViewsNome();
            var dataAtual = DateTime.Now;
            ViewBag.data1 = String.IsNullOrEmpty(data1) ? new DateTime(dataAtual.Year, dataAtual.Month, 1).ToString() : data1;
            ViewBag.data2 = String.IsNullOrEmpty(data2) ? new DateTime(dataAtual.Year, dataAtual.Month, 1).AddMonths(1).AddDays(-1).ToString() : data2;
            string query = "SELECT Tipo,object_id Id,upper(NAME) Nome FROM ";
            query += "(SELECT 'view' Tipo,object_id,name FROM SYS.views ";
            query += "union ";
            query += "SELECT 'procedure' Tipo,object_id,name FROM SYS.procedures) as p ";
            query += "WHERE object_id = '" + id + "' ";
            views = db.Database.SqlQuery<ViewsNome>(query).First();

            views.Indicador = db.T_Indicadores.Find(idIndicador);
            if (views.Tipo == "view")
                views.Campos = db.Database.SqlQuery<ViewsCampos>("SELECT name Nome FROM sys.columns where object_id = " + id.ToString()).ToList();
            else
                views.Campos = Util.QueryAnaliser.GetCamposProcedure(views.Nome);
            views.Valores = new string[0, 0];
            db.Database.CommandTimeout = 999;
            if ((data1 != "" && data1 != null) && (data2 != "" && data2 != null))
                views.Valores = QueryAnaliser.GetValores(views.Tipo, views.Nome, data1, data2);
            views.Nome = views.Nome.Replace("VW_SGI_" + idIndicador.ToString() + "_", "").Replace("_", " ");
            //select list itens
            return View(views);
        }

        #region MetodosJson
        /// <summary>
        /// Metódo para poder preencher o gráfico na tela
        /// </summary>
        /// <param name="idIndicador">Código do indicador</param>
        /// <param name="ano">Ano a ser filtrado</param>
        /// <returns>Objeto json preenchido</returns>
        ///    **grafico 
        public JsonResult GetGrafico(int idInd, int tipoGrafico, string ano, int dimensao, string periodo, string strDataIni, string strDataFim, string subDime)
        {
            // testar o tipo do grafico 
            // testar se a medicao possui sub dimencoes caso sim  o grafico deve ser mostrado em pizza onde todos os fotos+ subdimencoes sao agrupadas na pizza continua explicacao em baixo 
            // em caso de grafico de linha   o eixo x sera preenchido com os fatos e as linhas serao as subdimencoes
            // neste caso o filtro sempre sera por data. 

            DateTime dataDe = DateTime.Now; 
            DateTime dataAte = DateTime.Now;
            DateTime dataAux = DateTime.Now;

            periodo = periodo.Trim();
            if ((string.IsNullOrEmpty(periodo) && !string.IsNullOrEmpty(strDataIni) && !string.IsNullOrEmpty(strDataFim)) || (periodo == "D"))
            {
                dataDe = DateTime.ParseExact(strDataIni, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                dataAte = DateTime.ParseExact(strDataFim, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                dataAux = dataAte;
            }

            string query = "";
            if (periodo == "M")
            {
                query = @"select * from vw_SGI_PARAMETRO_RELMEDICOES 
                        WHERE IND_ID = '" + idInd.ToString() + "' AND DimId = " + dimensao +
                        " AND LEFT(PerId, 1) ='" + periodo + "'";
                //Filtra ano atual
                if (ano != "" && ano != null)
                {
                    query += " AND ((LEFT(MES,4) = '" + (Convert.ToInt32(ano) - 1) + "' and PerId in ('MAC', 'DAC' )) or LEFT(MES,4) = '" + ano + "')";
                }
                if (subDime != "null" && subDime.Trim() !="")
                {
                    query += " AND DimSubId = '" + subDime + "' ";
                }
                query += "order by IND_ID,Mes";
            }
            else if (periodo == "D")
            {
                query = string.Format(@"select * 
                        from vw_SGI_PARAMETRO_RELMEDICOES 
                        WHERE IND_ID = {0} AND MED_DATA_MEDICAO between {1} and {2} 
                        AND LEFT(PerId, 1) = 'D' AND DimId = {3} "
                        , idInd.ToString()
                        , dataDe.ToString("yyyyMMdd")
                        , dataAte.ToString("yyyyMMdd")
                        //, dataAtual.AddDays(-30).ToString("yyyyMMdd")
                        //, dataAtual.ToString("yyyyMMdd")
                        , dimensao);
                if (subDime != "null" && subDime.Trim() != "")
                {
                    query += " AND DimSubId = '" + subDime + "' ";
                }
                query += " order by MED_DATA_MEDICAO ";
            }
            else if (string.IsNullOrEmpty(periodo) && !string.IsNullOrEmpty(strDataIni) && !string.IsNullOrEmpty(strDataFim))
            {
                query = string.Format(@"select * 
                        from vw_SGI_PARAMETRO_RELMEDICOES 
                        WHERE IND_ID = {0} AND MED_DATA_MEDICAO between {1} and {2} 
                        AND LEFT(PerId, 1) = 'D' AND DimId = {3} "
                       , idInd.ToString()
                       , dataDe.ToString("yyyyMMdd")
                       , dataAte.ToString("yyyyMMdd")
                       , dimensao);
                if (subDime!= "null"&& subDime.Trim() !="")
                {
                    query += " AND DimSubId = '" + subDime + "' ";
                }
                query += " order by MED_DATA_MEDICAO ";

            }


            var grafico = new Graficos();
            List<vw_SGI_PARAMETRO_RELMEDICOES> medicao = db.Database.SqlQuery<vw_SGI_PARAMETRO_RELMEDICOES>(query).ToList();
            List<object> fatos = new List<object>();
            List<object> dados = new List<object>();
            List<string> cabecalho = new List<string>();
            List<string> legenda = new List<string>();
            string temDimSubId = "";

            //var fatosDesc = medicao.Select(x => new { x.FatDescricao, x.DimSubId }).Distinct().ToList();
            var fatosId = medicao.Select(x => new { x.FatId, x.FatDescricao }).Distinct().ToList();
            var subDimensao = medicao.Select(x => new { x.DimSubId, x.DimSubDescricao }).Distinct().ToList();//medicao.Select(x => x.FatId).Distinct().ToList();


            //var fatosDesc = medicao.Select(x => new { x.FatDescricao, x.DimSubId }).Distinct().ToList();
            if (medicao.Count > 0)
            {
                fatosId = medicao.Select(x => new { x.FatId, x.FatDescricao }).Distinct().ToList();
                subDimensao = medicao.Select(x => new { x.DimSubId,x.DimSubDescricao }).Distinct().ToList();//medicao.Select(x => x.FatId).Distinct().ToList();

                var indicador = db.T_Indicadores.Find(idInd);
                indicador.DIM_ID = dimensao;
                indicador.PER_ID = periodo;
                indicador.IND_GRAFICO = tipoGrafico;
                string tipoCompMeta = indicador.IND_TIPOCOMPARADOR.ToString();//db.T_Indicadores.Find(idInd).IND_TIPOCOMPARADOR.ToString();
                db.SaveChanges();//Salva mudanças
                if (!string.IsNullOrEmpty(periodo))
                {
                    if (!string.IsNullOrEmpty(subDimensao[0].DimSubId)) // teste se tem sub dimentcao 
                    {
                        temDimSubId = "T";
                        foreach (var s in subDimensao)
                        {
                            List<object> medicoesGrafico = new List<object>();
                            vw_SGI_PARAMETRO_RELMEDICOES med = null;
                            foreach (var f in fatosId)
                            {
                                var medicaoFato = medicao.Where(x => x.FatId.Trim() == f.FatId.Trim() &&
                                x.DimSubId.Trim() == s.DimSubId.Trim() && x.PerId.Trim() == periodo.Trim()).GroupBy(g => new
                                { g.DimSubId, g.FatId, g.FatDescricao }).Select(g => new
                                {
                                    subDesc = g.Key.DimSubId,
                                    name = g.Key.FatDescricao,
                                    value = g.Sum(m => m.Valor)
                                }).FirstOrDefault();
                                if (medicaoFato != null)
                                {
                                    medicoesGrafico.Add(new
                                    { ano = "",
                                        mes = "",
                                        un = "",
                                        meta = "",
                                        valor = medicaoFato.value,
                                        tipo = 0,
                                        dimId = 0,
                                        dimDescricao = "",
                                        fatId = f.FatId,
                                        fatDescricao = f.FatDescricao,
                                        perId = "D",
                                        perDescricao ="Dia",
                                        dimSubId = s.DimSubId,
                                        dimSubDescricao = s.DimSubDescricao
                                    });
                                }
                                else
                                {
                                    medicoesGrafico.Add(new
                                    {
                                        ano = "",
                                        mes = "",
                                        un = "",
                                        meta = "",
                                        valor = "",
                                        ac = 0,
                                        tipo = 0,
                                        data = "",
                                        dimId = 0,
                                        dimDescricao = "",
                                        fatId = "",
                                        fatDescricao = "",
                                        perId = "",
                                        perDescricao = "",
                                        medId = "",
                                        dimSubId = ""
                                    });
                                }
                            }
                            fatos.Add(medicoesGrafico);
                        }
                    }
                    else
                    {
                        foreach (var f in fatosId)
                        {
                            var medicaoFato = medicao.Where(x => x.FatId == f.FatId).ToList();
                            List<object> medicoesGrafico = new List<object>();
                            vw_SGI_PARAMETRO_RELMEDICOES med = null;
                            if (periodo == "M")
                            {
                                for (int i = 0; i < 14; i++)
                                {
                                    string mes;
                                    if (i + 1 < 10)
                                        mes = "0" + (i + 1);
                                    else
                                        mes = (i + 1).ToString();
                                    if (i == 12)
                                    {
                                        med = medicaoFato.Where(x => x.PerId.Trim() == "MAC" && x.Mes.Substring(0, 4) == ano).FirstOrDefault();
                                    }
                                    else if (i == 13)
                                    {
                                        med = medicaoFato.Where(x => x.PerId.Trim() == "MAC" && x.Mes.Substring(0, 4) == (Convert.ToInt32(ano) - 1).ToString()).FirstOrDefault();
                                    }
                                    else
                                    {
                                        med = medicaoFato.Where(x => x.Mes.Substring(4, 2) == mes && x.PerId.Trim() == "M").FirstOrDefault();

                                    }
                                    if (med != null)
                                    {
                                        medicoesGrafico.Add(new
                                        {
                                            ano = med.Mes.Substring(0, 4),
                                            mes = med.Mes.Substring(4, 2),
                                            un = med.UNID,
                                            meta = med.META,
                                            valor = med.Valor,
                                            tipo = tipoCompMeta,
                                            dimId = med.DimId,
                                            dimDescricao = med.DimDescricao,
                                            fatId = med.FatId,
                                            fatDescricao = med.FatDescricao,
                                            perId = med.PerId,
                                            perDescricao = med.DimDescricao,
                                            dimSubId = med.DimSubId,
                                            dimSubDescricao = med.DimSubDescricao
                                        });
                                    }
                                    else
                                    {

                                        medicoesGrafico.Add(new
                                        {
                                            ano = "",
                                            mes = "",
                                            un = "",
                                            meta = "",
                                            valor = "",
                                            ac = 0,
                                            tipo = 0,
                                            data = "",
                                            dimId = 0,
                                            dimDescricao = "",
                                            fatId = "",
                                            fatDescricao = "",
                                            perId = "",
                                            perDescricao = "",
                                            medId = "",
                                            dimSubId = ""
                                        });
                                    }
                                }
                            }
                            else if (periodo == "D")
                            {
                                dataAux = dataDe;
                                while (dataAte >= dataAux)
                                {
                                    cabecalho.Add(dataAux.Day.ToString());

                                    med = medicaoFato.Where(x => x.MED_DATA.Date == dataAux.Date && x.PerId.Trim().ToUpper() == "D").FirstOrDefault();
                                    if (med != null)
                                    {
                                        medicoesGrafico.Add(new
                                        {
                                            ano = med.Mes.Substring(4, 2),
                                            mes = med.Mes.Substring(0, 4),
                                            un = med.UNID,
                                            meta = med.META,
                                            valor = med.Valor,
                                            tipo = tipoCompMeta,
                                            data = med.MED_DATA.ToShortTimeString(),
                                            dimId = med.DimId,
                                            dimDescricao = med.DimDescricao,
                                            fatId = med.FatId,
                                            fatDescricao = med.FatDescricao,
                                            perId = med.PerId,
                                            perDescricao = med.DimDescricao,
                                            dimSubId = med.DimSubId,
                                            dimSubDescricao = med.DimSubDescricao
                                        });
                                    }
                                    else
                                    {
                                        medicoesGrafico.Add(new
                                        {
                                            ano = "",
                                            mes = "",
                                            un = "",
                                            meta = "",
                                            valor = "",
                                            ac = 0,
                                            tipo = 0,
                                            data = "",
                                            dimId = 0,
                                            dimDescricao = "",
                                            fatId = "",
                                            fatDescricao = "",
                                            perId = "",
                                            perDescricao = "",
                                            medId = "",
                                            dimSubId = ""
                                        });
                                    }
                                    dataAux = dataAux.AddDays(1);
                                }
                            }
                            fatos.Add(medicoesGrafico);
                        }
                    }
                }
                else
                {
                    var g1 = medicao.GroupBy(g => new { g.DimSubId, g.FatId, g.FatDescricao }).Select(g => new
                    {
                        subDesc = g.Key.DimSubId,
                        name = g.Key.FatDescricao,
                        value = g.Sum(m => m.Valor)
                    }).ToList();

                    var g2 = medicao.GroupBy(g => new { g.DimSubId }).Select(g => new
                    {
                        subDesc = g.Key.DimSubId,
                        value = g.Sum(m => m.Valor)
                    }).ToList();

                    foreach (var dt in g2)
                    {
                        foreach (var d in g1)
                        {
                            if (d.subDesc == dt.subDesc)
                            {
                                float percentual = (float) ((d.value / dt.value) * 100);
                                //dados.Add(new { subDesc = d.subDesc, name = d.name, value = d.value, percent = (d.value / dt.value) * 100 });
                                dados.Add(new { d.subDesc, d.name, d.value, percent = Math.Round(percentual, 2 ) });
                            }

                        }
                    }
                    if (!string.IsNullOrEmpty(subDimensao[0].DimSubId)) // teste se tem sub dimentcao 
                    {
                        temDimSubId = "T";
                    }
                }

                //mota o cabeçalio que é a escala do grafico
                if (temDimSubId == "T")
                {
                    foreach (var f in fatosId)
                    {
                        cabecalho.Add(f.FatDescricao.ToString().Trim());
                    }
                    // legenda 
                    if (tipoGrafico == 3)// pizzaa
                    {
                        var temp = medicao.Select(x => new { x.FatDescricao, x.DimSubId }).Distinct().ToList();
                        foreach (var t in temp)
                        {
                            legenda.Add(t.FatDescricao.ToString().Trim() + " - " + t.DimSubId.ToString().Trim());
                        }
                    }
                    else
                    {
                        var temp = medicao.Select(x => new { x.DimSubId }).Distinct().ToList();
                        foreach (var t in temp)
                        {
                            legenda.Add(t.DimSubId.ToString().Trim());
                        }
                    }
                }
                else
                {
                    var temp = medicao.Select(x => new { x.FatDescricao}).Distinct().ToList();
                    foreach (var t in temp)
                    {
                        legenda.Add(t.FatDescricao.ToString().Trim());
                    }
                    
                    if (periodo == "M")
                    {
                        cabecalho.Add("01");
                        cabecalho.Add("02");
                        cabecalho.Add("03");
                        cabecalho.Add("04");
                        cabecalho.Add("05");
                        cabecalho.Add("06");
                        cabecalho.Add("07");
                        cabecalho.Add("08");
                        cabecalho.Add("09");
                        cabecalho.Add("10");
                        cabecalho.Add("11");
                        cabecalho.Add("12");
                        cabecalho.Add("AC");
                        cabecalho.Add("AN");
                    }
                    else if (periodo == "D")
                    {
                        dataAux = dataDe;
                        while (dataAte >= dataAux)
                        {
                            cabecalho.Add(dataAux.Day.ToString());
                            dataAux = dataAux.AddDays(1);
                        }
                    }
                }
            }
            // fatos = serie                              cabecalho = eicho X xAxis
            return this.Json(new { fatos, dados, legenda, cabecalho, temDimSubId, subDimensao }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFatosPeriodos(int dimensao, int indicador)
        {
            var periodos = db.T_Medicoes.Where(x => x.IndId == indicador && x.DimId == dimensao && x.PerId.Trim().ToUpper() != "MAC" && x.PerId.Trim().ToUpper() != "DAC").Select(x => new { id = x.PerId, descricao = x.PerDescricao }).Distinct().ToList();
            return Json(new
            {
                periodos = periodos
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPeriodos(int indicador, int dimensao)
        {
            var periodos = db.T_Medicoes.Where(x => x.IndId == indicador && x.DimId == dimensao && x.PerId.Trim().ToUpper() != "MAC" && x.PerId.Trim().ToUpper() != "DAC").Select(x => new { id = x.PerId, descricao = x.PerDescricao }).Distinct().ToList();
            return Json(new
            {
                periodos = periodos
            }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetDtMedicao(int idInd, string ano)
        {
            string query = "select convert(varchar,max(T_DATA),120)DATA from vw_SGI_PARAMETRO_RELMEDICOES WHERE IND_ID = '" + idInd.ToString() + "' ";
            //Filtra ano atual
            if (ano != "" && ano != null)
                query += "AND LEFT(MES,4) = '" + ano + "' ";
            var grafico = new Graficos();
            var dtMedicao = db.Database.SqlQuery<string>(query).ToList();
            return this.Json(new { dtMedicao = dtMedicao }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AtuDados(int idInd, string ano)
        {
            var cnn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PlayConect"].ConnectionString);
            var cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = @"EXEC SP_SGI_EXTRATOR @data,@indicador";
            cmd.Parameters.Add("@data", SqlDbType.VarChar).Value = ano;
            cmd.Parameters.Add("@indicador", SqlDbType.Int).Value = idInd;
            if (cnn.State != ConnectionState.Closed) cnn.Close();
            cnn.Open();
            try
            {
                cmd.CommandTimeout = 99999;
                SqlDataReader reader = cmd.ExecuteReader();
            }
            catch (Exception erro)
            {
                throw new Exception("Erro ao executar procedure: " + erro.Message);
            }
            finally
            {
                cnn.Close();
            }
            return this.Json(new { sucess = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddFavoritos(int idIndicador)
        {
            var favorito = new T_Favoritos();
            string status = "incluir";
            if (db.T_Favoritos.Count(x => x.ID_INDICADOR == idIndicador) > 0)
            {
                favorito = db.T_Favoritos.First(x => x.ID_INDICADOR == idIndicador);
                db.T_Favoritos.Remove(favorito);
                db.SaveChanges();
                status = "remover";
            }
            else
            {
                var userId = Convert.ToInt32(HttpContext.User.Identity.Name);
                favorito.ID_USUARIO = db.T_Usuario.First(x => x.ID_USUARIO == userId).ID_USUARIO;
                favorito.ID_INDICADOR = idIndicador;
                db.T_Favoritos.Add(favorito);
                db.SaveChanges();
            }
            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }
        #endregion MetodosJson

        public ActionResult ExportarExcel(int id, int idIndicador, string data1, string data2)
        {
            var views = new ViewsNome();
            var dataAtual = DateTime.Now;
            ViewBag.data1 = String.IsNullOrEmpty(data1) ? new DateTime(dataAtual.Year, dataAtual.Month, 1).ToString() : data1;
            ViewBag.data2 = String.IsNullOrEmpty(data2) ? new DateTime(dataAtual.Year, dataAtual.Month, 1).AddMonths(1).AddDays(-1).ToString() : data2;
            string query = "SELECT Tipo,object_id Id,upper(NAME) Nome FROM ";
            query += "(SELECT 'view' Tipo,object_id,name FROM SYS.views ";
            query += "union ";
            query += "SELECT 'procedure' Tipo,object_id,name FROM SYS.procedures) as p ";
            query += "WHERE object_id = '" + id + "' ";
            views = db.Database.SqlQuery<ViewsNome>(query).First();
            views.Indicador = db.T_Indicadores.Find(idIndicador);
            if (views.Tipo == "view")
                views.Campos = db.Database.SqlQuery<ViewsCampos>("SELECT name Nome FROM sys.columns where object_id = " + id.ToString()).ToList();
            else
                views.Campos = Util.QueryAnaliser.GetCamposProcedure(views.Nome);
            views.Valores = new string[0, 0];
            if ((data1 != "" && data1 != null) && (data2 != "" && data2 != null))
                views.Valores = QueryAnaliser.GetValores(views.Tipo, views.Nome, data1, data2);
            views.Nome = views.Nome.Replace("VW_SGI_" + idIndicador.ToString() + "_", "").Replace("_", " ");

            var products = new System.Data.DataTable(views.Nome);
            //Adiciona colunas
            for (int i = 0; i < views.Campos.Count; i++)
            {
                products.Columns.Add(views.Campos[i].Nome, typeof(string));
            }
            //Preenche valores
            string[] valores;
            for (int ln = 0; ln < views.Valores.GetLength(0); ln++)
            {
                valores = new string[views.Campos.Count];
                for (int col = 0; col < views.Campos.Count; col++)
                {
                    valores[col] = views.Valores[ln, col];
                }
                products.Rows.Add(valores);
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + views.Nome + DateTime.Now.Date.ToString("ddMMyyyy") + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View();
        }
    }
}