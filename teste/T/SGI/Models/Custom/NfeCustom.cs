using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Models.Custom
{
    [NotMapped]
    public class NfeCustom
    {
        public NfeCustom()
        {
            this.ItensNfe = new List<ItensNfCustom>();
            this.LstAprovacoes = new List<AprovacoesCustom>();
        }

        public string Empresa { get; set; }
        public string Origem { get; set; }
        public double Moeda { get; set; }
        public string Status { get; set; }
        public string Filial { get; set; }
        public string Emissao { get; set; }
        public string Numero { get; set; }
        public string Serie { get; set; }
        public string Fornecedor { get; set; }
        public string Loja { get; set; }
        public string NomeFor { get; set; }
        public string CondPag { get; set; }
        public double Desconto { get; set; }
        public double VlrBruto { get; set; }
        public double VlrMerc { get; set; }
        public double Frete { get; set; }
        public string TipoFrete { get; set; }
        public int Recno { get; set; }
        public List<ItensNfCustom> ItensNfe { get; set; }
        public List<AprovacoesCustom> LstAprovacoes { get; set; }
    }
}