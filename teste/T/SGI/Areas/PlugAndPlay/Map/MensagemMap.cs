using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class MensagemMap : EntityTypeConfiguration<Mensagem>
    {
        public MensagemMap()
        {
            ToTable("T_MENSAGENS");
            Property(x => x.MEN_ID).HasColumnName("MEN_ID").HasMaxLength(100);
            Property(x => x.MEN_MENSAGEM).HasColumnName("MEN_MENSAGEM").IsRequired().HasMaxLength(8000);
            Property(x => x.MEN_EMISSAO).HasColumnName("MEN_EMISSAO").IsRequired();
            HasKey(x => x.MEN_ID);
        }
    }
}