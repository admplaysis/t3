using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class Calendario
    {
        public Calendario()
        {
            IntensCalendario = new HashSet<ItensCalendario>();
            Maquinas = new HashSet<Maquina>();
        }
        public int  Id { get; set; }
        public string Descricao { get; set; }
        public virtual ICollection<ItensCalendario> IntensCalendario { get; set; }
        public virtual ICollection<Maquina> Maquinas { get; set; }
    }
}