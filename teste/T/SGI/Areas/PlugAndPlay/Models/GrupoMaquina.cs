using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class GrupoMaquina
    {
        public GrupoMaquina()
        {
            Maquinas = new HashSet<Maquina>();
        }
        public string Id { get; set; }
        public string Descricao { get; set; }
        public virtual ICollection<Maquina> Maquinas { get; set; }
    }
}