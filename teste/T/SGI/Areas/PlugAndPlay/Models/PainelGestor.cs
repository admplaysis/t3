using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class PainelGestor
    {
        public int MaqID { get; set; }
        public string MaqDescricao { get; set; }
        public string MaqStatus { get; set; }
        public string UltimaAtutlizacao { get; set; }
        public string OcorenciaID { get; set; }
        public string OcorenciaDescricao { get; set; }
        public string Obs { get; set; }
        public string StatusCor { get; set; }
        public string FeedbacksPendentes { get; set; }
        public string TempoSemFeedback { get; set; }
        public string OpsParciais { get; set; }
    }
}