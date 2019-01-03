using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class CalendarioMap : EntityTypeConfiguration<Calendario>
    {
        public CalendarioMap()
        {
            ToTable("T_CALENDARIO");
            Property(X => X.Id).HasColumnName("CAL_ID");
            Property(X => X.Descricao).HasColumnName("CAL_DESCRICAO").HasMaxLength(100);

            HasKey(X => X.Id);
        }
    }
}