using SGI.Context;
using SGI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SGI.Util
{
    public static class ExtratorDre
    {
        /// <summary>
        /// Metódo para buscar os dados da dre no banco de dados
        /// </summary>
        /// <param name="idVisao">Código da visão</param>
        /// <param name="data1">Data inicial</param>
        /// <param name="data2">Data final</param>
        /// <returns></returns>
        public static List<DreView> DrePadrao(int idVisao, string data)
        {
            JSgi db = new JSgi();
            //Query banco de dados
            #region Query
            string query = "";
            query += "select p.PLA_ID,isnull(m.MOV_DATA,'')MOV_DATA,p.PLA_TIPO,ISNULL(m.MOV_UNID,0)MOV_UNID,u.UNI_DESCRICAO,p.PLA_CODIGO,p.PLA_DESCRICAO,isnull(sum(m.MOV_VALOR),0) VALOR from TR_VISOES v ";
            query += "inner join TR_PLANOCONTAS p on p.PLA_ID = v.VIS_PLANID ";
            query += "left join TR_MOVIMENTOS m on m.MOV_PLAID = p.PLA_ID and left(m.MOV_DATA,len(@data)) = @data ";
            query += "left join TR_UNIDADE u on u.UNI_ID = m.MOV_UNID ";
            query += "where v.CAB_ID = @idVisao ";
            query += "group by p.PLA_ID,p.PLA_CODIGO,p.PLA_DESCRICAO,p.PLA_TIPO,m.MOV_UNID,u.UNI_DESCRICAO,m.MOV_DATA ";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@idVisao", idVisao));
            parameters.Add(new SqlParameter("@data", data));
            var dre = db.Database.SqlQuery<DreView>(query, parameters.ToArray()).ToList();
            #endregion Query

            //Soma contas sinteticas
            /*    #region SomaSinteticas
            var sinteticasSomadas = new List<DreView>();
            foreach (var item in dre.Where(x => x.PLA_TIPO == (int)Enums.TipoConta.Sintetica).ToList())
            {
                foreach (var meses in dre.Where(x => !String.IsNullOrEmpty(x.MOV_DATA)).GroupBy(x => new { mes = x.MOV_DATA.Substring(4, 2) }).Select(x => new { x.Key.mes }).OrderBy(x => x.mes).OrderBy(x => x.mes))
                {
                    //Filtra valores por unidade de medida
                    var valores = dre.Where(x => !String.IsNullOrEmpty(x.MOV_DATA) && x.MOV_DATA.Substring(0, 6) == data.Substring(0, 4) + meses.mes && x.PLA_CODIGO.Substring(0, Math.Min(item.PLA_CODIGO.Length, x.PLA_CODIGO.Length)) == item.PLA_CODIGO).GroupBy(x => new { x.UNI_DESCRICAO, x.MOV_UNID }).Select(x => new { valor = x.Sum(j => j.Valor), UNI_DESCRICAO = x.Key.UNI_DESCRICAO, MOV_UNID = x.Key.MOV_UNID });
                    //Percorre e adiciona contas sinteticas
                    foreach (var valor in valores)
                    {
                        sinteticasSomadas.Add(new DreView { MOV_DATA = data.Substring(0, 4) + meses.mes + "01", MOV_UNID = valor.MOV_UNID, PLA_CODIGO = item.PLA_CODIGO, PLA_DESCRICAO = item.PLA_DESCRICAO, PLA_ID = item.PLA_ID, PLA_TIPO = item.PLA_TIPO, UNI_DESCRICAO = valor.UNI_DESCRICAO, Valor = valor.valor });
                    }
                }
            }
            dre.AddRange(sinteticasSomadas);
            #endregion SomaSinteticas

*/
            return dre.OrderBy(x => x.PLA_CODIGO).ToList();
        }
    }
}