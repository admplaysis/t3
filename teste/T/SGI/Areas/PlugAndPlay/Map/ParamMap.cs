using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class ParamMap : EntityTypeConfiguration<Param>
    {
        public ParamMap()
        {
            ToTable("T_PARAMETROS");
            Property(x => x.PAR_ID).HasColumnName("PAR_ID").HasMaxLength(100);
            Property(x => x.PAR_DESCRICAO).HasColumnName("PAR_DESCRICAO").IsRequired().HasMaxLength(100);
            Property(x => x.PAR_VALOR_S).HasColumnName("PAR_VALOR_S").IsRequired().HasMaxLength(100);
            Property(x => x.PAR_VALOR_N).HasColumnName("PAR_VALOR_N").IsRequired();
            HasKey(x => x.PAR_ID);
        }
    }
}