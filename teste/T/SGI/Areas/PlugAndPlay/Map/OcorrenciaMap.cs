using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class OcorrenciaMap : EntityTypeConfiguration<Ocorrencia>
    {
        public OcorrenciaMap()
        {
            ToTable("T_OCORRENCIAS");
            Property(x => x.Id).HasColumnName("OCO_ID").HasMaxLength(10);
            Property(x => x.Descricao).HasColumnName("OCO_DESCRICAO").IsRequired().HasMaxLength(100);
            Property(x => x.TipoOcorrenciaId).HasColumnName("TIP_ID");
            Property(x => x.Spr).HasColumnName("SPR").IsOptional();

            HasKey(x => x.Id);
            HasRequired(x => x.TipoOcorrencia).WithMany(x => x.Ocorrencias).HasForeignKey(x => x.TipoOcorrenciaId);
        }
    }
}