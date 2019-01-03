using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class TurmaMap : EntityTypeConfiguration<Turma>
    {
        public TurmaMap()
        {
            ToTable("T_TURMA");
            Property(x => x.Id).HasColumnName("TURM_ID").HasMaxLength(10);
            Property(x => x.Descricao).HasColumnName("TURM_DESCRICAO").IsRequired().HasMaxLength(100);

            HasKey(x => x.Id);
        }
    }
}