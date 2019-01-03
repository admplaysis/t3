using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SGI.Models.Custom
{
    [NotMapped]
    public class ItensPedCompraCustom
    {
        public string Pedido { get; set; }
        public string Item { get; set; }
        public string CodProduto { get; set; }
        public string Produto { get; set; }
        public string Un { get; set; }
        public double Qtd { get; set; }
        public double Preco { get; set; }
        public double Ipi { get; set; }
        public double Total { get; set; }
        public double ValDesc { get; set; }
        public double ValDesp { get; set; }
        public string DtEntrega { get; set; }
        public double UltPrcCopra { get; set; }
        public string DtUlPrc { get; set; }
        public string CCusto { get; set; }
        public string NumSc { get; set; }
        public string Justificativa { get; set; }
        public string EspTecnica { get; set; }
        public string Obs { get; set; }
    }
}