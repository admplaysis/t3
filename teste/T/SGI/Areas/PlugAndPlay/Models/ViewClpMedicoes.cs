using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class ViewClpMedicoes
    {
        public string MaquinaId { get; set; }
        public DateTime DataIni { get; set; }
        public  DateTime DataFim { get; set; }
        public double Quantidade { get; set; }
        public double? Grupo { get; set; }
        public string TurnoId { get; set; }
        public string TurmaId { get; set; }
        public string FeedbackObs { get; set; }
        public string OcoId { get; set; }
        public int? FeedBackId { get; set; }
        public int? FeedBackIdMov { get; set; }
    }
}