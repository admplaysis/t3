using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class MaquinaMap : EntityTypeConfiguration<Maquina>
    {
        public MaquinaMap()
        {
            ToTable("T_MAQUINA");
            Property(x => x.Id).HasColumnName("MAQ_ID").HasMaxLength(30);
            Property(x => x.Descricao).HasColumnName("MAQ_DESCRICAO").IsRequired().HasMaxLength(100);
            Property(x => x.CalendarioId).HasColumnName("CAL_ID").IsRequired();
            Property(x => x.ControlIp).HasColumnName("MAQ_CONTROL_IP").IsOptional();
            Property(x => x.GrupoMaquinaId).HasColumnName("GMA_ID").IsRequired();
            Property(x => x.Sirene).HasColumnName("MAQ_SIRENE_SEMAFORO");
            Property(x => x.CorSemafaro).HasColumnName("MAQ_COR_SEMAFORO");
            Property(x => x.TipoContador).HasColumnName("MAQ_TIPO_CONTADOR").IsRequired();
            Property(x => x.MaqIdMaqPai).HasColumnName("MAQ_ID_MAQ_PAI");
            HasKey(x => x.Id);
            HasRequired(x => x.Calendario).WithMany(x => x.Maquinas).HasForeignKey(x => x.CalendarioId);
            HasRequired(x => x.GrupoMaquina).WithMany(x => x.Maquinas).HasForeignKey(x => x.GrupoMaquinaId);
        }
    }
}