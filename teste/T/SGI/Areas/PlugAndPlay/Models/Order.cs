using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class Order
    {
        public Order()
        {
            MovimentosEstoque = new HashSet<MovimentoEstoque>();
            Medicoes = new HashSet<Feedback>();
            FilasProducao = new HashSet<FilaProducao>();
            TargetsProduto = new HashSet<TargetProduto>();
        }
        [Display(Name = "ID")]
        public string Id { get; set; }
        [Display(Name = "Data Entrega De")]
        public DateTime DataEntregaDe { get; set; }
        [Display(Name = "Data Entrega Até")]
        public DateTime DataEntregaAte { get; set; }
        [Display(Name = "Quantidade")]
        public double Quantidade { get; set; }
        [Display(Name = "Preço Unitário")]
        public double? PrecoUnitario { get; set; }
        [Display(Name = "Tipo")]
        public string Tipo { get; set; }
        [Display(Name = "Tolerância +")]
        public double? ToleranciaMais { get; set; }
        [Display(Name = "Tolerância -")]
        public double? ToleranciaMenos { get; set; }
        [Display(Name = "Cliente")]
        public String ClienteId { get; set; }
        [Display(Name = "Produto")]
        public string ProdutoId { get; set; }
        public Cliente Cliente { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual ICollection<MovimentoEstoque> MovimentosEstoque { get; set; }
        public virtual ICollection<Feedback> Medicoes { get; set; }
        public virtual ICollection<FilaProducao> FilasProducao{ get; set; }
        public virtual ICollection<TargetProduto> TargetsProduto { get; set; }
    }
}