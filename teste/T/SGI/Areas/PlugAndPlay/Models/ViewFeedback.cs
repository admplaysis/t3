using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class ViewFeedback
    {
        public int Id { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime Datafinal { get; set; }
        public String Observacoes { get; set; }
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
        public int? FeeIdMovEstoque { get; set; }
        public string ProdutoDescricao { get; set; }
    }
}