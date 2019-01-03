using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SGI.Context;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class Mensagem
    {
        public string MEN_ID { get; set; }
        public string MEN_MENSAGEM { get; set; }
        public DateTime MEN_EMISSAO { get; set; }
        public Boolean AddMensagem(JSgi db, Mensagem m)
        {
            Mensagem Men = null;
            Men = db.Mensagens.Find(m.MEN_ID);
            if (Men == null)
            {
                m.MEN_EMISSAO = DateTime.Now;
                db.Mensagens.Add(m);
            }
            else
            {
                db.Entry(Men).State = EntityState.Modified;
                Men.MEN_EMISSAO = DateTime.Now;
                Men.MEN_MENSAGEM = m.MEN_MENSAGEM;
                db.SaveChanges();
            }
            db.SaveChanges();
            m = null;
            Men = null;
            return true;
        }

/*        internal void AddMensagem(Mensagem m)
        {
            throw new NotImplementedException();
        }*/
    }
}