    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class Turno
    {
        public Turno()
        {
            Medicoes = new HashSet<Feedback>();
            Calendarios = new HashSet<ItensCalendario>();
            TargetsProduto = new HashSet<TargetProduto>();
        }

        public string Id { get; set; }
        public string Descricao { get; set; }
        public virtual ICollection<Feedback> Medicoes { get; set; }
        public virtual ICollection<ItensCalendario> Calendarios { get; set; }
        public virtual ICollection<TargetProduto> TargetsProduto { get; set; }
    }
}