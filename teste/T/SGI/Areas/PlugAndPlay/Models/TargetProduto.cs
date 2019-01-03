using SGI.Context;
using SGI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class TargetProduto
    {
        public int Id { get; set; }
        public string DiaTurma { get; set; }
        public double? MetaPerformace { get; set; }
        public double? RealizadoPerformace { get; set; }
        public double? ProximaMetaPerformace { get; set; }
        public double? MetaTempoSetup { get; set; }
        public double? RealizadoTempoSetup { get; set; }
        public double? ProximaMetaTempoSetup { get; set; }
        public double? MetaTempoSetupAjuste { get; set; }
        public double? RealizadoTempoSetupAjuste { get; set; }
        public double? ProximaMetaTempoSetupAjuste { get; set; }
        //observações
        public string ObsPerformace { get; set; }
        public string ObsSetup { get; set; }
        public string ObsSetupAjuste { get; set; }
        public string ObsOpParcial { get; set; }
        //tipo de feedbacks
        public string TipoFeedbackPerformace { get; set; }
        public string TipoFeedbackSetup { get; set; }
        public string TipoFeedbackSetupAjuste { get; set; }
        //outros
        public double? Quantidade { get; set; }
        public int? ParametroTimeWorkStopMachine { get; set; }
        public int? ParametroTempoQuebraLote { get; set; }
        //chaves estrangeiras para ocorrencias
        public string OcoIdPerformace { get; set; }
        public string OcoIdSetup { get; set; }
        public string OcoIdSetupAjuste { get; set; }
        public string OcoIdOpParcial { get; set; }
        //parametros para as cores 
        public double? PerformaceMaxVerde { get; set; }
        public double? PerformaceMinVerde { get; set; }
        public double? SetupMaxVerde { get; set; }
        public double? SetupMinVerde { get; set; }
        public double? SetupAjusteMaxVerde { get; set; }
        public double? SetupAjusteMinVerde { get; set; }

        public double? SetupMaxAmarelo { get; set; }
        public double? SetupAjusteMaxAmarelo { get; set; }

        public double? PerformaceMinAmarelo { get; set; }
        //chaves estrangeiras
        public int MovimentoEstoqueId { get; set; }
        public string OrderId { get; set; }
        public string ProdutoId { get; set; }
        public string MaquinaId { get; set; }
        public string UnidadeMedidaId { get; set; }
        public string TurmaId { get; set; }
        public string TurnoId { get; set; }
        public int UsuarioId { get; set; }
        //
        public DateTime DiaTurmaD { get; set; }

        public virtual MovimentoEstoque MovimentoEstoque { get; set; }
        public virtual Order Order { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual Maquina Maquina { get; set; }
        public virtual UnidadeMedida UnidadeMedida { get; set; }
        public virtual Turma Turma { get; set; }
        public virtual Turno Turno { get; set; }

        public virtual Ocorrencia OcorrenciaPerformace { get; set; }
        public virtual Ocorrencia OcorrenciaSetup { get; set; }
        public virtual Ocorrencia OcorrenciaSetupAjuste { get; set; }
        public virtual Ocorrencia OcorrenciaOpParcial { get; set; }

        public T_Usuario Usuario { get; set; }

        public TargetProduto ObterUltimaMeta(string maquinaId, string produtoId, int? movId = null, JSgi db = null)
        {
            TargetProduto metaPerformace, metaSetup, metaSetupAjuste;
            TargetProduto target = null;
            if (db == null)
                db = new JSgi();

            if (movId ==  null)
            {
                metaPerformace = db.TargetProduto.Where(t => t.MaquinaId == maquinaId && t.ProdutoId == produtoId && t.RealizadoPerformace != -1)
                    .OrderByDescending(t => t.MovimentoEstoqueId).Take(1).FirstOrDefault();
                metaSetup = db.TargetProduto.Where(t => t.MaquinaId == maquinaId && t.ProdutoId == produtoId && t.RealizadoPerformace != -1)
                    .OrderByDescending(t => t.MovimentoEstoqueId).Take(1).FirstOrDefault();
                metaSetupAjuste = db.TargetProduto.Where(t => t.MaquinaId == maquinaId && t.ProdutoId == produtoId && t.RealizadoPerformace != -1)
                    .OrderByDescending(t => t.MovimentoEstoqueId).Take(1).FirstOrDefault();
            }
            else
            {
                metaPerformace = db.TargetProduto.Where(t => t.MovimentoEstoqueId < movId && t.MaquinaId == maquinaId && t.ProdutoId == produtoId && t.RealizadoPerformace != -1)
                    .OrderByDescending(t => t.MovimentoEstoqueId).Take(1).FirstOrDefault();
                metaSetup = db.TargetProduto.Where(t => t.MovimentoEstoqueId < movId && t.MaquinaId == maquinaId && t.ProdutoId == produtoId && t.RealizadoPerformace != -1)
                    .OrderByDescending(t => t.MovimentoEstoqueId).Take(1).FirstOrDefault();
                metaSetupAjuste = db.TargetProduto.Where(t => t.MovimentoEstoqueId < movId && t.MaquinaId == maquinaId && t.ProdutoId == produtoId && t.RealizadoPerformace != -1)
                    .OrderByDescending(t => t.MovimentoEstoqueId).Take(1).FirstOrDefault();
            }
            if (metaPerformace != null && metaSetup != null && metaSetupAjuste != null)
            {
                target = new TargetProduto()
                {
                    PerformaceMinAmarelo = metaPerformace.PerformaceMinAmarelo,
                    PerformaceMinVerde = metaPerformace.PerformaceMinVerde,
                    PerformaceMaxVerde = metaPerformace.PerformaceMaxVerde,
                    RealizadoPerformace = metaPerformace.RealizadoPerformace,
                    ProximaMetaPerformace = metaPerformace.ProximaMetaPerformace,
                    SetupMinVerde = metaSetup.SetupMinVerde,
                    SetupMaxVerde = metaSetup.SetupMaxVerde,
                    SetupMaxAmarelo = metaSetup.SetupMaxAmarelo,
                    RealizadoTempoSetup = metaSetup.RealizadoTempoSetup,
                    ProximaMetaTempoSetup = metaSetup.ProximaMetaTempoSetup,
                    SetupAjusteMinVerde = metaSetupAjuste.SetupAjusteMinVerde,
                    SetupAjusteMaxVerde = metaSetupAjuste.SetupAjusteMaxVerde,
                    SetupAjusteMaxAmarelo = metaSetupAjuste.SetupAjusteMaxAmarelo,
                    RealizadoTempoSetupAjuste = metaSetupAjuste.RealizadoTempoSetupAjuste,
                    ProximaMetaTempoSetupAjuste = metaSetupAjuste.ProximaMetaTempoSetupAjuste
                };
            }
            return target;
        }
    }
}