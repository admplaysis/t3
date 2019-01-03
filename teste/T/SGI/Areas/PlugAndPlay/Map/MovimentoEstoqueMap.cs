using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class MovimentoEstoqueMap : EntityTypeConfiguration<MovimentoEstoque>
    {
        public MovimentoEstoqueMap()
        {
            ToTable("T_MOVIMENTOS_ESTOQUE");
            Property(x => x.Id).HasColumnName("MOV_ID");
            Property(x => x.Quantidade).HasColumnName("MOV_QUANTIDADE").IsRequired();
            Property(x => x.Tipo).HasColumnName("TIP_ID").HasMaxLength(3).IsRequired();
            Property(x => x.DataHoraCriacao).HasColumnName("MOV_DATA_HORA_EMISSAO").IsRequired();
            Property(x => x.DataHoraEmissao).HasColumnName("MOV_DATA_HORA_CRIACAO").IsRequired();
            Property(x => x.DiaTurma).HasColumnName("MOV_DIA_TURMA").HasMaxLength(8).IsOptional();
            Property(x => x.Lote).HasColumnName("MOV_LOTE").IsOptional();
            Property(x => x.SubLote).HasColumnName("MOV_SUB_LOTE").IsOptional();
            Property(x => x.Observacao).HasColumnName("MOV_OBS").HasMaxLength(200).IsOptional();
            Property(x => x.ProdutoId).HasColumnName("PRO_ID").HasMaxLength(30).IsRequired();
            Property(x => x.OrderId).HasColumnName("ORD_ID").HasMaxLength(30).IsRequired();
            Property(x => x.MaquinaId).HasColumnName("MAQ_ID").HasMaxLength(30).IsRequired();
            Property(x => x.UsuarioId).HasColumnName("USU_ID").IsRequired();
            Property(x => x.OcorrenciaId).HasColumnName("OCO_ID").IsOptional();
            Property(x => x.Armazem).HasColumnName("MOV_ARMAZEM").HasMaxLength(30).IsOptional();
            Property(x => x.Endereco).HasColumnName("MOV_ENDERECO").HasMaxLength(30).IsOptional();
            Property(x => x.Estorno).HasColumnName("MOV_ESTORNO").HasMaxLength(2).IsOptional();
            Property(x => x.SequenciaTransformacao).HasColumnName("FPR_SEQ_TRANFORMACAO").IsOptional();
            Property(x => x.SequenciaRepeticao).HasColumnName("FPR_SEQ_REPETICAO").IsOptional();
            Property(x => x.TurnoId).HasColumnName("TURN_ID").IsOptional();
            Property(x => x.TurmaId).HasColumnName("TURM_ID").IsOptional();
            Property(x => x.ObsOpParcial).HasColumnName("MOV_OBS_OP_PARCIAL").HasMaxLength(200).IsOptional();
            Property(x => x.OcoIdOpParcial).HasColumnName("MOV_OCO_ID_OP_PARCIAL").IsOptional();

            HasKey(x => x.Id);

            HasRequired(x => x.Produto).WithMany(x => x.MovimentosEstoque).HasForeignKey(x => x.ProdutoId);
            HasRequired(x => x.Order).WithMany(x => x.MovimentosEstoque).HasForeignKey(x => x.OrderId);
            HasRequired(x => x.Maquina).WithMany(x => x.MovimentosEstoque).HasForeignKey(x => x.MaquinaId);
            HasRequired(x => x.Usuario).WithMany(x => x.MovimentosEstoque).HasForeignKey(x => x.UsuarioId);
            HasOptional(x => x.Ocorrencia).WithMany(x => x.MovimentosEstoque).HasForeignKey(x => x.OcorrenciaId);
            HasOptional(x => x.OcorrenciaOpParcial).WithMany(x => x.MovimentosEstoqueOpParcial).HasForeignKey(x => x.OcoIdOpParcial);
            
            HasMany(x => x.Feedbacks).WithMany(x => x.MovimentosEstoque).Map(x => {
                x.MapLeftKey("MOV_ID");
                x.MapRightKey("FEE_ID");
                x.ToTable("T_FEEDBACK_MOV_ESTOQUE");
            });
        }
    }
}