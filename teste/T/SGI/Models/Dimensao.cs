using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Models
{
    public class Dimensao
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public int IndicadorId { get; set; }
        public virtual T_Indicadores Indicador { get; set; }
        public virtual ICollection<Fato> Fatos { get; set; }
        public virtual ICollection<Periodo> Periodos { get; set; }
        public virtual ICollection<SubDimensao> SubDimensao { get; set; }
    }
    public class SubDimensao
    {
        public string Id { get; set; }
        public string Descricao { get; set; }
        public int IndicadorId { get; set; }
        public virtual T_Indicadores Indicador { get; set; }
    }

    public class IndicadorDimencao
    {
        public int DIM_ID { get; set; }
        public int IND_ID { get; set; }
        public string DIM_DESCRICAO { get; set; }
        public string DIM_SQL { get; set; }
        public string DIM_CONEXAO { get; set; }
    }

    public class IndicadorDimensaoMap : EntityTypeConfiguration<Dimensao>
    {
        public IndicadorDimensaoMap()
        {
            ToTable("T_INDICADORES_DIMENCOES");
            Property(x => x.Id).HasColumnName("DIM_ID").IsRequired();
            Property(x => x.Descricao).HasColumnName("DIM_DESCRICAO").HasMaxLength(100).IsRequired();
            Property(x => x.IndicadorId).HasColumnName("IND_ID").IsRequired();
            HasKey(x => new { x.Id, x.IndicadorId });
        }
    }
}