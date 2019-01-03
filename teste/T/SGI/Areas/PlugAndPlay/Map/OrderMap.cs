using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
            ToTable("T_ORDENS");
            HasKey(x => x.Id);
            //colunas comuns
            Property(x => x.Id).HasColumnName("ORD_ID").HasMaxLength(30).IsRequired();
            Property(x => x.DataEntregaDe).HasColumnName("ORD_DATA_ENTREGA_DE").IsRequired();
            Property(x => x.DataEntregaAte).HasColumnName("ORD_DATA_ENTREGA_ATE").IsRequired();
            Property(x => x.Tipo).HasColumnName("ORD_TIPO").HasMaxLength(2).IsRequired();
            Property(x => x.Quantidade).HasColumnName("ORD_QUANTIDADE").IsRequired();
            Property(x => x.PrecoUnitario).HasColumnName("ORD_PRECO_UNITARIO").IsRequired();
            Property(x => x.ToleranciaMais).HasColumnName("ORD_TOLERANCIA_MAIS").IsRequired();
            Property(x => x.ToleranciaMenos).HasColumnName("ORD_TOLERANCIA_MENOS").IsRequired();
            //chaves estrangeiras
            Property(x => x.ProdutoId).HasColumnName("PRO_ID").HasMaxLength(30).IsRequired();
            Property(x => x.ClienteId).HasColumnName("CLI_ID").HasMaxLength(50).IsRequired();

            HasRequired(x => x.Produto).WithMany(x => x.Ordens).HasForeignKey(x => x.ProdutoId);
            HasRequired(x => x.Cliente).WithMany(x => x.Ordens).HasForeignKey(x => x.ClienteId);
        }
    }
}