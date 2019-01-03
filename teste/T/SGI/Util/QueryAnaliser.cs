using SGI.Context;
using SGI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace SGI.Util
{
    public static class QueryAnaliser
    {
        public static List<ViewsCampos> GetCamposProcedure(string nome)
        {
            List<ViewsCampos> lstNomes = new List<ViewsCampos>();
            var cnn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PlayConect"].ConnectionString);
            var cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = @"EXEC " + nome;
            if (cnn.State != ConnectionState.Closed) cnn.Close();
            cnn.Open();
            try
            {
                cmd.CommandTimeout = 99999;
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        lstNomes.Add(new ViewsCampos() { Nome = reader.GetName(i) });
                    }
                }
            }
            catch (Exception erro)
            {
                throw new Exception("Erro ao executar procedure: " + erro.Message);
            }
            finally
            {
                cnn.Close();
            }

            return lstNomes;
        }

        public static string[,] GetValores(string tipo, string viewNome, string data1, string data2)
        {
            data1 = DateTime.Parse(data1).ToString("yyyyMMdd");
            data2 = DateTime.Parse(data2).ToString("yyyyMMdd");
            string[,] dados = new string[0, 0];
            int cont = 0;
            int qtdLinhas = GetLinhas(tipo, viewNome, data1, data2);
            var cnn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PlayConect"].ConnectionString);
            var cmd = new SqlCommand();
            cmd.Connection = cnn;
            if (tipo == "view")
            {
                cmd.CommandText = @"select * from " + viewNome;
                cmd.CommandText += " where (DATA between @data1 and @data2) or (DATA = '') ";
                cmd.Parameters.Add("@data1", SqlDbType.VarChar).Value = data1;
                cmd.Parameters.Add("@data2", SqlDbType.VarChar).Value = data2;
            }
            else
            {
                cmd.CommandText = @"EXEC " + viewNome;
            }
            if (cnn.State != ConnectionState.Closed) cnn.Close();
            cnn.Open();
            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                dados = new string[qtdLinhas, reader.FieldCount];
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        dados[cont, i] = reader[i].ToString();
                    }
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
            return dados;
        }

        public static int GetLinhas(string tipo, string viewNome, string data1, string data2)
        {
            int linhas = 0;
            var strConexao = System.Configuration.ConfigurationManager.ConnectionStrings["PlayConect"].ConnectionString;
            var cnn = new SqlConnection(strConexao);
            var cmd = new SqlCommand();
            cmd.Connection = cnn;
            if (tipo == "view")
            {
                cmd.CommandText = @"select COUNT(*)TOTAL from " + viewNome;
                cmd.CommandText += " where (DATA between @data1 and @data2) or (DATA = '') ";
                cmd.Parameters.Add("@data1", SqlDbType.VarChar).Value = data1;
                cmd.Parameters.Add("@data2", SqlDbType.VarChar).Value = data2;
            }
            else
            {
                cmd.CommandText = @"EXEC " + viewNome;
            }
            if (cnn.State != ConnectionState.Closed) cnn.Close();
            cnn.Open();
            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (tipo == "view")
                        linhas = Int32.Parse(reader["TOTAL"].ToString());
                    else
                        linhas++;
                }
            }
            catch (Exception erro)
            {
                throw new Exception("Conta linhas views: " + erro.Message);
            }
            finally
            {
                cnn.Close();
            }
            return linhas;
        }

        /// <summary>
        /// Rotina para retornar valores referente ao ano Anterior
        /// </summary>
        /// <param name="ano">Ano atual</param>
        /// <param name="idMeta">Id Meta</param>
        /// <returns>Lista de dados do tipo vw_SGI_PARAMETRO_RELMEDICOES</returns>
        public static List<vw_SGI_PARAMETRO_RELMEDICOES> AnoAnterior(string ano, int idIndicador)
        {
            JSgi db2 = new JSgi();
            var anoAtual = DateTime.Parse("01/01/" + ano);
            var valores = new List<vw_SGI_PARAMETRO_RELMEDICOES>();
            string query = "select * from vw_SGI_PARAMETRO_RELMEDICOES WHERE IND_ID = '" + idIndicador.ToString() + "' ";
            //Filtra ano atual
            if (ano != "" && ano != null)
                query += "AND LEFT(MES,4) = '" + anoAtual.AddYears(-1).Year.ToString() + "' ";
            query += "order by IND_ID,Mes";
            valores = db2.Database.SqlQuery<vw_SGI_PARAMETRO_RELMEDICOES>(query).ToList();
            return valores;
        }

        /// <summary>
        /// Rotina para retornar valores referente ao ano Anterior
        /// </summary>
        /// <param name="ano">Ano atual</param>
        /// <returns>Lista de dados do tipo vw_SGI_PARAMETRO_RELMEDICOES</returns>
        public static List<vw_SGI_PARAMETRO_RELMEDICOES> AnoAnterior(string ano)
        {
            JSgi db2 = new JSgi();
            var anoAtual = DateTime.Parse("01/01/" + ano);
            var valores = new List<vw_SGI_PARAMETRO_RELMEDICOES>();
            string query = "select * from vw_SGI_PARAMETRO_RELMEDICOES ";
            //Filtra ano atual
            query += "where LEFT(MES,4) = '" + anoAtual.AddYears(-1).Year.ToString() + "' ";
            query += "order by IND_ID,Mes";
            valores = db2.Database.SqlQuery<vw_SGI_PARAMETRO_RELMEDICOES>(query).ToList();
            return valores;
        }

        public static List<String> GetOrderBy(int idObject)
        {
            var lstCampos = new List<String>();
            try
            {
                JSgi db2 = new JSgi();
                string query = "SELECT name Nome FROM sys.columns ";
                query += "where object_id = '" + idObject + "' ";
                lstCampos = db2.Database.SqlQuery<String>(query).ToList();
            }
            catch (Exception erro)
            {
                throw erro;
            }
            return lstCampos;
        }
    }
}
