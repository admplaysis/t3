using SGI.Areas.PlugAndPlay.Models;
using SGI.Context;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SincronizadorClpConsole
{
    class MaquinaTimer
    {
        public Timer Timer { get; set; }
        public Maquina Maquina { get; set; }
        public MaquinaTimer(Maquina maquina)
        {
            Maquina = maquina;
            Timer = new Timer(SincronizarMaquina, null, 5000, Timeout.Infinite);
        }
        private void SincronizarMaquina(object sender)
        {
            try
            {
                using (JSgi bd = new JSgi())
                {
                    HttpWebResponse response = null;
                    StreamReader streamReader = null;
                    try
                    {
                        string url = "http://" + Maquina.ControlIp + "/getLotes";
                        DateTime dataMenor = new DateTime(1970, 01, 01, 00, 00, 00);
                        DateTime dataAtual = DateTime.Now;
                        var difSeconds = Math.Floor((dataAtual - dataMenor).TotalSeconds);
                        url += "?data=" + difSeconds + "&h=";

                        var cmd = bd.Database.Connection.CreateCommand();
                        cmd.Connection.Open();
                        cmd.CommandText = @"SELECT * FROM V_QUEBRA_LOTES_CLP   WHERE CAL_ID = 0 OR CAL_ID = @CalendarioId";
                        DbParameter parametros = cmd.CreateParameter();
                        parametros.ParameterName = "CalendarioId";
                        parametros.Value = Maquina.CalendarioId;
                        cmd.Parameters.Add(parametros);

                        var dataReader = cmd.ExecuteReader();
                        List<TimeSpan> horas = new List<TimeSpan>();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                string hora = dataReader.GetString(1);
                                var arrayHora = hora.Split(':');
                                horas.Add(new TimeSpan(Convert.ToInt32(arrayHora[0]), Convert.ToInt32(arrayHora[1]), Convert.ToInt32(arrayHora[2])));
                            }
                        }
                        cmd.Connection.Close();
                        foreach (TimeSpan h in horas)
                        {
                            url += h.Hours + ":" + h.Minutes + ".";
                        }
                        string result = string.Empty;
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                        httpWebRequest.Timeout = 3000;
                        List<ClpMedicoes> medicoes = new List<ClpMedicoes>();
                        response = (HttpWebResponse)httpWebRequest.GetResponse();

                        streamReader = new StreamReader(response.GetResponseStream(), true);
                        result = streamReader.ReadToEnd();
                        if (!string.IsNullOrEmpty(result))
                        {
                            string[] dataArray;
                            string[] linhas = result.Split('|');
                            foreach (string l in linhas)
                            {
                                if (!string.IsNullOrEmpty(l.Trim()))
                                {
                                    string[] colunas = l.Split('#');
                                    ClpMedicoes medicao = new ClpMedicoes();
                                    dataArray = colunas[1].Split(new Char[] { '-', ' ', ':' });
                                    medicao.DataInicio = new DateTime(Convert.ToInt32(dataArray[0]), Convert.ToInt32(dataArray[1]),
                                        Convert.ToInt32(dataArray[2]), Convert.ToInt32(dataArray[3]), Convert.ToInt32(dataArray[4]), Convert.ToInt32(dataArray[5]), 00);
                                    dataArray = colunas[2].Split(new Char[] { '-', ' ', ':' });
                                    medicao.DataFim = new DateTime(Convert.ToInt32(dataArray[0]), Convert.ToInt32(dataArray[1]),
                                        Convert.ToInt32(dataArray[2]), Convert.ToInt32(dataArray[3]), Convert.ToInt32(dataArray[4]), Convert.ToInt32(dataArray[5]), 00);
                                    medicao.Quantidade = Convert.ToDouble(colunas[3]);
                                    medicao.OrdemProducaoId = ""; /*colunas[4];*/
                                    medicao.IdLoteClp = Convert.ToInt32(colunas[0]);
                                    medicao.MaquinaId = Maquina.Id;
                                    medicao.ProdutoId = "";/*colunas[5];*/
                                    medicao.Status = 0;
                                    medicao.SequenciaTransformacaoId = 0;/*!string.IsNullOrEmpty(colunas[6]) ? Convert.ToInt32(colunas[6]) : 0;*/
                                    medicao.SequenciaRepeticaoId = 0;/*!string.IsNullOrEmpty(colunas[7]) ?  Convert.ToInt32(colunas[7]) : 0;*/
                                    medicao.OcorrenciaId = "";
                                    medicao.FilaProducaoId = 0;
                                    medicao.ClpOrigem = "";
                                    medicao.Grupo = 1.2;
                                    medicoes.Add(medicao);
                                }
                            }
                            url = "http://" + Maquina.ControlIp + "/confirmLotes?l=";
                            foreach (ClpMedicoes med in medicoes)
                            {
                                if (bd.ClpMedicoes.Where(x => x.IdLoteClp == med.IdLoteClp
                                        && x.DataInicio == med.DataInicio).Count() == 0)
                                {
                                    bd.ClpMedicoes.Add(med);
                                }
                                url += med.IdLoteClp + ".";
                            }
                            bd.SaveChanges();
                            response.Close();
                            httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                            response = (HttpWebResponse)httpWebRequest.GetResponse();
                            response.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        if (response != null)
                            response.Close();
                    }
                    finally
                    {
                        if (streamReader != null)
                            streamReader.Close();
                    }
                }
            }
            catch (Exception e)
            {

            }
            finally {
                Timer.Change(5000, Timeout.Infinite);
            }
        }
    }
}
