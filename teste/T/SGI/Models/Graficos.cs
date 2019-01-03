using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Models
{
    public class Graficos
    {
        public IPagedList<MedicoesInd> Indicadores { get; set; }
        public List<vw_SGI_PARAMETRO_RELMEDICOES> Medicoes { get; set; }
        public List<vw_SGI_PARAMETRO_RELMEDICOES> AnoAnterior { get; set; }
        public List<T_Informacoes_Complementares> Complementares { get; set; }
        public List<T_PlanoAcao> PlanoAcoes { get; set; }
        public List<T_Favoritos> Favoritos { get; set; }
    }
}   