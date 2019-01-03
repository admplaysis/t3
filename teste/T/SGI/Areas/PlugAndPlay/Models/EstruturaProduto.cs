using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class EstruturaProduto
    {
        public DateTime DataValidade { get; set; }
        public float Quantidade { get; set; }
        public DateTime DataInclusao { get; set; }
        public float BaseProducao { get; set; }
        public string ProdutoPaiId { get; set; }
        public string ProdutoFilhoId { get; set; }
        public string TipoRequisicao { get; set; }//RE: Requisição, RA: Rateio
        public virtual Produto ProdutoPai { get; set; }
        public virtual Produto ProdutoFilho { get; set; }
    }
}