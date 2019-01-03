using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class ItensCalendarioMap : EntityTypeConfiguration<ItensCalendario>
    {
        public ItensCalendarioMap()
        {
            ToTable("T_ITENS_CALENDARIO");
            Property(x => x.Id).HasColumnName("ICA_ID");
            Property(x => x.DataDe).HasColumnName("ICA_DATA_DE").IsRequired();
            Property(x => x.DataAte).HasColumnName("ICA_DATA_ATE").IsRequired();
            Property(x => x.Observacao).HasColumnName("ICA_OBSERVACAO").IsOptional().HasMaxLength(200);
            Property(x => x.Tipo).HasColumnName("ICA_TIPO").IsOptional();
            Property(x => x.TurmaId).HasColumnName("URM_ID").IsRequired().HasMaxLength(10);
            Property(x => x.TurnoId).HasColumnName("URN_ID").IsRequired().HasMaxLength(10);
            //Property(x => x.CalendarioId).HasColumnName("CAL_ID").IsRequired();
            Property(x => x.CalendarioId).HasColumnName("CAL_ID");

            HasKey(x => x.Id);
            HasRequired(x => x.Turno).WithMany(x => x.Calendarios).HasForeignKey(x => x.TurnoId);
            HasRequired(x => x.Turma).WithMany(x => x.Calendarios).HasForeignKey(x => x.TurmaId);
            HasRequired(x => x.Calendario).WithMany(x => x.IntensCalendario).HasForeignKey(x => x.CalendarioId);
        }
    }
}