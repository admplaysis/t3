using SGI.Models;
using SGI.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using SGI.Context;



namespace SGI.Controllers
{
    [CustomAuthorize(Roles = "Gerente, AdiminstradorTI, AdiminstradorPCP")]
    public class ConsultasController : Controller
    {
        private JSgi db = new JSgi();
        // GET: Consultas
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListaConsultas(String sPesquisa)
        {
            var queryResult = new Resultquery();
            string[,] dados = new string[0, 0];
            int cont = 0;
            var cnn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PlayConect"].ConnectionString);
            var cmd = new SqlCommand();
            cmd.CommandText = "SELECT CON_ID ID,CON_TITULO TITULO,CON_DESCRICAO DESCRICAO,CON_GRUPO GRUPO FROM T_CONSULTAS ";

            if (!string.IsNullOrEmpty(sPesquisa))
            {
                cmd.CommandText = " SELECT CON_ID ID,CON_TITULO TITULO,CON_DESCRICAO DESCRICAO,CON_GRUPO GRUPO  FROM T_CONSULTAS WHERE CON_DESCRICAO LIKE  @PESQUISA  OR CON_GRUPO LIKE  @PESQUISA  OR CON_ID LIKE @PESQUISA ";
                cmd.Parameters.Add("@PESQUISA", SqlDbType.NVarChar).Value = "%" + sPesquisa + "%";
            }

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
                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                        queryResult.Dados[cont, i] = row[i].ToString();
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

            return View(queryResult);
        }





        public ActionResult ExecutaConsultaGrafico(int indId, int dimensao, string periodo,string subDim, string dataIni, string dataFim, Boolean excel = true)
        {
            var queryResult = new Resultquery();
            List<Parametro> aiparam = new List<Parametro>();
            string sql = "";
            string cipartemp = "";
            bool bicopia = false;
            int cont = 0;
            bool lijatem = false;
            int nipos = 0;
            string cichar = "";
            int niini = 0;
            var conexao = "PlayConect";
            excel = true;
            var ind =  db.Database.SqlQuery<IndicadorDimencao>("select * from T_INDICADORES_DIMENCOES WHERE IND_ID = "+ indId + " AND DIM_ID = "+ dimensao ).ToList();
            foreach (var item in ind)
            {
                sql = item.DIM_SQL.ToUpper();
            }
            sql = sql.Replace("SELECT","");
            int ifrom = 0;
            int iwhere = 0;
            for (int i = 0; i < sql.Length -5; i++)
            {
                
                if (sql.Substring(i,4).ToUpper() == "FROM")
                {
                    ifrom = i;
                    iwhere = sql.Length+1;
                }
                if (sql.Substring(i, 5).ToUpper() == "WHERE")
                {
                    iwhere = i;
                }
            }
            string campos = sql.Substring(0, ifrom - 1);
            string tabela = sql.Substring(ifrom + 5,iwhere-5-ifrom);
            string[] sql_split = campos.Split(',');
            sql = "SELECT * FROM "+ tabela;
            string where = "";
            //indId, dimensao, subDimensao, dataIni, dataFim, periodo
            if (subDim != "")
                where += " WHERE "+sql_split[0] + "'" + subDim + "'";
            if (periodo.Trim() == "D")
            {
                if (where.Length > 0)
                    where += " AND ";
                else
                    where += " WHERE ";
                where += sql_split[2] + " BETWEEN '" + dataIni.Substring(6, 4) + dataIni.Substring(3, 2) + dataIni.Substring(0, 2) + "' AND '" + dataFim.Substring(6, 4) + dataFim.Substring(3, 2) + dataFim.Substring(0, 2) + "'";
            }

            // montagem da query por parametro 
            sql += where; 
            var cnn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[conexao].ConnectionString);
            var cmd = new SqlCommand();
            try
            {
                 cmd.Connection = cnn;
                 if (cnn.State != ConnectionState.Closed)
                    cnn.Close();
                cnn.Open();
                cmd.CommandText = "USE " + cnn.Database;
                cmd.ExecuteNonQuery();
                cmd.CommandText = sql;
                    
                cmd.CommandTimeout = 99999;
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
                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                        queryResult.Dados[cont, i] = row[i].ToString();
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
            queryResult.Parametros = aiparam;




            if (excel)
            {
                var products = new System.Data.DataTable(queryResult.Titulo);
                //Adiciona colunas
                for (int i = 0; i < queryResult.Colunas.Count; i++)
                {
                    products.Columns.Add(queryResult.Colunas[i].Nome, typeof(string));
                }
                //Preenche valores
                string[] valores;
                for (int ln = 0; ln < queryResult.Dados.GetLength(0); ln++)
                {
                    valores = new string[queryResult.Colunas.Count];
                    for (int col = 0; col < queryResult.Colunas.Count; col++)
                    {
                        valores[col] = queryResult.Dados[ln, col];
                    }
                    products.Rows.Add(valores);
                }


                var grid = new GridView();
                grid.DataSource = products;
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=" + queryResult.Titulo + DateTime.Now.Date.ToString("ddMMyyyy") + ".xls");
                Response.ContentType = "application/vnd.ms-excel";

                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }

            return View(queryResult);
        }


        public ActionResult ExecutaConsulta(int id, FormCollection paramforms)
        {
            var queryResult = new Resultquery();
            List<Parametro> aiparam = new List<Parametro>();
            string sql = "";
            string cipartemp = "";
            bool bicopia = false;
            int cont = 0;
            bool lijatem = false;
            int nipos = 0;
            string cichar = "";
            int niini = 0;
            bool excel = false;
            string conexao = "";

            var cnn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PlayConect"].ConnectionString);
            var cmd = new SqlCommand();
            cmd.CommandText = "SELECT CON_ID,CON_DESCRICAO,CON_GRUPO,CON_COMAND,CON_CONEXAO,CON_TITULO,CON_TIPO FROM T_CONSULTAS WHERE CON_ID = @ID ";
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
            cmd.Connection = cnn;

            if (cnn.State != ConnectionState.Closed) cnn.Close();
            cnn.Open();
            try
            {
                // Montando parametros PARA FORMANURIO 
                if (paramforms.Count == 0)
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        queryResult.Id = (int)reader["CON_ID"];
                        queryResult.Titulo = reader["CON_TITULO"].ToString();
                        queryResult.Descricao = reader["CON_DESCRICAO"].ToString();
                        queryResult.Tipo = reader["CON_TIPO"].ToString();
                        queryResult.Qtdlinhas = 0;
                        //queryResult.Dados = new string[ds.Tables[0].Rows.Count, ds.Tables[0].Columns.Count];
                        //queryResult.Colunas = new List<Coluna>();

                        sql = reader["CON_COMAND"].ToString();
                        conexao = reader["CON_CONEXAO"].ToString().Trim();

                    }
                    for (int i = 0; i < sql.Length; i++)
                    {
                        if (sql.Substring(i, 1) == "@")
                        {
                            cipartemp = "";
                            bicopia = true;
                            nipos = i + 1;
                            cichar = sql.Substring(nipos, 1);
                            niini = i;
                            while (cichar != "@")
                            {
                                nipos++;
                                cichar = sql.Substring(nipos, 1);
                            }
                            i = nipos + 1;
                            cipartemp = sql.Substring(niini, nipos - (niini) + 1);
                            lijatem = false;
                            foreach (Parametro p in aiparam)
                            {
                                if (p.Idfull == cipartemp)
                                {
                                    lijatem = true;
                                }
                            }
                            if (!lijatem)
                            {
                                var aiparamtemp = cipartemp.Split('#');
                                // exemplo      @Titulo#  Tipo campo(c,d,dtos,i)  # PARAMETRO_ID # valor defalt # Tabela de pesquisa#@
                                Parametro Pal = new Parametro()
                                {
                                    Idfull = cipartemp,
                                    Idform = aiparamtemp[2].Replace("@", ""),
                                    Descricao = aiparamtemp[0].Replace("@", ""),
                                    Tipo = aiparamtemp[1].Replace("@", ""),
                                    Conteudo = aiparamtemp[3].Replace("@", "")
                                };
                                aiparam.Add(Pal);
                            }
                        }
                    }

                }
                else
                {


                    /*foreach (KeyValuePair<int, String> item in paramforms)
                    {
                        //item.key
                        //item.value
                    }
                    foreach (var key in paramforms.AllKeys)
                    {
                        var value = paramforms[key];
                    }

                    */
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        queryResult.Id = (int)reader["CON_ID"];
                        queryResult.Titulo = reader["CON_TITULO"].ToString();
                        queryResult.Descricao = reader["CON_DESCRICAO"].ToString();
                        queryResult.Tipo = reader["CON_TIPO"].ToString();
                        queryResult.Qtdlinhas = 0;
                        sql = reader["CON_COMAND"].ToString();
                        conexao = reader["CON_CONEXAO"].ToString().Trim();
                    }
                    string parfull = "";
                    string conteudo = "";

                    foreach (var key in paramforms.Keys)
                    {
                        if (key.ToString() == "EXCEL")
                            excel = true;
                        var value = paramforms[key.ToString()];
                        if (key.ToString().Substring(0, 4) == "REP_")
                        {

                            parfull = "FULL_" + key.ToString().Substring(4, key.ToString().Length - 4);
                            parfull = paramforms[parfull];
                            conteudo = paramforms[key.ToString()];
                            sql = sql.Replace(parfull, conteudo);

                            // exemplo      @Pesquisar#C#PARAMETRO_ID#@
                            var aiparamtemp = parfull.Split('#');
                            Parametro Pal = new Parametro()
                            {
                                Idfull = parfull,
                                Idform = aiparamtemp[2].Replace("@", ""),
                                Descricao = aiparamtemp[0].Replace("@", ""),
                                Tipo = aiparamtemp[1].Replace("@", ""),
                                Conteudo = conteudo
                            };
                            aiparam.Add(Pal);
                        }
                    }

                    reader.Close();

                    var cnn2 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[conexao].ConnectionString);
                    var cmd2 = new SqlCommand();
                    cmd2.Connection = cnn;
                    if (cnn2.State != ConnectionState.Closed) cnn2.Close();
                        cnn2.Open();
                    cmd2.CommandText = "USE "+ cnn2.Database;
                    cmd2.ExecuteNonQuery();
                    cmd2.CommandText = sql;

                    cmd2.CommandTimeout = 99999;
                    DataSet ds = new DataSet();
                    DataAdapter data = new SqlDataAdapter(cmd2);
                    data.Fill(ds);
                    queryResult.Id = 0;
                    queryResult.Titulo = "";
                    queryResult.Descricao = "";
                    queryResult.Tipo = "SQL";
                    queryResult.Qtdlinhas = ds.Tables[0].Columns.Count;
                    queryResult.Dados = new string[ds.Tables[0].Rows.Count, ds.Tables[0].Columns.Count];
                    queryResult.Colunas = new List<Coluna>();

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
                        for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                            queryResult.Dados[cont, i] = row[i].ToString();
                        cont++;
                    }
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
            queryResult.Parametros = aiparam;




            if (excel)
            {
                var products = new System.Data.DataTable(queryResult.Titulo);
                //Adiciona colunas
                for (int i = 0; i < queryResult.Colunas.Count; i++)
                {
                    products.Columns.Add(queryResult.Colunas[i].Nome, typeof(string));
                }
                //Preenche valores
                string[] valores;
                for (int ln = 0; ln < queryResult.Dados.GetLength(0); ln++)
                {
                    valores = new string[queryResult.Colunas.Count];
                    for (int col = 0; col < queryResult.Colunas.Count; col++)
                    {
                        valores[col] = queryResult.Dados[ln, col];
                    }
                    products.Rows.Add(valores);
                }


                var grid = new GridView();
                grid.DataSource = products;
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=" + queryResult.Titulo + DateTime.Now.Date.ToString("ddMMyyyy") + ".xls");
                Response.ContentType = "application/vnd.ms-excel";

                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }

            return View(queryResult);
        }


        /*// monta parametros
	For i := 1 to Len(cISql)
		if substr(cISql,i,1) = "@"
		   	CiPartemp := ""
			BiCopia := .T.
			
			//////////////
			nIPos := i + 1
			cIChar := Substr(cISql,nIPos,1)
			nIIni := i
			While cIChar # "@"
				nIPos++
				cIChar := Substr(cISql,nIPos,1)
			Enddo
			i := nIPos + 1
			CiPartemp := Substr(cISql,nIIni,nIPos-(nIIni)+1)

			lIJatem := .F.
			For i2 = 1 to Len(aIParam)
				If aIParam[i2][1] = cIParTemp
					lIJatem := .T.
					Exit
				Endif
			Next i2
			
			If !liJatem
				aiParamTemp := {}
				aiParamTemp := Separa(CiPartemp,"#")
				aadd(aiParam,{CiPartemp,aiParamTemp[1],aiParamTemp[2],aiParamTemp[3],aiParamTemp[4]})
			Endif
		Elseif Substr(cISql,i,1) = "$"
			nIPos := i + 1
			cIChar := Substr(cISql,nIPos,1)
			nIIni := i
			While cIChar # "$"
				nIPos++
				cIChar := Substr(cISql,nIPos,1)
			Enddo
			i := nIPos + 1
			cIMacro := Substr(cISql,nIIni+1,nIPos-(nIIni+1))
			cISqlTemp := &cIMacro 
			AADD(aIMacro,{"$"+cIMacro+"$",cISqlTemp})
		EndIf
	Next*/



    }



}
