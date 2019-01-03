using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Models
{
    public class MedicoesInd
    {
        public T_Indicadores Indicador { get; set; }
        public int IND_ID { get; set; }
        public int ID_FAVORITO { get; set; }
        public int? MET_ID { get; set; }
        public string TIPO_COMPARADOR { get; set; }
        public string DESC_CALCULO { get; set; }
        public int? IND_GRAFICO { get; set; }
        public int? DIM_ID { get; set; }
        public string PER_ID { get; set; }
        public T_Metas Meta { get; set; }
        public string Ano { get; set; }
        public string IND_CONEXAO { get; set; }
    }
}