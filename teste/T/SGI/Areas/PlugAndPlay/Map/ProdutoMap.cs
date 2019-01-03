using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class ProdutoMap : EntityTypeConfiguration<Produto>
    {
        public ProdutoMap()
        {
            ToTable("T_PRODUTOS");
            HasKey(x => x.Id);
            Property(x => x.Id).HasColumnName("PRO_ID").HasMaxLength(30).IsRequired();
            Property(x => x.Descricao).HasColumnName("PRO_DESCRICAO").HasMaxLength(100).IsRequired();
            Property(x => x.Estoque).HasColumnName("PRO_ESTOQUE_ATUAL").IsRequired();
            Property(x => x.Descricao).HasColumnName("PRO_DESCRICAO").HasMaxLength(100).IsRequired();
            Property(x => x.UnidadeMedidaId).HasColumnName("UNI_ID").HasMaxLength(10).IsRequired();
            HasRequired(x => x.UnidadeMedida).WithMany(x => x.Produtos).HasForeignKey(x => x.UnidadeMedidaId);
            Property(x => x.PecasPorFardo).HasColumnName("PRO_PECAS_POR_FARDO").IsRequired();
            Property(x => x.FardosPorPalet).HasColumnName("PRO_FARDOS_POR_PALET").IsRequired();
            Property(x => x.TPIdentificador).HasColumnName("PRO_TIPO_IDENTIFICACAO").IsRequired();
            
        }
    }
}