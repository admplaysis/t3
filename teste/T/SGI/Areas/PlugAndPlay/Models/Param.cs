using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SGI.Context;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class Param
    {
        public string PAR_ID { get; set; }
        public string PAR_DESCRICAO { get; set; }
        public string PAR_VALOR_S { get; set; }
        public Double PAR_VALOR_N { get; set; }
        public Boolean AddParametro(JSgi db, Param p)
        {
            Param Par = db.Parametros.Find(p.PAR_ID);
            if (Par == null)
            {
                Par.PAR_ID = p.PAR_ID;
                Par.PAR_DESCRICAO = p.PAR_DESCRICAO;
                Par.PAR_VALOR_S = p.PAR_VALOR_S;
                Par.PAR_VALOR_N = p.PAR_VALOR_N;
                db.Parametros.Add(Par);
            }
            else
            {
                db.Entry(Par).State = EntityState.Modified;
                Par.PAR_ID = p.PAR_ID;
                Par.PAR_DESCRICAO = p.PAR_DESCRICAO;
                Par.PAR_VALOR_S = p.PAR_VALOR_S;
                Par.PAR_VALOR_N = p.PAR_VALOR_N;
            }
            db.SaveChanges();
            return true;
        }

    }
}