using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class RoteiroMap : EntityTypeConfiguration<Roteiro>
    {
        public RoteiroMap()
        {
            ToTable("T_ROTEIROS");
            //primarykey
            Property(x => x.ProdutoId).HasColumnName("PRO_ID").HasMaxLength(30);
            Property(x => x.MaquinaId).HasColumnName("MAQ_ID").HasMaxLength(30);
            Property(x => x.SequenciaTransformacao).HasColumnName("ROT_SEQ_TRANFORMACAO");
            HasKey(x => new { x.MaquinaId, x.ProdutoId, x.SequenciaTransformacao });
            //comuns
            Property(x => x.Padrao).HasColumnName("ROT_PADRAO").HasMaxLength(1);
            Property(x => x.ConsideraGrupoMaquina).HasColumnName("ROT_CONSIDERA_GRUPO_MAQUINAS").HasMaxLength(1);
            Property(x => x.PecasPorPulso).HasColumnName("ROT_PECAS_POR_PULSO");

            //forengn key
            HasRequired(x => x.Produto).WithMany(x => x.Roteiros).HasForeignKey(x => x.ProdutoId);
            HasRequired(x => x.Maquina).WithMany(x => x.Roteiros).HasForeignKey(x => x.MaquinaId);
        }
    }
}