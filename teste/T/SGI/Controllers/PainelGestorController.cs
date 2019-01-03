using SGI.Context;
using SGI.Models;
using SGI.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SGI.Controllers
{
    [CustomAuthorize(Roles = "Gerente, AdiminstradorTI, AdiminstradorPCP")]
    public class PainelGestorController : Controller
    {
        private JSgi db = new JSgi();
        // GET: PainelGestor
        public ActionResult Index() // estrutura da pagina
        {
            ViewBag.m = PainelGestorListaMaquinasE();
            ViewBag.t = PainelGestorListaTurnoE();
            ViewBag.r = PainelGestorRanking();
            ViewBag.oee = PainelGestorOEEChart();
            ViewBag.tm = PainelGestorListaTurnoMaquinas();
            return View();
        }


        public ActionResult GestorAjax() // estrutura da pagina
        {

            return Json(new {

                m = PainelGestorListaMaquinasE(),
                t = PainelGestorListaTurnoE(),
                r = PainelGestorRanking(),
                oee = PainelGestorOEEChart(),
                tm = PainelGestorListaTurnoMaquinas()

            },JsonRequestBehavior.AllowGet);

        }



        public List<V_PAINEL_GESTOR_STATUS_MAQUINAS> PainelGestorListaMaquinasE()
        {
            return db.Database.SqlQuery<V_PAINEL_GESTOR_STATUS_MAQUINAS>("select * from V_PAINEL_GESTOR_STATUS_MAQUINAS ").ToList();
        }
        public List<V_PAINEL_GESTOR_DESEMPENHO_TURNOS_MAQUINA> PainelGestorListaTurnoMaquinas()
        {
            return db.Database.SqlQuery<V_PAINEL_GESTOR_DESEMPENHO_TURNOS_MAQUINA>("select * from V_PAINEL_GESTOR_DESEMPENHO_TURNOS_MAQUINA WHERE FEE_DIA_TURMA = dbo.DIATURMA(GETDATE()) ").ToList();
        }
        public List<V_PAINEL_GESTOR_DESEMPENHO_TURNOS> PainelGestorListaTurnoE()
        {
            return db.Database.SqlQuery<V_PAINEL_GESTOR_DESEMPENHO_TURNOS>("select * from V_PAINEL_GESTOR_DESEMPENHO_TURNOS  WHERE FEE_DIA_TURMA = dbo.DIATURMA(GETDATE()) ").ToList();
        }



        public Resultquery PainelGestorOEEChart()
        {
            var queryResult = new Resultquery();
            string[,] dados = new string[0, 0];
            int cont = 0;
            var cnn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PlayConect"].ConnectionString);
            var cmd = new SqlCommand();
            cmd.CommandText = " SELECT * FROM V_PAINEL_GESTOR_OEE  WHERE PER_ID = 'M' AND LEFT(MED_DATAMEDICAO,6) = FORMAT( GETDATE(), 'yyyyMM','en-US') ";

            if (cnn.State != ConnectionState.Closed) cnn.Close();
            cnn.Open();
            try
            {
                cmd.CommandTimeout = 99999;
                cmd.Connection = cnn;
                DataSet ds = new DataSet();
                DataAdapter data = new SqlDataAdapter(cmd);
                data.Fill(ds);
                queryResult.Id = 0;
                queryResult.Titulo = "";
                queryResult.Descricao = "";
                queryResult.Tipo = "SQL";
                queryResult.Qtdlinhas = ds.Tables[0].Columns.Count;
                queryResult.Dados = new string[ds.Tables[0].Rows.Count, ds.Tables[0].Columns.Count];
                queryResult.Colunas = new List<Coluna>();
                queryResult.Linhas = new List<LineData>();

                //public List<Colunas> Colunas { get; set; }
                for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                {
                    Coluna Col = new Coluna()
                    {
                        Nome = ds.Tables[0].Columns[i].ColumnName,
                        Tipo = ds.Tables[0].Columns[i].DataType.ToString()
                    };
                    queryResult.Colunas.Add(Col);
                }

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    LineData line = new LineData();
                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                    {
                        queryResult.Dados[cont, i] = row[i].ToString();
                        line.linha.Add(row[i].ToString());
                    }
                    queryResult.Linhas.Add(line);
                    cont++;
                }
            }
            catch (Exception erro)
            {
                throw new Exception("Erro ao listar tipo embalagem: " + erro.Message);
            }
            finally
            {
                cnn.Close();
            }

            return queryResult;
        }




            public Resultquery PainelGestorListaMaquinas()
        {
            var queryResult = new Resultquery();
            string[,] dados = new string[0, 0];
            int cont = 0;
            var cnn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PlayConect"].ConnectionString);
            var cmd = new SqlCommand();
            cmd.CommandText = " SELECT * FROM V_PAINEL_GESTOR_STATUS_MAQUINAS ";

            if (cnn.State != ConnectionState.Closed) cnn.Close();
            cnn.Open();
            try
            {
                cmd.CommandTimeout = 99999;
                cmd.Connection = cnn;
                DataSet ds = new DataSet();
                DataAdapter data = new SqlDataAdapter(cmd);
                data.Fill(ds);
                queryResult.Id = 0;
                queryResult.Titulo = "";
                queryResult.Descricao = "";
                queryResult.Tipo = "SQL";
                queryResult.Qtdlinhas = ds.Tables[0].Columns.Count;
                queryResult.Dados = new string[ds.Tables[0].Rows.Count, ds.Tables[0].Columns.Count];
                queryResult.Colunas = new List<Coluna>();
                queryResult.Linhas = new List<LineData>();

                //public List<Colunas> Colunas { get; set; }
                for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                {
                    Coluna Col = new Coluna()
                    {
                        Nome = ds.Tables[0].Columns[i].ColumnName,
                        Tipo = ds.Tables[0].Columns[i].DataType.ToString()
                    };
                    queryResult.Colunas.Add(Col);
                }

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    LineData line = new LineData();
                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                    {
                        queryResult.Dados[cont, i] = row[i].ToString();
                        line.linha.Add(row[i].ToString());
                    }
                    queryResult.Linhas.Add(line);
                    cont++;
                }
            }
            catch (Exception erro)
            {
                throw new Exception("Erro ao listar tipo embalagem: " + erro.Message);
            }
            finally
            {
                cnn.Close();
            }

            return queryResult;
        }

        public Resultquery PainelGestorRanking()
        {
            var queryResult = new Resultquery();
            string[,] dados = new string[0, 0];
            int cont = 0;
            var cnn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PlayConect"].ConnectionString);
            var cmd = new SqlCommand();
            cmd.CommandText = " SELECT * FROM V_PAINEL_GESTOR_RANKING ORDER BY ORDEM, round(MED_VALOR,2) DESC ";

            if (cnn.State != ConnectionState.Closed) cnn.Close();
            cnn.Open();
            try
            {
                cmd.CommandTimeout = 99999;
                cmd.Connection = cnn;
                DataSet ds = new DataSet();
                DataAdapter data = new SqlDataAdapter(cmd);
                data.Fill(ds);
                queryResult.Id = 0;
                queryResult.Titulo = "";
                queryResult.Descricao = "";
                queryResult.Tipo = "SQL";
                queryResult.Qtdlinhas = ds.Tables[0].Columns.Count;
                queryResult.Dados = new string[ds.Tables[0].Rows.Count, ds.Tables[0].Columns.Count];
                queryResult.Colunas = new List<Coluna>();
                queryResult.Linhas = new List<LineData>(); 

                //public List<Colunas> Colunas { get; set; }
                for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                {
                    Coluna Col = new Coluna()
                    {
                        Nome = ds.Tables[0].Columns[i].ColumnName,
                        Tipo = ds.Tables[0].Columns[i].DataType.ToString()
                    };
                    queryResult.Colunas.Add(Col);
                }

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    LineData line = new LineData();
                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                    {
                        queryResult.Dados[cont, i] = row[i].ToString();
                        line.linha.Add(row[i].ToString());
                    }
                    queryResult.Linhas.Add(line);
                    cont++;
                }
            }
            catch (Exception erro)
            {
                throw new Exception("Erro ao listar tipo embalagem: " + erro.Message);
            }
            finally
            {
                cnn.Close();
            }

            return queryResult;
        }


        public Resultquery PainelGestorDesempenhoPorTurno()
        {
            var queryResult = new Resultquery();
            string[,] dados = new string[0, 0];
            int cont = 0;
            var cnn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PlayConect"].ConnectionString);
            var cmd = new SqlCommand();
            cmd.CommandText = " SELECT * FROM V_PAINEL_GESTOR_DESEMPENHO_TURNOS ";

            if (cnn.State != ConnectionState.Closed) cnn.Close();
            cnn.Open();
            try
            {
                cmd.CommandTimeout = 99999;
                cmd.Connection = cnn;
                DataSet ds = new DataSet();
                DataAdapter data = new SqlDataAdapter(cmd);
                data.Fill(ds);
                queryResult.Id = 0;
                queryResult.Titulo = "";
                queryResult.Descricao = "";
                queryResult.Tipo = "SQL";
                queryResult.Qtdlinhas = ds.Tables[0].Columns.Count;
                queryResult.Dados = new string[ds.Tables[0].Rows.Count, ds.Tables[0].Columns.Count];
                queryResult.Colunas = new List<Coluna>();
                queryResult.Linhas = new List<LineData>();

                //public List<Colunas> Colunas { get; set; }
                for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                {
                    Coluna Col = new Coluna()
                    {
                        Nome = ds.Tables[0].Columns[i].ColumnName,
                        Tipo = ds.Tables[0].Columns[i].DataType.ToString()
                    };
                    queryResult.Colunas.Add(Col);
                }

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    LineData line = new LineData();
                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                    {
                        queryResult.Dados[cont, i] = row[i].ToString();
                        line.linha.Add(row[i].ToString());
                    }
                    queryResult.Linhas.Add(line);
                    cont++;
                }
            }
            catch (Exception erro)
            {
                throw new Exception("Erro ao listar tipo embalagem: " + erro.Message);
            }
            finally
            {
                cnn.Close();
            }

            return queryResult;
        }
    }





}