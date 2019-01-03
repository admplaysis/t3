using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class TipoMovimentoEstoqueMap :EntityTypeConfiguration<TipoMovimentoEstoque>
    {
        public TipoMovimentoEstoqueMap()
        {
            ToTable("T_TIPO_MOV_ESTOQUE");
            Property(x => x.Id).HasColumnName("TIP_ID").HasMaxLength(3);
            Property(x => x.Descricao).HasColumnName("TIP_DESCRICAO").HasMaxLength(100).IsRequired();
            Property(x => x.SPR).HasColumnName("SPR").IsRequired();
            Property(x => x.SPR).HasColumnName("FPR_SEQ_TRANFORMACAO").IsRequired();
            Property(x => x.SPR).HasColumnName("FPR_SEQ_REPETICAO").IsRequired();

            HasKey(x => x.Id); 
        }
    }
}