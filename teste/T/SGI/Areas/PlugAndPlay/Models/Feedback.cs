using SGI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class Feedback
    {
        public Feedback()
        {
            MovimentosEstoque = new HashSet<MovimentoEstoque>();
        }
        public int Id { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime Datafinal { get; set; }
        public String  Observacoes { get; set; }
        public double Grupo { get; set; }
        public string ProdutoId { get; set; }
        public string MaquinaId { get; set; }
        public string OcorrenciaId { get; set; }
        public string TurnoId { get; set; }
        public string TurmaId { get; set; }
        public string DiaTurma { get; set; }
        public int UsuarioId { get; set; }
        public string OrderId { get; set; }
        public Nullable<int> SequenciaTransformacao { get; set; }
        public Nullable<int> SequenciaRepeticao { get; set; }
        public double QuantidadePulsos { get; set; }
        public Nullable<double> QuantidadePecasPorPulso { get; set; }
        public virtual Maquina Maquina { get; set; }
        public virtual Ocorrencia Ocorrencia { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual Turno Turno { get; set; }
        public virtual Turma Turma { get; set; }
        public virtual Order Order { get; set; }
        public virtual T_Usuario Usuario { get; set; }
        public virtual ICollection<MovimentoEstoque> MovimentosEstoque { get; set; }
    }
}