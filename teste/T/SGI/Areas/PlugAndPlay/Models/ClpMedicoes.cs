using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class ClpMedicoes
    {
        public int Id { get; set; }
        public string MaquinaId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public double Quantidade { get; set; }
        public double? Grupo { get; set; }
        public int? Status { get; set; }
        public string TurnoId { get; set; }
        public string TurmaId { get; set; }
        //public string OrdemProducaoId { get; set; }
        public string OcorrenciaId { get; set; }
        public int? IdLoteClp { get; set; }
        public int? Fase { get; set; }
        public DateTime? Emissao { get; set; }
        //public string ProdutoId { get; set; }
        //public int? SequenciaTransformacaoId { get; set; }
        //public int? SequenciaRepeticaoId { get; set; }
        //public int? FilaProducaoId { get; set; }
        public string ClpOrigem { get; set; }
    }


    public class Schedule_ClpMedicoes
    {
        public int Id { get; set; }
        public string MaquinaId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public double Quantidade { get; set; }
        public double? Grupo { get; set; }
        public int? Status { get; set; }
        public string TurnoId { get; set; }
        public string TurmaId { get; set; }
        //public string OrdemProducaoId { get; set; }
        public string OcorrenciaId { get; set; }
        public int? IdLoteClp { get; set; }
        public int? Fase { get; set; }
        public DateTime? Emissao { get; set; }
        //public string ProdutoId { get; set; }
        //public int? SequenciaTransformacaoId { get; set; }
        //public int? SequenciaRepeticaoId { get; set; }
        //public int? FilaProducaoId { get; set; }
        public string ClpOrigem { get; set; }
        public int IdAuxPendente { get; set; }
        public int? FeeID { get; set; }
        public double? FardosPorPalet { get; set; }
        public int Lote { get; set; }

    }



}