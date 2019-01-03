using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class FilaProducao
    {
        // DETERMINA A SEQUENCIA DE PRODUCAO POR EXEMPLO UM PRODUTO ACABADO QUE TEM UMA UNICA TRANSFORMACAO A SEQUENCIA SERA 99 (SEQUENCIA 99 DETERMINA ULTIMA PORCESSO ANTES DE VIRAR PRODUTO ACABADO)
        // PRODUTOS COM MAIS DE QUE UMA FAZE DE TRANSFORMACAO EXEMPLO  UMA CAIXA    PARA FAZER UMA CAIXA DEVEMOS FAZER UMA CHAPA PARA FAZER CHAPA DEVEMOS COMPRAR MATERIA PRIMA ENTAO A SEQUENCIA FICA
        // 01 PRODUCAO DA CHAPA 99 PRODUCAO DA CAIXA. OUTRO EXEMPLO PARA FAZER UMA CAIXA DEVEMOS PASSAR POR DUAS MAQUINAS PARA FINALIZAR O PRODUTO ACABADO FICARIA ASSIM
        // SEQUENCIA 01 FABRICACAO DA CHAPA
        // SEQUENCIA 02 FABRICA CAIXA
        // SEQUENCIA 99 GRANPEIA PARA FINALIZAR A PRODUCAO.
        // DETERMINA A ORDEM NA FILA  O OPERADOR DEVE SEGUIR ESTA ORDEM
        [Display(Name = "Pedido")]
        public string OrderId { get; set; }
        [Display(Name = "Máquina")]
        //Roteiro
        public string MaquinaId { get; set; }
        [Display(Name = "Produto")]
        public string ProdutoId { get; set; }
        public Produto Produto { get; set; }
        [Display(Name = "Sequencia de Transformação")]
        public int SequenciaTransformacao { get; set; }
        public virtual Roteiro Roteiro { get; set; }
        //pedido
        public virtual Order Order { get; set; }
        [Display(Name = "Data início")]
        public DateTime DataInicioPrevista { get; set; }
        [Display(Name = "Data Fim")]
        public DateTime DataFimPrevista { get; set; }
        [Display(Name = "Data Fim Máxima")]
        public DateTime DataFimMaxima { get; set; }
        
        [Display(Name = "Sequencia de Repetição")]
        public int? SequenciaRepeticao { get; set; }
        [Display(Name = "Observacões")]
        public string ObservacaoProducao { get; set; }
        [Display(Name = "Quantidade Prevista")]
        public double QuantidadePrevista { get; set; }
        public string Status { get; set; }

        public int? Produzindo { get; set; }

        //medicoes variaveis
        public double? TempoDecorridoSetup { get; set; }
        public double? TempoDecorridoSetupAjuste { get; set; }
        public double? TempoDecorridoPerformacace { get; set; }
        public double? QuantidadePerformace { get; set; }
        public double? QuantidadeSetup { get; set; }
        public double? QuantidadeProduzida { get; set; }
        public double? QuantidadeRestante { get; set; }
        public double? TempoTeoricoPerformace { get; set; }
        public double? TempoRestantePerformace { get; set; }
        public double? VelocidadeAtingirMeta { get; set; }
        public double? VeloAtuPcSegundo { get; set; }
        public double? PerformaceProjetada { get; set; }
        public double? TempoDecorridoPequenasParadas { get; set; }
        
        
    }
}