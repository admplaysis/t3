using SGI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class Turma
    {
        public Turma()
        {
            Calendarios = new HashSet<ItensCalendario>();
            Colaboradores = new HashSet<Colaborador>();
            Feedbacks = new HashSet<Feedback>();
            Usuarios = new HashSet<T_Usuario>();
            TargetsProduto = new HashSet<TargetProduto>();
        }
        [Required(ErrorMessage = "Prencimento Obrigatório.")]
        [MaxLength(10, ErrorMessage = "Permitido no máximo 10 caracteres.")]
        [Display(Name = "ID")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Prencimento Obrigatório.")]
        [MaxLength(100, ErrorMessage = "Permitido no máximo 100 caracteres.")]
        [Display(Name ="Descrição")]
        public string Descricao { get; set; }
        public virtual ICollection<ItensCalendario> Calendarios { get; set; }
        public virtual ICollection<Colaborador> Colaboradores { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<T_Usuario> Usuarios { get; set; }
        public virtual ICollection<TargetProduto> TargetsProduto { get; set; }
    }
}