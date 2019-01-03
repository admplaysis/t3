using SGI.Areas.PlugAndPlay.Models;
using SGI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class FeedbackMap : EntityTypeConfiguration<Feedback>
    {
        public FeedbackMap()
        {
            ToTable("T_FEEDBACK");
            HasKey(x => x.Id);
            //colunas comuns
            Property(x => x.Id).HasColumnName("FEE_ID").IsRequired();
            Property(x => x.DataInicial).HasColumnName("FEE_DATA_INICIAL").IsRequired();
            Property(x => x.Datafinal).HasColumnName("FEE_DATA_FINAL").IsRequired();
            Property(x => x.Observacoes).HasColumnName("FEE_OBSERVACOES").HasMaxLength(100).IsOptional();
            Property(x => x.Grupo).HasColumnName("FEE_GRUPO").IsRequired();
            Property(x => x.DiaTurma).HasColumnName("FEE_DIA_TURMA").IsOptional();
            Property(x => x.SequenciaTransformacao).HasColumnName("ROT_SEQ_TRANFORMACAO").IsOptional();
            Property(x => x.SequenciaRepeticao).HasColumnName("FPR_SEQ_REPETICAO").IsOptional();
            Property(x => x.QuantidadePulsos).HasColumnName("FEE_QTD_PULSOS").IsOptional();
            Property(x => x.QuantidadePecasPorPulso).HasColumnName("FEE_QTD_PECAS_POR_PULSO").IsOptional();
            //chaves estrangeiras
            Property(x => x.MaquinaId).HasColumnName("MAQ_ID").HasMaxLength(30).IsRequired();
            Property(x => x.TurnoId).HasColumnName("TURN_ID").HasMaxLength(10).IsRequired();
            Property(x => x.TurmaId).HasColumnName("TURM_ID").HasMaxLength(10).IsRequired();
            Property(x => x.OrderId).HasColumnName("ORD_ID").HasMaxLength(30).IsOptional();
            Property(x => x.OcorrenciaId).HasColumnName("OCO_ID").HasMaxLength(10).IsOptional();
            Property(x => x.UsuarioId).HasColumnName("USU_ID").IsRequired();
            Property(x => x.ProdutoId).HasColumnName("PRO_ID").IsOptional();

            HasRequired(x => x.Maquina).WithMany(x => x.Feedbacks).HasForeignKey(x => x.MaquinaId);
            HasRequired(x => x.Turno).WithMany(x => x.Medicoes).HasForeignKey(x => x.TurnoId);
            HasRequired(x => x.Turma).WithMany(x => x.Feedbacks).HasForeignKey(x => x.TurmaId);
            HasRequired(x => x.Usuario).WithMany(x => x.T_Feedbacks).HasForeignKey(x => x.UsuarioId);
            HasRequired(x => x.Produto).WithMany(x => x.Feedbacks).HasForeignKey(x => x.ProdutoId);
            HasOptional(x => x.Order).WithMany(x => x.Medicoes).HasForeignKey(x => x.OrderId);
            HasOptional(x => x.Ocorrencia).WithMany(x => x.Medicoes).HasForeignKey(x => x.OcorrenciaId);
        }
    }
}