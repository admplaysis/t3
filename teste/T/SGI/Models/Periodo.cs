using SGI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Models
{
    public class Periodo
    {
        public string Id { get; set; }
        public string Descricao { get; set; }
        public int IndicadorId { get; set; }
        public int  DimensaoId { get; set; }
        public virtual T_Indicadores Indicador{ get; set; }
    }
    public class IndicadorPeriodoDimensaoMap: EntityTypeConfiguration<Periodo>
    {
        public IndicadorPeriodoDimensaoMap()
        {
            ToTable("T_INDICADORES_PERIODOS_DIMENCOES");
            Property(x => x.Id).HasColumnName("PER_ID").HasMaxLength(10).IsRequired();
            Property(x => x.Descricao).HasColumnName("PER_DESCRICAO").HasMaxLength(100).IsRequired();
            Property(x => x.IndicadorId).HasColumnName("IND_ID").IsRequired();
            Property(x => x.DimensaoId).HasColumnName("DIM_ID").IsRequired();

            HasKey(x=>new {x.Id, x.IndicadorId, x.DimensaoId});
        }
    }
}