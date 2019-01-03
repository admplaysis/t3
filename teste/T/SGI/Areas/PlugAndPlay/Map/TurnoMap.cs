using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class TurnoMap : EntityTypeConfiguration<Turno>
    {
        public TurnoMap()
        {
            ToTable("T_TURNO");
            Property(x => x.Id).HasColumnName("TURN_ID").HasMaxLength(10);
            Property(x => x.Descricao).HasColumnName("TURN_DESCRICAO").IsRequired().HasMaxLength(100);

            HasKey(x => x.Id);
        }
    }
}