using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class Ocorrencia
    {
        public Ocorrencia()
        {
            Medicoes = new HashSet<Feedback>();
            TarProdOcoPers = new HashSet<TargetProduto>();
            TarProdOcoSetups = new HashSet<TargetProduto>();
            TarProdOcoSetupAs = new HashSet<TargetProduto>();
            TarProdOcoOpParciais = new HashSet<TargetProduto>();
            MovimentosEstoqueOpParcial = new HashSet<MovimentoEstoque>();
            MovimentosEstoque = new HashSet<MovimentoEstoque>();
        }
        public string Id { get; set; }
        public string Descricao { get; set; }
        public int TipoOcorrenciaId { get; set; }
        public int? Spr{ get; set; }
        public virtual TipoOcorrencia TipoOcorrencia { get; set; }
        public virtual ICollection<Feedback> Medicoes { get; set; }
        public virtual ICollection<MovimentoEstoque> MovimentosEstoque { get; set; }
        public virtual ICollection<MovimentoEstoque> MovimentosEstoqueOpParcial { get; set; }
        public virtual ICollection<TargetProduto> TarProdOcoPers { get; set; }
        public virtual ICollection<TargetProduto> TarProdOcoSetups { get; set; }
        public virtual ICollection<TargetProduto> TarProdOcoSetupAs { get; set; }
        public virtual ICollection<TargetProduto> TarProdOcoOpParciais { get; set; }
    }
}