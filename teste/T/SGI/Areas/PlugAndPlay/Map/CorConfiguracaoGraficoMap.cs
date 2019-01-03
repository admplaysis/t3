using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class CorConfiguracaoGraficoMap : EntityTypeConfiguration<CorConfiguracaoGrafico>
    {
        public CorConfiguracaoGraficoMap()
        {
            ToTable("T_CORES_CONFIGURACAO_GRAFICO");
            Property(x => x.Id).HasColumnName("COR_ID").HasMaxLength(2);
            Property(x => x.PercentualIni).HasColumnName("COR_PERCENTUAL_INI");
            Property(x => x.PercentualFim).HasColumnName("COR_PERCENTUAL_FIM");
            Property(x => x.Descricao).HasColumnName("COR_DESCRICAO").HasMaxLength(30);

            HasKey(x=>x.Id);
        }
    }
}