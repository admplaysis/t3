using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class TargetProdutoMap : EntityTypeConfiguration<TargetProduto>
    {
        public TargetProdutoMap()
        {
            ToTable("T_TARGET_PRODUTO");
            Property(x => x.Id).HasColumnName("TAR_ID");
            Property(x => x.DiaTurma).HasColumnName("TAR_DIA_TURMA").HasMaxLength(8);
            Property(x => x.MetaPerformace).HasColumnName("TAR_META_PERFORMANCE").IsOptional();
            Property(x => x.RealizadoPerformace).HasColumnName("TAR_REALIZADO_PERFORMANCE").IsOptional();
            Property(x => x.ProximaMetaPerformace).HasColumnName("TAR_PROXIMA_META_PERFORMANCE").IsOptional();
            Property(x => x.MetaTempoSetup).HasColumnName("TAR_META_TEMPO_SETUP").IsOptional();
            Property(x => x.RealizadoTempoSetup).HasColumnName("TAR_REALIZADO_TEMPO_SETUP").IsOptional();
            Property(x => x.ProximaMetaTempoSetup).HasColumnName("TAR_PROXIMA_META_TEMPO_SETUP").IsOptional();
            Property(x => x.MetaTempoSetupAjuste).HasColumnName("TAR_META_TEMPO_SETUP_AJUSTE").IsOptional();
            Property(x => x.RealizadoTempoSetupAjuste).HasColumnName("TAR_REALIZADO_TEMPO_SETUP_AJUSTE").IsOptional();
            Property(x => x.ProximaMetaTempoSetupAjuste).HasColumnName("TAR_PROXIMA_META_TEMPO_SETUP_AJUSTE").IsOptional();
            Property(x => x.DiaTurmaD).HasColumnName("TAR_DIA_TURMA_D").IsOptional();
            //observações
            Property(x => x.ObsPerformace).HasColumnName("TAR_OBS_PERFORMANCE").HasMaxLength(200).IsOptional();
            Property(x => x.ObsSetup).HasColumnName("TAR_OBS_SETUP").HasMaxLength(200).IsOptional();
            Property(x => x.ObsSetupAjuste).HasColumnName("TAR_OBS_SETUPA").HasMaxLength(200).IsOptional();
            Property(x => x.ObsOpParcial).HasColumnName("TAR_OBS_OP_PARCIAL").HasMaxLength(200).IsOptional();
            //tipos de feedbacks
            Property(x => x.TipoFeedbackPerformace).HasColumnName("TAR_TIPO_FEEDBACK_PERFORMANCE").HasMaxLength(1).IsOptional();
            Property(x => x.TipoFeedbackSetup).HasColumnName("TAR_TIPO_FEEDBACK_SETUP").HasMaxLength(1).IsOptional();
            Property(x => x.TipoFeedbackSetupAjuste).HasColumnName("TAR_TIPO_FEEDBACK_SETUP_AJUSTE").HasMaxLength(1).IsOptional();
            //outros
            Property(x => x.Quantidade).HasColumnName("TAR_QTD").IsOptional();
            Property(x => x.ParametroTimeWorkStopMachine).HasColumnName("TAR_PARAMETRO_TIME_WORK_STOP_MACHINE").IsOptional();
            Property(x => x.ParametroTempoQuebraLote).HasColumnName("TAR_PARAMETRO_TEMPO_QUEBRA_DE_LOTE").IsOptional();
            //parametros para as cores
            Property(x => x.PerformaceMaxVerde).HasColumnName("TAR_PERFORMANCE_MAX_VERDE").IsOptional();
            Property(x => x.PerformaceMinVerde).HasColumnName("TAR_PERFORMANCE_MIN_VERDE").IsOptional();
            Property(x => x.SetupMaxVerde).HasColumnName("TAR_SETUP_MAX_VERDE").IsOptional();
            Property(x => x.SetupMinVerde).HasColumnName("TAR_SETUP_MIN_VERDE").IsOptional();
            Property(x => x.SetupAjusteMaxVerde).HasColumnName("TAR_SETUPA_MAX_VERDE").IsOptional();
            Property(x => x.SetupAjusteMinVerde).HasColumnName("TAR_SETUPA_MIN_VERDE").IsOptional();
            Property(x => x.PerformaceMinAmarelo).HasColumnName("TAR_PERFORMANCE_MIN_AMARELO").IsOptional();
            Property(x => x.SetupMaxAmarelo).HasColumnName("TAR_SETUP_MAX_AMARELO").IsOptional();
            Property(x => x.SetupAjusteMaxAmarelo).HasColumnName("TAR_SETUPA_MAX_AMARELO").IsOptional();

            //id chaves estrangeiras
            Property(x => x.OcoIdPerformace).HasColumnName("OCO_ID_PERFORMANCE").HasMaxLength(30).IsOptional();
            Property(x => x.OcoIdSetup).HasColumnName("OCO_ID_SETUP").HasMaxLength(30).IsOptional();
            Property(x => x.OcoIdSetupAjuste).HasColumnName("OCO_ID_SETUPA").HasMaxLength(30).IsOptional();
            Property(x => x.OcoIdOpParcial).HasColumnName("TAR_OCO_ID_OP_PARCIAL").HasMaxLength(30).IsOptional();
            Property(x => x.UsuarioId).HasColumnName("USU_ID").IsOptional();

            Property(x => x.MovimentoEstoqueId).HasColumnName("MOV_ID").IsOptional();
            Property(x => x.OrderId).HasColumnName("ORD_ID").HasMaxLength(30).IsOptional();
            Property(x => x.ProdutoId).HasColumnName("PRO_ID").HasMaxLength(30).IsOptional();
            Property(x => x.MaquinaId).HasColumnName("MAQ_ID").HasMaxLength(30).IsOptional();
            Property(x => x.UnidadeMedidaId).HasColumnName("UNI_ID").HasMaxLength(30).IsOptional();
            Property(x => x.TurmaId).HasColumnName("TURM_ID").HasMaxLength(30).IsOptional();
            Property(x => x.TurnoId).HasColumnName("TURN_ID").HasMaxLength(30).IsOptional();
            //definição de relacionamentos
            HasOptional(x => x.OcorrenciaPerformace).WithMany(x => x.TarProdOcoPers).HasForeignKey(x => x.OcoIdPerformace);
            HasOptional(x => x.OcorrenciaSetup).WithMany(x => x.TarProdOcoSetups).HasForeignKey(x => x.OcoIdSetup);
            HasOptional(x => x.OcorrenciaSetupAjuste).WithMany(x => x.TarProdOcoSetupAs).HasForeignKey(x => x.OcoIdSetupAjuste);
            HasOptional(x => x.OcorrenciaOpParcial).WithMany(x =>x.TarProdOcoOpParciais).HasForeignKey(x => x.OcoIdOpParcial);
            HasOptional(x => x.Usuario).WithMany(x =>x.TargetsProduto).HasForeignKey(x => x.UsuarioId);

            HasOptional(x => x.MovimentoEstoque).WithMany(x => x.TargetsProduto).HasForeignKey(x => x.MovimentoEstoqueId);
            HasOptional(x => x.Order).WithMany(x => x.TargetsProduto).HasForeignKey(x => x.OrderId);
            HasOptional(x => x.Produto).WithMany(x => x.TargetsProduto).HasForeignKey(x => x.ProdutoId);
            HasOptional(x => x.Maquina).WithMany(x => x.TargetsProduto).HasForeignKey(x => x.MaquinaId);
            HasOptional(x => x.UnidadeMedida).WithMany(x => x.TargetsProduto).HasForeignKey(x => x.UnidadeMedidaId);
            HasOptional(x => x.Turma).WithMany(x => x.TargetsProduto).HasForeignKey(x => x.TurmaId);
            HasOptional(x => x.Turno).WithMany(x => x.TargetsProduto).HasForeignKey(x => x.TurmaId);

            HasOptional(x => x.UnidadeMedida).WithMany(x => x.TargetsProduto).HasForeignKey(x => x.UnidadeMedidaId);
            HasOptional(x => x.Maquina).WithMany(x => x.TargetsProduto).HasForeignKey(x => x.MaquinaId);
            HasOptional(x => x.Produto).WithMany(x => x.TargetsProduto).HasForeignKey(x => x.ProdutoId);

            //definição da chave primaria
            HasKey(x => new { x.Id });
        }
    }
}