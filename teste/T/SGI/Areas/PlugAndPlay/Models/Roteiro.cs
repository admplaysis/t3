using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class Roteiro
    {
        public Roteiro()
        {
            FilasProducao = new HashSet<FilaProducao>();
        }
        [Display(Name="Produto")][Required]
        public string ProdutoId { get; set; }
        [Display(Name = "Maquina")][Required]
        public string MaquinaId { get; set; }
        [Display(Name = "Sequencia de Transformação")][Required]
        public int SequenciaTransformacao { get; set; }
        [Display(Name = "Padrão")]
        public string Padrao { get; set; }
        [Display(Name = "Considerar Grupos de Máquina")]
        public string ConsideraGrupoMaquina { get; set; }
        [Display(Name = "Peças por Pulso")]
        public double PecasPorPulso { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual Maquina Maquina { get; set; }
        public virtual ICollection<FilaProducao> FilasProducao { get; set; }
    }
}