using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SGI.Models;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class MovimentoEstoque
    {
        public MovimentoEstoque()
        {
            Feedbacks = new HashSet<Feedback>();
            TargetsProduto = new HashSet<TargetProduto>();
        }
        public int Id { get; set; }
        public double Quantidade { get; set; }
        public DateTime DataHoraEmissao { get; set; }
        public DateTime DataHoraCriacao { get; set; }
        public string DiaTurma { get; set; }
        public string Lote { get; set; }
        public string SubLote { get; set; }
        public string Observacao { get; set; }
        public string MaquinaId { get; set; }
        public string OcorrenciaId { get; set; }
        public int UsuarioId { get; set; }
        public string ProdutoId { get; set; }
        public string OrderId { get; set; }
        public string Tipo { get; set; }
        public string Armazem { get; set; }
        public string Endereco  { get; set; }
        public string Estorno { get; set; }
        public int SequenciaTransformacao { get; set; }
        public int SequenciaRepeticao { get; set; }
        public string TurmaId { get; set; }
        public string TurnoId { get; set; }
        public string ObsOpParcial { get; set; }
        public string OcoIdOpParcial { get; set; }
        public virtual Ocorrencia Ocorrencia { get; set; }
        public virtual Ocorrencia OcorrenciaOpParcial { get; set; }
        public virtual Maquina Maquina { get; set; }
        public virtual T_Usuario Usuario { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual Order Order{ get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<TargetProduto> TargetsProduto { get; set; }
    }
}