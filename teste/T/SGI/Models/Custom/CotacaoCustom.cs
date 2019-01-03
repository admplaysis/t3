using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Models.Custom
{
    public class CotacaoCustom
    {
        public string Item { get; set; }
        public string Numero { get; set; }
        public string CodProduto { get; set; }
        public string Produto { get; set; }
        public string Un { get; set; }
        public double Qtd { get; set; }
        public double Preco { get; set; }
        public double Total { get; set; }
        public double ValIpi { get; set; }
        public double ValIcms { get; set; }
        public double ValFrete { get; set; }
        public string Fornecedor { get; set; }
        public string Loja { get; set; }
        public string ForNome { get; set; }
    }
}