using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class Cliente
    {
        public Cliente()
        {
            Ordens = new HashSet<Order>();
        }
        [Display(Name ="ID")][Required]
        public string Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Display(Name = "Telefone")][Required]
        public string Fone { get; set; }
        [Display(Name = "Observacão")]
        public string Observacao{ get; set; }
        [Display(Name = "Endereço")][Required]
        public string Endereco { get; set; }
        [Display(Name = "CPF/CNPJ")][Required]
        public string CpfCnpj { get; set; }
        [Display(Name = "Tempo para entrega")]
        [Required]
        public DateTime TempoParaEntrega { get; set; }
        public ICollection<Order> Ordens { get; set; }
    }
}