using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class Produto
    {
        public Produto()
        {
            EstruturasProdutoPai = new HashSet<EstruturaProduto>();
            EstruturasProdutoFilho = new HashSet<EstruturaProduto>();
            Roteiros = new HashSet<Roteiro>();
            TargetsProduto = new HashSet<TargetProduto>();
            MovimentosEstoque = new HashSet<MovimentoEstoque>();
            Ordens = new HashSet<Order>();
            Feedbacks = new HashSet<Feedback>();
            FilasProducao = new HashSet<FilaProducao>();
        }
        [Required(ErrorMessage = "Prencimento Obrigatório.")]
        [MaxLength(30, ErrorMessage = "Permitido no máximo 30 caracteres.")]
        [Display(Name = "Código")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Prencimento Obrigatório.")]
        [MaxLength(100, ErrorMessage = "Permitido no máximo 100 caracteres.")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        [Required(ErrorMessage = "Prencimento Obrigatório.")]
        [Display(Name = "Estoque")]
        [Range(0, float.MaxValue, ErrorMessage ="Informe um número válido.")]
        public float Estoque { get; set; }
        [Required(ErrorMessage = "Selecione a unidade de medida.")]
        [Display(Name = "Unidade de Medida")]
        public string UnidadeMedidaId { get; set; }
        public virtual UnidadeMedida UnidadeMedida { get; set; }
        [Required(ErrorMessage ="Indique a quantidade de produtos para compor um fardo deste produto.")]
        [Display(Name ="Quantidade por Fardo")]
        [Range(0, float.MaxValue, ErrorMessage = "Informe um número válido.")]
        public double? PecasPorFardo { get; set ; }
        [Required(ErrorMessage ="Indique a quantidade de fardos para compor um palet palet deste produto")]
        [Display(Name = "Fardos por Palet")]
        [Range(0, float.MaxValue, ErrorMessage = "Informe um número válido.")]
        public double? FardosPorPalet { get; set; }
        [Required(ErrorMessage ="Indique o tipo de Identificador para este produto 1..4 ")]
        [Display(Name = "Tipo Identificador : 1 - IDENTIFICADOR_PECA, 2-  IDENTIFICADOR_FARDO, 3-  IDENTIFICADOR_CAIXA , 4-  IDENTIFICADOR_PALET")]
        [Range(1,4,ErrorMessage = "Valores para identificador  1 - IDENTIFICADOR_PECA, 2-  IDENTIFICADOR_FARDO, 3-  IDENTIFICADOR_CAIXA , 4-  IDENTIFICADOR_PALET")]
        public int TPIdentificador { get; set; }  // define se o produto possue metodo de identificação 
        public virtual ICollection<EstruturaProduto> EstruturasProdutoPai { get; set; }
        public virtual ICollection<EstruturaProduto> EstruturasProdutoFilho { get; set; }
        public virtual ICollection<Roteiro> Roteiros { get; set; }
        public virtual ICollection<TargetProduto> TargetsProduto { get; set; }
        public virtual ICollection<MovimentoEstoque> MovimentosEstoque { get; set; }
        public virtual ICollection<Order> Ordens { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<FilaProducao> FilasProducao { get; set; }
    }
}