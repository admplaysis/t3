using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Models.Custom
{
    [NotMapped]
    public class PedidoCustom
    {
        public PedidoCustom()
        {
            this.ItensPedCompra = new List<ItensPedCompraCustom>();
            this.LstAprovacoes = new List<AprovacoesCustom>();
            this.Cotacoes = new List<CotacaoCustom>();
        }

        public string Empresa { get; set; }
        public string NumAprovacao { get; set; }
        public string Pedido { get; set; }
        public string Usuario { get; set; }
        public string Emissao { get; set; }
        public string DtLiberacao { get; set; }
        public string Fornecedor { get; set; }
        public string Loja { get; set; }
        public string ForNome { get; set; }
        public string StatusSCR { get; set; }
        public string TpFrete { get; set; }
        public string CondPag { get; set; }
        public double Moeda { get; set; }
        public double Total { get; set; }
        public string ObsAprovacao { get; set; }
        public string AprovadorSc { get; set; }
        public string UserAprovador { get; set; }
        public string MotivoRejeicao { get; set; }
        public List<ItensPedCompraCustom> ItensPedCompra { get; set; }
        public virtual AprovadorCustom Aprovador { get; set; }
        public virtual SolicitanteCustom Solicitante { get; set; }
        public virtual CompradorCustom Comprador { get; set; }
        public List<AprovacoesCustom> LstAprovacoes { get; set; }
        public List<CotacaoCustom> Cotacoes { get; set; }
    }
}