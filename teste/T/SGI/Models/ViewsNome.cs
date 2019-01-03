using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Models
{
    public class ViewsNome
    {
        public T_Indicadores Indicador { get; set; }
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public List<ViewsCampos> Campos { get; set; }
        public string[,] Valores { get; set; }
    }


    public class Coluna
    {
        public string Nome { get; set; }
        public string Tipo { get; set; }
    }

    public class LineData
    {
        public LineData()
        {
            linha = new List<string>();
        }
        public List<String> linha { get; set; }
    }

    public class Parametro
    {
        public string Idfull { get; set; }
        public string Idform { get; set; }
        public string Descricao { get; set; }
        public string Tipo { get; set; }
        public string Conteudo { get; set; }
    }

    public class Resultquery
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Tipo { get; set; }
        public int Qtdlinhas { get; set; }
        public List<Coluna> Colunas { get; set; }
        public List<Parametro> Parametros { get; set; }
        public List<LineData> Linhas { get; set; }
        public string[,] Dados { get; set; }

    }
    public class V_PAINEL_LISTA_METAS
    {
        public string IND_DESCRICAO { get; set; }
        public String MED_DATAMEDICAO { get; set; }
        public string MED_VALOR { get; set; }
        public int IND_ID { get; set; }
        public int MET_TIPOALVO { get; set; }
        public string MET_ALVO { get; set; }
        public double? MET_RANGE01 { get; set; }
        public double? MET_RANGE02 { get; set; }
        public double? MET_RANGE03 { get; set; }
        public string MET_DTINICIO { get; set; }
        public string MET_DTFIM { get; set; }
        public int DIM_ID { get; set; }
        public string FAT_ID { get; set; }
        public string DIM_SUBDIMENSAO_ID { get; set; }
        public string PER_ID { get; set; }
        public string DOM_FILIAL { get; set; }
        public string DOM_EMPRESA { get; set; }
        public string FAT_DESCRICAO { get; set; }
        public string PER_DESCRICAO { get; set; }
        public string DIM_SUB_DESCRICAO { get; set; }
    }

    public class V_PAINEL_GESTOR_STATUS_MAQUINAS
    {
        public int? FEEDBACKS_PENDENTES { get; set; }
        public String ROT_MAQ_ID { get; set; }
        public String MAQ_DESCRICAO { get; set; }
        public String MAQ_STATUS { get; set; }
        public String ULTIMA_ATUALIZACAO { get; set; }
        public String STATUS_COR_MAQUINA { get; set; }
        public String OCO_ID { get; set; }
        public String OCO_DESCRICAO { get; set; }
        public String FEE_OBSERVACOES { get; set; }
        public String TEMPO_SEM_FEEDBACK { get; set; }
        public String OPS_PARCIAIS { get; set; }
        public String STATUS_COR_SETUP { get; set; }
        public double? PERCENTUAL_SETUP { get; set; }
        public String STATUS_COR_SETUP_AJUSTE { get; set; }
        public Double? PERCENTUAL_SETUP_AJUSTE { get; set; }


        public String STATUS_COR_PERFORMANCE { get; set; }
        public Double? PERCENTUAL_PERFORMANCE { get; set; }

        public int? SETUP_GERAL_AZUL { get; set; }
        public int? SETUP_GERAL_VERDE { get; set; }
        public int? SETUP_GERAL_AMARELO { get; set; }
        public int? SETUP_GERAL_VERMELHO { get; set; }
        public int? PERFORMANCE_AZUL { get; set; }
        public int? PERFORMANCE_VERDE { get; set; }
        public int? PERFORMANCE_AMARELO { get; set; }
        public int? PERFORMANCE_VERMELHO { get; set; }
        public int? TAR_ID { get; set; }
        public Double? QTD_PARADAS_NAO_PROGRAMADAS_EXETO_SETUP { get; set; }
        public Double? QTD_PEQUENAS_PARADAS { get; set; }
        public String TEMPO_PARADAS_NAO_PROGRAMADAS_EXETO_SETUP { get; set; }
        public String TEMPO_PEQUENAS_PARADAS { get; set; }
        public String STATUS_FILA { get; set; }
        public Double? VELO_ATU_PC_SEGUNDO_ROUD_1 { get; set; }
        public Double? PERCENTUAL_CONCLUIDO_SETUP { get; set; }
        public Double? PERCENTUAL_CONCLUIDO_SETUP_AJUSTE { get; set; }
        public Double? PERCENTUAL_CONCLUIDO_PERFORMANCE { get; set; }

        public Double? FPR_QTD_PRODUZIDA { get; set; }
        public Double? FPR_QUANTIDADE_PREVISTA { get; set; }
    }
    public class V_PAINEL_GESTOR_DESEMPENHO_TURNOS_MAQUINA
    {
        public String FEE_DIA_TURMA { get; set; }
        public String MAQ_ID { get; set; }
        public String TURN_ID { get; set; }
        public String TURM_ID { get; set; }
        public String TEMPO_PARADAS_NAO_PROGRAMADAS { get; set; }
        public Double? QTD_PARADAS_NAO_PROGRAMADAS { get; set; }
        public String TEMPO_PARADAS_NAO_PROGRAMADAS_EXETO_SETUP { get; set; }
        public Double? QTD_PARADAS_NAO_PROGRAMADAS_EXETO_SETUP { get; set; }
        public String TEMPO_PEQUENAS_PARADAS { get; set; }
        public Double QTD_PEQUENAS_PARADAS { get; set; }
        public Double? TEMPO_PLANEJADO { get; set; }
        public Double? TEMPO_PRODUZINDO { get; set; }
        public Double? SETUP_GERAL_AZUL { get; set; }
        public Double? SETUP_GERAL_VERDE { get; set; }
        public Double? SETUP_GERAL_AMARELO { get; set; }
        public Double? SETUP_GERAL_VERMELHO { get; set; }
        public Double? PERFORMANCE_AZUL { get; set; }
        public Double? PERFORMANCE_VERDE { get; set; }
        public Double? PERFORMANCE_AMARELO { get; set; }
        public Double? PERFORMANCE_VERMELHO { get; set; }
        public Double? QTD_SETUPS { get; set; }
    }

    public class V_PAINEL_GESTOR_DESEMPENHO_TURNOS
    {
        public String FEE_DIA_TURMA { get; set; }
        public String TURN_ID { get; set; }
        public String TURM_ID { get; set; }
        public String TEMPO_PARADAS_NAO_PROGRAMADAS { get; set; }
        public Double? QTD_PARADAS_NAO_PROGRAMADAS { get; set; }
        public String TEMPO_PARADAS_NAO_PROGRAMADAS_EXETO_SETUP { get; set; }
        public Double? QTD_PARADAS_NAO_PROGRAMADAS_EXETO_SETUP { get; set; }
        public String TEMPO_PEQUENAS_PARADAS { get; set; }
        public Double? QTD_PEQUENAS_PARADAS { get; set; }
        public Double? TEMPO_PLANEJADO { get; set; }
        public Double? TEMPO_PRODUZINDO { get; set; }
        public Double? SETUP_GERAL_AZUL { get; set; }
        public Double? SETUP_GERAL_VERDE { get; set; }
        public Double? SETUP_GERAL_AMARELO { get; set; }
        public Double? SETUP_GERAL_VERMELHO { get; set; }
        public Double? PERFORMANCE_AZUL { get; set; }
        public Double? PERFORMANCE_VERDE { get; set; }
        public Double? PERFORMANCE_AMARELO { get; set; }
        public Double? PERFORMANCE_VERMELHO { get; set; }
        public Double? QTD_SETUPS { get; set; }
    }
}