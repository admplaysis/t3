using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class ViewFilaProducao
    {
        //cliente
        public string CliId { get; set; }
        public string CliNome{ get; set; }
        public string CliFone { get; set; }
        public string CliObs { get; set; }
        //produto componente
        public string PcProId { get; set; }
        public string PcProDescricao { get; set; }
        public string PcUniId { get; set; }
        //maquina
        public string RotMaqId { get; set; }
        public string MaqDescricao { get; set; }
        //order
        public  string OrdId{ get; set; }
        public string PaProId { get; set; }
        public string PaProDescricao { get; set; }
        public string PaUniId { get; set; }
        public double? OrdPrecoUnitario { get; set; }
        public double OrdQuantidade { get; set; }
        public DateTime OrdDataEntregaDe { get; set; }
        public DateTime OrdDataEntregaAte { get; set; }
        public string OrdTipo { get; set; }
        public double? OrdToleranciaMais { get; set; }
        public double? OrdToleranciaMenos { get; set; }
        //fila de producao
        public DateTime FprDataInicioPrevista { get; set; }
        public DateTime FprDataFimPrevista { get; set; }
        public DateTime FprDataFimMaxima { get; set; }
        public double FprQuantidadePrevista { get; set; }
        public int RotSeqTransformacao{ get; set; }
        public int FprSeqRepeticao{ get; set; }
        public string FprObsProducao { get; set; }
        public string FprStatus { get; set; }
        public int? Produzindo { get; set; }
        //roteiro
        public double? RotQuantPecasPulso { get; set; }
        //medicoes variaveis
        public double? FprTempoDecorridoSetup { get; set; }
        public double? FprTempoDecorridoSetupAjuste { get; set; }
        public double? FprTempoDecorridoPerformacace { get; set; }
        public double? FprQuantidadePerformace { get; set; }
        public double? FprQuantidadeSetup { get; set; }
        public double? FprQuantidadeProduzida { get; set; }
        public double? FprQuantidadeRestante { get; set; }
        public double? FprTempoTeoricoPerformace { get; set; }
        public double? FprTempoRestantePerformace { get; set; }
        public double? FprVelocidadeAtingirMeta { get; set; }
        public double? FprVeloAtuPcSegundo { get; set; }
        public double? FprPerformaceProjetada { get; set; }
        public double? TempoDecorridoPequenasParadas { get; set; }
    }
}