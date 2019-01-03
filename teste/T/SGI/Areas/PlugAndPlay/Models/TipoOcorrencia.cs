using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class TipoOcorrencia
    {
        public TipoOcorrencia()
        {
            Ocorrencias = new HashSet<Ocorrencia>();
        }
        [Required]
        public int Id { get; set; }
        [Required]
        public string Descricao { get; set; }
        public int? Spr { get; set; }
        public virtual ICollection<Ocorrencia> Ocorrencias { get; set; }
    }
}