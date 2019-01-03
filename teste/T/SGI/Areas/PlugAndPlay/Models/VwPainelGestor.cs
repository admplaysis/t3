using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class VwPainelGestor
    {
        public double OrdQuantidade { get; set; }
        public DateTime OrdDataEntregaDe { get; set; }
        public DateTime OrdDataEntregaAte { get; set; }
        public string OrdTipo { get; set; }
        public double? OrdToleranciaMais { get; set; }
        public double? OrdToleranciaMenos { get; set; }
        //fila de producao
        public DateTime FprDataInicioPrevista { get; set; }
        public DateTime FprDataFimPrevista { get; set; }
        public DateTime FprDataFimMaxima { get; set; }
        public double FprQuantidadePrevista { get; set; }
        public int RotSeqTransformacao { get; set; }

    }
}