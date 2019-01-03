using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Models
{
    public class DreView
    {
        public int PLA_ID { get; set; }
        public string PLA_CODIGO { get; set; }
        public string PLA_DESCRICAO { get; set; }
        public string MOV_DATA { get; set; }
        public decimal Valor { get; set; }
        public int PLA_TIPO { get; set; }
        public int MOV_UNID { get; set; }
        public string UNI_DESCRICAO { get; set; }
    }
}