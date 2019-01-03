using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class ClpMedicoesMap : EntityTypeConfiguration<ClpMedicoes>
    {
        public ClpMedicoesMap()
        {
            ToTable("T_CLP_MEDICOES");
            Property(x => x.Id).HasColumnName("ID");
            Property(x => x.MaquinaId).HasColumnName("MAQUINA_ID").HasMaxLength(10);
            Property(x => x.DataInicio).HasColumnName("DATA_INI");
            Property(x => x.DataFim).HasColumnName("DATA_FIM");
            Property(x => x.Quantidade).HasColumnName("QTD");
            Property(x => x.Grupo).HasColumnName("GRUPO").IsOptional();
            Property(x => x.Status).HasColumnName("STATUS").IsOptional();
            Property(x => x.TurmaId).HasColumnName("URM_ID").HasMaxLength(1);
            Property(x => x.TurnoId).HasColumnName("URN_ID").HasMaxLength(1);
            //Property(x => x.OrdemProducaoId).HasColumnName("ORD_ID").HasMaxLength(30);
            Property(x => x.OcorrenciaId).HasColumnName("OCO_ID").HasMaxLength(30);
            Property(x => x.IdLoteClp).HasColumnName("ID_LOTE_CLP");
            Property(x => x.Fase).HasColumnName("FASE").IsOptional();
            Property(x => x.Emissao).HasColumnName("CLP_EMISSAO").IsOptional();
            //Property(x => x.ProdutoId).HasColumnName("PRO_ID").HasMaxLength(30).IsOptional();
            //Property(x => x.SequenciaTransformacaoId).HasColumnName("ROT_SEQ_TRANFORMACAO").IsOptional();
            //Property(x => x.SequenciaRepeticaoId).HasColumnName("FPR_SEQ_REPETICAO").IsOptional();
            //Property(x => x.FilaProducaoId).HasColumnName("FPR_ID").IsOptional();
            Property(x => x.ClpOrigem).HasColumnName("CLP_ORIGEM").HasMaxLength(1).IsOptional();

            HasKey(x => x.Id);
        }
    }
}