using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class TipoOcorrenciaMap : EntityTypeConfiguration<TipoOcorrencia>
    {
        public TipoOcorrenciaMap()
        {
            ToTable("T_TIPO_OCORRENCIA");
            Property(x => x.Id).HasColumnName("TIP_ID");
            Property(x => x.Descricao).HasColumnName("TIP_DESCRICAO").IsRequired().HasMaxLength(100);
            Property(x => x.Spr).HasColumnName("SPR").IsOptional();
            HasKey(x => x.Id);
        }
    }
}