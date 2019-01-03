using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class CorConfiguracaoGrafico
    {
        public string Id { get; set; }
        public double PercentualIni { get; set; }
        public double PercentualFim { get; set; }
        public string Descricao { get; set; }
    }
}