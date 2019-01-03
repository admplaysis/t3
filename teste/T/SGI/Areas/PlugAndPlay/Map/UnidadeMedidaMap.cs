using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class UnidadeMedidaMap : EntityTypeConfiguration<UnidadeMedida>
    {
        public UnidadeMedidaMap()
        {
            ToTable("T_UNIDADE_MEDIDA");
            Property(x => x.Id).HasColumnName("UNI_ID").HasMaxLength(10);
            Property(x => x.Descricao).HasColumnName("UNI_DESCRICAO").IsRequired().HasMaxLength(100);
            Property(x => x.EscalaTempo).HasColumnName("UNI_ESCALA_TEMPO").IsRequired().HasMaxLength(1);
            HasKey(x => x.Id);
        }
    }
}