using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class ViewFilaProducaoMap : EntityTypeConfiguration<ViewFilaProducao>
    {
        public ViewFilaProducaoMap()
        {
            ToTable("V_FILA_PRODUCAO");
            //cliente
            Property(x => x.CliId).HasColumnName("CLI_ID").HasMaxLength(30).IsRequired();
            Property(x => x.CliNome).HasColumnName("CLI_NOME").HasMaxLength(100).IsRequired();
            Property(x => x.CliFone).HasColumnName("CLI_FONE").HasMaxLength(100).IsRequired();
            Property(x => x.CliObs).HasColumnName("CLI_OBS").IsOptional();
            //produto
//            Property(x => x.RotPcProId).HasColumnName("ROT_PC_PRO_ID").HasMaxLength(30).IsRequired();
            Property(x => x.PcProId).HasColumnName("PC_PRO_ID").HasMaxLength(30).IsRequired();

            Property(x => x.PcProDescricao).HasColumnName("PC_PRO_DESCRICAO").HasMaxLength(100).IsRequired();
            Property(x => x.PcUniId).HasColumnName("PC_UNI_ID").HasMaxLength(30).IsRequired();
            //maquina
            Property(x => x.RotMaqId).HasColumnName("ROT_MAQ_ID").HasMaxLength(30).IsRequired();
            Property(x => x.MaqDescricao).HasColumnName("MAQ_DESCRICAO").HasMaxLength(100).IsRequired();
            //order
            Property(x => x.OrdId).HasColumnName("ORD_ID").HasMaxLength(30).IsRequired();
            Property(x => x.PaProId).HasColumnName("PA_PRO_ID").HasMaxLength(30).IsRequired();
            Property(x => x.PaProDescricao).HasColumnName("PA_PRO_DESCRICAO").HasMaxLength(100).IsRequired();
            Property(x => x.PaUniId).HasColumnName("PA_UNI_ID").HasMaxLength(30).IsOptional();
            Property(x => x.OrdPrecoUnitario).HasColumnName("ORD_PRECO_UNITARIO").IsOptional();
            Property(x => x.OrdQuantidade).HasColumnName("ORD_QUANTIDADE").IsRequired();
            Property(x => x.OrdDataEntregaDe).HasColumnName("ORD_DATA_ENTREGA_DE").IsRequired();
            Property(x => x.OrdDataEntregaAte).HasColumnName("ORD_DATA_ENTREGA_ATE").IsRequired();
            Property(x => x.OrdTipo).HasColumnName("ORD_TIPO").HasMaxLength(1).IsOptional();
            Property(x => x.OrdToleranciaMais).HasColumnName("ORD_TOLERANCIA_MAIS").IsOptional();
            Property(x => x.OrdToleranciaMenos).HasColumnName("ORD_TOLERANCIA_MENOS").IsOptional();
            //fila
            Property(x => x.FprDataInicioPrevista).HasColumnName("FPR_DATA_INICIO_PREVISTA").IsRequired();
            Property(x => x.FprDataFimPrevista).HasColumnName("FPR_DATA_FIM_PREVISTA").IsRequired();
            Property(x => x.FprDataFimMaxima).HasColumnName("FPR_DATA_FIM_MAXIMA").IsRequired();
            Property(x => x.FprQuantidadePrevista).HasColumnName("FPR_QUANTIDADE_PREVISTA").IsRequired();
            Property(x => x.RotSeqTransformacao).HasColumnName("ROT_SEQ_TRANFORMACAO").IsRequired();
            Property(x => x.FprSeqRepeticao).HasColumnName("FPR_SEQ_REPETICAO").IsRequired();
            Property(x => x.FprObsProducao).HasColumnName("FPR_OBS_PRODUCAO").IsOptional();
            Property(x => x.FprStatus).HasColumnName("FPR_STATUS").HasMaxLength(2).IsOptional();
            Property(x => x.Produzindo).HasColumnName("FPR_PRODUZINDO").IsOptional();

            //roteiro
            Property(x => x.RotQuantPecasPulso).HasColumnName("ROT_PECAS_POR_PULSO").IsOptional();
            //medicoes variaveis
            Property(x => x.FprTempoDecorridoSetup).HasColumnName("FPR_TEMPO_DECORRIDO_SETUP").IsOptional();
            Property(x => x.FprTempoDecorridoSetupAjuste).HasColumnName("FPR_TEMPO_DECORRIDO_SETUPA").IsOptional();
            Property(x => x.FprTempoDecorridoPerformacace).HasColumnName("FPR_TEMPO_DECORRIDO_PERFORMANC").IsOptional();
            Property(x => x.FprQuantidadePerformace).HasColumnName("FPR_QTD_PERFORMANCE").IsOptional();
            Property(x => x.FprQuantidadeSetup).HasColumnName("FPR_QTD_SETUP").IsOptional();
            Property(x => x.FprQuantidadeProduzida).HasColumnName("FPR_QTD_PRODUZIDA").IsOptional();
            Property(x => x.FprTempoTeoricoPerformace).HasColumnName("FPR_TEMPO_TEORICO_PERFORMANCE").IsOptional();
            Property(x => x.FprTempoRestantePerformace).HasColumnName("FPR_TEMPO_RESTANTE_PERFORMANC").IsOptional();
            Property(x => x.FprVelocidadeAtingirMeta).HasColumnName("FPR_VELOCIDADE_P_ATINGIR_META").IsOptional();
            Property(x => x.FprQuantidadeRestante).HasColumnName("FPR_QTD_RESTANTE").IsOptional();
            Property(x => x.FprVeloAtuPcSegundo).HasColumnName("FPR_VELO_ATU_PC_SEGUNDO").IsOptional(); 
            Property(x => x.FprPerformaceProjetada).HasColumnName("FPR_PERFORMANCE_PROJETADA").IsOptional();
            Property(x => x.TempoDecorridoPequenasParadas).HasColumnName("FPR_TEMPO_DECO_PEQUENA_PARADA").IsOptional();
            
            
            //primary key
            HasKey(x => new { x.RotMaqId, x.OrdId, x.PaProId, x.FprSeqRepeticao, x.RotSeqTransformacao });
        }
    }
}