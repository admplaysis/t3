using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Models
{
    public class Fato
    {
        public string Id { get; set; }
        public string Descricao { get; set; }
        public int IndicadorId{ get; set; }
        public int DimensaoId { get; set; }
        public virtual T_Indicadores Indicador{ get; set; }
        public virtual ICollection<Periodo> Periodos { get; set; }
    }
    public class IndicadorFatoDimensaoMap : EntityTypeConfiguration<Fato>
    {
        public IndicadorFatoDimensaoMap()
        {
            ToTable("T_INDICADORES_FATOS_DIMENCOES");
            Property(x => x.Id).HasColumnName("FAT_ID").HasMaxLength(10).IsRequired();
            Property(x => x.Descricao).HasColumnName("FAT_DESCRICAO").HasMaxLength(100).IsRequired();
            Property(x => x.IndicadorId).HasColumnName("IND_ID").IsRequired();
            Property(x => x.DimensaoId).HasColumnName("DIM_ID").IsRequired();

            HasKey(x => new { x.Id, x.IndicadorId, x.DimensaoId });
        }
    }
}