using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class GrupoMaquinaMap :EntityTypeConfiguration<GrupoMaquina>
    {
        public GrupoMaquinaMap()
        {
            ToTable("T_GRUPO_MAQUINA");
            Property(x => x.Id).HasColumnName("GMA_ID").HasMaxLength(30);
            Property(x => x.Descricao).HasColumnName("GMA_DESCRICAO").HasMaxLength(100);
            HasKey(x => x.Id);
        }
    }
}