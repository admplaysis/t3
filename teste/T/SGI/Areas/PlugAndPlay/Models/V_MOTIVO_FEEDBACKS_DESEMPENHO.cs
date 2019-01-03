using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class V_MOTIVO_FEEDBACKS_DESEMPENHO
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(28)]
        public string TIPO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(8)]
        public string TAR_DIA_TURMA { get; set; }

        [StringLength(30)]
        public string OCO_ID_PERFORMANCE { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(100)]
        public string OCO_DESCRICAO { get; set; }

        [StringLength(200)]
        public string TAR_OBS_PERFORMANCE { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(80)]
        public string NOME { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(30)]
        public string MAQ_ID { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(30)]
        public string ORD_ID { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(30)]
        public string PRO_ID { get; set; }

        public int? FPR_SEQ_REPETICAO { get; set; }

        public int? ROT_SEQ_TRANFORMACAO { get; set; }

        [StringLength(10)]
        public string TURM_ID { get; set; }

        [StringLength(10)]
        public string TURN_ID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? TAR_DIA_TURMA_D { get; set; }

        public string TIPO_ID { get; set; }
    }
}