using PagedList;
using SGI.Autenticacao;
using SGI.Context;
using SGI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Util
{
    public static class ExtratorMedidor
    {
        /// <summary>
        /// Metódo responsavel por retornar agrupado os indicadores para serem utilizados na páginação
        /// </summary>
        /// <param name="nPageSize">Registros por página</param>
        /// <param name="page">Página a ser exibida</param>
        /// <param name="idNegocio">Código do negócio</param>
        /// <param name="pAno">Ano a ser filtrado</param>
        /// <returns>Irá retornar lista de indicadores com ano,id negocio e id meta</returns>
        public static IPagedList<MedicoesInd> GetIndicadores(int? nPageSize, int? page, int? idNegocio, int? idGrupo, int? idUnidade, int? idDepartamento, string pAno, string search, List<T_Favoritos> Favoritos)
        {
            JSgi db = new JSgi();
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            var agrupado = new List<MedicoesInd>();
            //Agrupa dados por mês, Indicador e meta
            var indicadores = db.vw_SGI_PARAMETRO_RELMEDICOES
                .Select(x => new { Ano = x.Mes.Substring(0, 4), IND_DESCRICAO = x.IND_DESCRICAO, IND_ID = x.IND_ID, NEG_ID = x.NEG_ID, UNI_ID = x.UNID })
                .GroupBy(x => new { x.IND_ID, x.IND_DESCRICAO, x.NEG_ID, x.Ano, x.UNI_ID }).OrderByDescending(x => x.Key.Ano).ToList();
            #region Filtros

            //Filtra grupos por usuário
            /*if (!HttpContext.Current.User.IsInRole("Admin"))
            {
                var usuario = db.T_Usuario.First(x => x.ID_USUARIO == ((ICustomPrincipal)HttpContext.Current.User).Id);
                var grupos = db.T_USER_GRUPO.Where(x => x.ID_USUARIO == usuario.ID_USUARIO).ToList();
                var indGrupos = db.T_Grupo_Indicador.ToList().Where(x => grupos.Any(g => g.GRU_ID == x.GRU_ID)).ToList();
                indicadores = indicadores.Where(x => indGrupos.Any(g => x.Key.IND_ID == g.IND_ID)).ToList();
            }*/

            //Filtra Ano
            if (pAno != "" && pAno != null)
                indicadores = indicadores.Where(x => x.Key.Ano == pAno).ToList();
            //Filtra Negócio
            if (idNegocio > 0)
                indicadores = indicadores.Where(x => x.Key.NEG_ID == idNegocio).ToList();

            //Filtra Grupo
            if (idGrupo > 0)
            {
                var grupos = db.T_Grupo_Indicador.Where(x => x.GRU_ID == idGrupo).ToList();
                indicadores = indicadores.Where(x => grupos.Any(g => g.IND_ID == x.Key.IND_ID)).ToList();
            }

            //Filtra Departamento
            if (idDepartamento > 0)
            {
                var departamentos = db.T_Indicadores_Departamentos.Where(x => x.DEP_ID == idDepartamento).ToList();
                indicadores = indicadores.Where(x => departamentos.Any(d => d.IND_ID == x.Key.IND_ID)).ToList();
            }

            //Filtra por unidade
            if (idUnidade > 0)
                indicadores = indicadores.Where(x => x.Key.UNI_ID == idUnidade).ToList();

            //Filtra por pesquisa de usuário
            if (!string.IsNullOrEmpty(search))
                indicadores = indicadores.Where(x => (x.Key.IND_DESCRICAO).ToUpper().Contains(search.ToUpper())).ToList();

            #endregion Filtros
            //Percorre registros recuperados
            foreach (var item in indicadores.OrderBy(x => x.Key.IND_DESCRICAO))
            {
                if (agrupado.Count(x => x.IND_ID == item.Key.IND_ID) <= 0)
                {
                    //busca detalhes do indicador
                    var indicador = db.T_Indicadores.Where(x => x.IND_ID == item.Key.IND_ID).FirstOrDefault();
                    //busca as dimensoes do indicador
                    indicador.Dimensoes = db.T_Medicoes
                        .Where(x => x.IndId == indicador.IND_ID && x.DimId != null)
                        .Select(x => new Dimensao
                        {
                            Id = (int)x.DimId,
                            Descricao = x.DimDescricao
                        }).Distinct().ToList();

                    //busca as sub dimensoes do indicador
                    /*var SubDimensoes = db.T_Medicoes
                        .Where(x => x.IndId == indicador.IND_ID && x.DimSubId != null)
                        .Select(x => new SubDimensao
                        {
                            Id = x.DimSubId,
                            Descricao = x.DimSubDescricao
                        }).Distinct().ToList();
                        
                    indicador.Dimensoes.ElementAt(0).SubDimensao = SubDimensoes;
                    */
                    var dimId = indicador.Dimensoes.ElementAt(0).Id;
                    //busca os periodos de um indicador baseado na primeira dimensão recuperada

                    var periodos = db.T_Medicoes
                        .Where(x => x.IndId == indicador.IND_ID 
                            && x.DimId != null && x.DimId == dimId 
                            && x.PerId != null && x.PerId.Trim().ToUpper() != "MAC" && x.PerId.Trim().ToUpper() != "DAC")
                        .Select(x => new Periodo { Id = x.PerId, Descricao = x.PerDescricao })
                        .Distinct().ToList();
                    
                    //adciona os periodos na primeira posicao da lista de dimensoes do indicador
                    indicador.Dimensoes.ElementAt(0).Periodos = periodos;
                    agrupado.Add(new MedicoesInd()
                    {
                        ID_FAVORITO = (Favoritos.Count(x => x.ID_INDICADOR == item.Key.IND_ID) > 0 ? item.Key.IND_ID : 0),
                        IND_ID = item.Key.IND_ID,
                        Ano = item.Key.Ano,
                        Indicador = indicador,
                        DESC_CALCULO = indicador.DESC_CALCULO,
                        TIPO_COMPARADOR = indicador.IND_TIPOCOMPARADOR.ToString(),
                        IND_CONEXAO = indicador.IND_CONEXAO,
                        IND_GRAFICO = indicador.IND_GRAFICO,
                        DIM_ID  = indicador.DIM_ID,
                        PER_ID = indicador.PER_ID
                    });
                }
            }
            return agrupado.OrderBy(x => x.Indicador.IND_DESCRICAO).OrderByDescending(x => x.ID_FAVORITO).ToPagedList(_PageNumber, _PageSize);
        }

        /// <summary>
        /// Metódo responsavel por retornar agrupado os indicadores para serem utilizados na páginação
        /// </summary>
        /// <param name="nPageSize">Registros por página</param>
        /// <param name="page">Página a ser exibida</param>
        /// <param name="idNegocio">Código do negócio</param>
        /// <param name="pAno">Ano a ser filtrado</param>
        /// <returns>Irá retornar lista de indicadores com ano,id negocio e id meta</returns>
        public static IPagedList<MedicoesInd> GetIndicadores(string pesquisa, int? nPageSize, int? page, int? idNegocio, int? idGrupo, int? idUnidade, int? idDepartamento, string pAno)
        {
            JSgi db = new JSgi();
            var _PageNumber = page ?? 1;
            var _PageSize = nPageSize ?? 10;
            var agrupado = new List<MedicoesInd>();
            //Agrupa dados por mês, Indicador e meta
            var indicadores = db.vw_SGI_PARAMETRO_RELMEDICOES
                .Select(x => new { Ano = x.Mes.Substring(0, 4), IND_ID = x.IND_ID, NEG_ID = x.NEG_ID, UNI_ID = x.UNID })
                .GroupBy(x => new { x.IND_ID, x.NEG_ID, x.Ano, x.UNI_ID }).ToList();
            #region Filtros
            //Filtra grupos por usuário
            if (!HttpContext.Current.User.IsInRole("Admin"))
            {
                var usuario = db.T_Usuario.First(x => x.EMAIL == HttpContext.Current.User.Identity.Name);
                var grupos = db.T_USER_GRUPO.Where(x => x.ID_USUARIO == usuario.ID_USUARIO).ToList();
                var gruposLiberados = db.T_Grupo_Indicador.Where(x => grupos.Any(g => g.GRU_ID == x.GRU_ID)).ToList();
                indicadores = indicadores.Where(x => gruposLiberados.Any(g => x.Key.IND_ID == g.IND_ID)).ToList();
            }
            //Filtra Ano
            if (pAno != "" && pAno != null)
                indicadores = indicadores.Where(x => x.Key.Ano == pAno).ToList();
            //Filtra Negócio
            if (idNegocio > 0)
                indicadores = indicadores.Where(x => x.Key.NEG_ID == idNegocio).ToList();
            //Filtra Grupo
            if (idGrupo > 0)
            {
                var grupos = db.T_Grupo_Indicador.Where(x => x.GRU_ID == idGrupo).ToList();
                indicadores = indicadores.Where(x => grupos.Any(g => g.IND_ID == x.Key.IND_ID)).ToList();
            }
            //Filtra por unidade
            if (idUnidade > 0)
                indicadores = indicadores.Where(x => x.Key.UNI_ID == idUnidade).ToList();
            //Filtra Departamento
            if (idDepartamento > 0)
            {
                var departamentos = db.T_Indicadores_Departamentos.Where(x => x.DEP_ID == idDepartamento).ToList();
                indicadores = indicadores.Where(x => departamentos.Any(d => d.IND_ID == x.Key.IND_ID)).ToList();
            }
            #endregion Filtros
            //Percorre registros recuperados
            foreach (var item in indicadores)
            {
                if (agrupado.Count(x => x.IND_ID == item.Key.IND_ID) <= 0)
                {
                    agrupado.Add(new MedicoesInd()
                    {
                        IND_ID = item.Key.IND_ID,
                        Ano = item.Key.Ano,
                        Indicador = db.T_Indicadores.Find(item.Key.IND_ID)
                    });
                }
            }
            return agrupado.OrderBy(x => x.MET_ID).ToPagedList(_PageNumber, _PageSize);
        }

        /// <summary>
        /// Metódo para processar as medições filtrando por indicadores
        /// </summary>
        /// <param name="indicadores">Objeto com dados dos indicadores para serem filtrados.</param>
        /// <param name="pAno">Ano a ser filtrado</param>
        /// <returns></returns>
        public static List<vw_SGI_PARAMETRO_RELMEDICOES> GetMedicoes(List<MedicoesInd> indicadores, string pAno)
        {
            JSgi db2 = new JSgi();
            List<vw_SGI_PARAMETRO_RELMEDICOES> medicoes = new List<vw_SGI_PARAMETRO_RELMEDICOES>();
            var medicao = new List<vw_SGI_PARAMETRO_RELMEDICOES>();
            foreach (var item in indicadores)
            {
                //Busca dados no banco filtrando por indicador e meta
                medicao = db2.Database.SqlQuery<vw_SGI_PARAMETRO_RELMEDICOES>("select * from vw_SGI_PARAMETRO_RELMEDICOES WHERE IND_ID = '" + item.IND_ID.ToString() + "' ").ToList();
                //db2.vw_SGI_PARAMETRO_RELMEDICOES.Where(x => x.IND_ID == item.IND_ID && x.MET_ID == item.MET_ID).ToList();
                //Appenda dados no objeto lista medições
                if (pAno != "" && pAno != null)
                    medicao = medicao.Where(x => x.Mes.Substring(0, 4) == pAno).ToList();
                medicoes.AddRange(medicao);
            }
            return medicoes;
        }

        /// <summary>
        /// Metódo para processar as medições filtrando por indicadores
        /// </summary>
        /// <param name="indicadores">Objeto com dados dos indicadores para serem filtrados.</param>
        /// <param name="pAno">Ano a ser filtrado</param>
        /// <returns>Objeto do tipo T_Medicoes</returns>
        public static List<T_Medicoes> GetMedicao(List<MedicoesInd> indicadores, string pAno)
        {
            JSgi db = new JSgi();
            List<T_Medicoes> medicoes = new List<T_Medicoes>();
            foreach (var item in indicadores)
            {
                //Busca dados no banco filtrando por indicador e meta
                var medicao = db.T_Medicoes.Where(x => x.T_Metas.IND_ID == item.IND_ID && x.MET_ID == item.MET_ID).ToList();
                //Appenda dados no objeto lista medições
                if (pAno != "" && pAno != null)
                    medicao = medicao.Where(x => x.MED_DATAMEDICAO.Substring(0, 4) == pAno).ToList();
                medicoes.AddRange(medicao);
            }
            return medicoes;
        }
    }
}