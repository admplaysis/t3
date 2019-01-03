using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Models.Custom
{
    public class ItensNfCustom
    {
        public string Empresa { get; set; }
        public string CodProduto { get; set; }
        public string Produto { get; set; }
        public string Un { get; set; }
        public string Item { get; set; }
        public double Qtd { get; set; }
        public string DtUtmComp { get; set; }
        public double UltPrcCompra { get; set; }
        public double VlrUnit { get; set; }
        public double VlrTotal { get; set; }
        public double VlrIcms { get; set; }
        public double VlrIpi { get; set; }
        public string Pedido { get; set; }
        public string CC { get; set; }
        public int RecF1 { get; set; }
    }
}