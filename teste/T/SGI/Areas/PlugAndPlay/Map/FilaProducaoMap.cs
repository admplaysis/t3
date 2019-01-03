using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class FilaProducaoMap : EntityTypeConfiguration<FilaProducao>
    {
        public FilaProducaoMap()
        {
            ToTable("T_FILA_PRODUCAO");
            Property(x => x.DataInicioPrevista).HasColumnName("FPR_DATA_INICIO_PREVISTA");
            Property(x => x.QuantidadePrevista).HasColumnName("FPR_QUANTIDADE_PREVISTA");
            Property(x => x.DataFimPrevista).HasColumnName("FPR_DATA_FIM_PREVISTA");
            Property(x => x.DataFimMaxima).HasColumnName("FPR_DATA_FIM_MAXIMA");
            Property(x => x.OrderId).HasColumnName("ORD_ID");
            Property(x => x.Status).HasColumnName("FPR_STATUS").HasMaxLength(2).IsRequired();
            //Roteiro
            Property(x => x.ProdutoId).HasColumnName("ROT_PRO_ID");
            Property(x => x.MaquinaId).HasColumnName("ROT_MAQ_ID");
            Property(x => x.SequenciaTransformacao).HasColumnName("ROT_SEQ_TRANFORMACAO");
            HasRequired(x => x.Roteiro).WithMany(x => x.FilasProducao).HasForeignKey(x => new { x.MaquinaId, x.ProdutoId, x.SequenciaTransformacao });
            //medicoes variaveis
            Property(x => x.TempoDecorridoSetup).HasColumnName("FPR_TEMPO_DECORRIDO_SETUP").IsOptional();
            Property(x => x.TempoDecorridoSetupAjuste).HasColumnName("FPR_TEMPO_DECORRIDO_SETUPA").IsOptional();
            Property(x => x.TempoDecorridoPerformacace).HasColumnName("FPR_TEMPO_DECORRIDO_PERFORMAN").IsOptional();
            Property(x => x.QuantidadePerformace).HasColumnName("FPR_QTD_PERFORMANCE").IsOptional();
            Property(x => x.QuantidadeSetup).HasColumnName("FPR_QTD_SETUP").IsOptional();
            Property(x => x.QuantidadeProduzida).HasColumnName("FPR_QTD_PRODUZIDA").IsOptional();
            Property(x => x.TempoTeoricoPerformace).HasColumnName("FPR_TEMPO_TEORICO_PERFORMANCE").IsOptional();
            Property(x => x.TempoRestantePerformace).HasColumnName("FPR_TEMPO_RESTANTE_PERFORMANC").IsOptional();
            Property(x => x.VelocidadeAtingirMeta).HasColumnName("FPR_VELOCIDADE_P_ATINGIR_META").IsOptional();
            Property(x => x.QuantidadeRestante).HasColumnName("FPR_QTD_RESTANTE").IsOptional();
            Property(x => x.VeloAtuPcSegundo).HasColumnName("FPR_VELO_ATU_PC_SEGUNDO").IsOptional();
            Property(x => x.PerformaceProjetada).HasColumnName("FPR_PERFORMANCE_PROJETADA").IsOptional();
            Property(x => x.TempoDecorridoPequenasParadas).HasColumnName("FPR_TEMPO_DECO_PEQUENA_PARADA").IsOptional();
            Property(x => x.Produzindo).HasColumnName("FPR_PRODUZINDO").IsOptional();
            



            HasKey(x => new {x.MaquinaId, x.OrderId , x.ProdutoId, x.SequenciaRepeticao, x.SequenciaTransformacao});
            Property(x => x.SequenciaRepeticao).HasColumnName("FPR_SEQ_REPETICAO");
            Property(x => x.ObservacaoProducao).HasColumnName("FPR_OBS_PRODUCAO");


            HasRequired(x => x.Order).WithMany(x => x.FilasProducao).HasForeignKey(x => x.OrderId);
            HasRequired(x => x.Produto).WithMany(x => x.FilasProducao).HasForeignKey(x => x.ProdutoId);

        }
    }
}