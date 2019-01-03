using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using SGI.Areas.PlugAndPlay.Models;
using SGI.Context;
using Z.EntityFramework.Plus;


namespace SGI.classes
{
    public class GlobalFunctons
    {
        private GlobalFunctons() { }
        
        private static GlobalFunctons instancia;

        public static void SetProduzindo(JSgi bd, string maquina, string produto, string order, int seqRep, int seqTran)
        {
            try
            {
                bd.Database.ExecuteSqlCommand(@" UPDATE T_FILA_PRODUCAO SET FPR_PRODUZINDO = 0  WHERE ROT_MAQ_ID= '" + maquina + "'  AND FPR_PRODUZINDO = 1 ");
                string sql = " UPDATE T_FILA_PRODUCAO SET FPR_PRODUZINDO = 1  WHERE ORD_ID = '" + order + "' AND ROT_PRO_ID = '" + produto + "' AND ROT_MAQ_ID = '" + maquina + "' AND ROT_SEQ_TRANFORMACAO = " + seqTran + " AND FPR_SEQ_REPETICAO = " + seqRep;
                bd.Database.ExecuteSqlCommand(sql);
                bd.Database.ExecuteSqlCommand(" SP_PLUG_SET_FASES_CLP_PENDENTES '" + maquina + "'");
                bd.Database.ExecuteSqlCommand(" SP_PLUG_CALCULA_FILAS '" + maquina + "'");
            }
            catch (Exception ex) {
                Console.WriteLine("Erro ao executar SQL:"+ex);
            }

            /*Mensagem m = new Mensagem();
            m.MEN_ID = "SP_PLUG_SET_FASES_CLP_PENDENTES" + maquina;
            m.MEN_MENSAGEM = "PENDENTE";
            m.AddMensagem(bd, m);
            m = null;*/

        }

        public static void SetPrimeiraProducao(JSgi bd, string Maquina)
        {
            // DEFINI OPE PRODUZINDO CASO NAO ENCONTRE NENHUM PEDIDO 
            if (bd.VwFilaProducao.Count(f => f.RotMaqId == Maquina && f.Produzindo == 1) < 1)
            {
                var Op = bd.VwFilaProducao.Where(f => f.RotMaqId == Maquina).OrderBy(f => f.FprDataInicioPrevista).Take(1).FirstOrDefault();
                if (Op != null)
                {
                    string sql = " UPDATE T_FILA_PRODUCAO SET FPR_PRODUZINDO = 1 " +
                        " WHERE ORD_ID = '" + Op.OrdId + "' AND ROT_" +
                        "PRO_ID = '" + Op.PaProId + "' " +
                        " AND ROT_MAQ_ID = '" + Maquina + "' AND ROT_SEQ_TRANFORMACAO = " + Op.RotSeqTransformacao + " " +
                        " AND FPR_SEQ_REPETICAO = " + Op.FprSeqRepeticao;
                    bd.Database.ExecuteSqlCommand(sql);
                }
            }
        }



        public static void SendMensagem(string msgid,string msg)
        {
            using (JSgi db = new JSgi())
            {
                Mensagem Men = null;
                Men = db.Mensagens.Find(msgid);
                if (Men == null)
                {
                    Mensagem m = new Mensagem();
                    m.MEN_ID = msgid;
                    m.MEN_EMISSAO = DateTime.Now;
                    m.MEN_MENSAGEM = msg;
                    db.Mensagens.Add(m);
                }
                else
                {
                    db.Entry(Men).State = EntityState.Modified;
                    Men.MEN_EMISSAO = DateTime.Now;
                    Men.MEN_MENSAGEM = msg;
                }
                db.SaveChanges();
                Men = null;
            }
        }



        public static string GetMensagem(string msgid)
        {
            using (JSgi db = new JSgi())
            {
                Mensagem Men = null;
                Men = db.Mensagens.Find(msgid);
                if (Men == null)
                {
                    return "";
                }
                else
                {
                    return Men.MEN_MENSAGEM;
                }
            }
        }



        public static  GlobalFunctons Instancia
        {
            get
            {
                if (instancia == null) {

                    instancia = new GlobalFunctons();
                }
                return instancia;
            }
        }
    }
}