using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class UnidadeMedida
    {
        public UnidadeMedida()
        {
            Produtos = new HashSet<Produto>();
            TargetsProduto = new HashSet<TargetProduto>();
        }
        public string Id { get; set; }
        [Required(ErrorMessage = "Prencimento Obrigatório.")]
        [MaxLength(100, ErrorMessage = "Permitido no máximo 100 caracteres.")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        public string EscalaTempo { get; set; }//hora, minuto ou segundo : H, M, S
        public virtual ICollection<Produto> Produtos { get; set; }
        public virtual ICollection<TargetProduto> TargetsProduto { get; set; }
    }
}