using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class EstruturaProdutoMap : EntityTypeConfiguration<EstruturaProduto>
    {
        public EstruturaProdutoMap()
        {
            ToTable("T_ESTRUTURA_PRODUTO");
            Property(x => x.DataValidade).HasColumnName("EST_DATA_VALIDADE");
            Property(x => x.ProdutoPaiId).HasColumnName("PRO_ID_PAI").HasMaxLength(30);
            Property(x => x.ProdutoFilhoId).HasColumnName("PRO_ID_FILHO").HasMaxLength(30);
            Property(x => x.Quantidade).HasColumnName("EST_QUANT").IsRequired();
            Property(x => x.DataInclusao).HasColumnName("EST_DATA_INCLUSAO").IsRequired();
            Property(x => x.BaseProducao).HasColumnName("EST_BASE_PRODUCAO").IsRequired();
            Property(x => x.TipoRequisicao).HasColumnName("EST_TIPO_REQUISICAO").IsRequired();

            HasKey(x => new { x.DataValidade, x.ProdutoPaiId, x.ProdutoFilhoId});

            HasRequired(x => x.ProdutoPai).WithMany(x => x.EstruturasProdutoPai).HasForeignKey(x => x.ProdutoPaiId);
            HasRequired(x => x.ProdutoFilho).WithMany(p => p.EstruturasProdutoFilho).HasForeignKey(x => x.ProdutoFilhoId);
        }
    }
}