using SGI.Areas.PlugAndPlay.Map;
using SGI.Areas.PlugAndPlay.Models;
using SGI.Models;
using SGI.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Z.EntityFramework.Plus;
using System.IO;

namespace SGI.Context
{
    public class JSgi : DbContext
    {
        public JSgi()
            : base("PlayConect")
        {
            Database.SetInitializer<JSgi>(null);
            Database.Log = GravaLog;
            //filtra no contexto global da aplicação os feed backs com grupo -1 | isso é feito para ignorar os registros("Fakes") gerados pelas funcoes do banco
            //QueryFilterManager.Filter<Feedback>(q => q.Where(x => x.Grupo != -1));
            //QueryFilterManager.Filter<Ocorrencia>(o => o.Where(x => x.Spr != 1));
            //System.Data.Entity.Infrastructure.Interception.DbInterception.Add(new CommandInterceptor());
        }

        public void GravaLog(string sql)
        {
            /*StreamWriter vWriter = new StreamWriter(@"c:\Play\sql_GERAL.txt", true);
            vWriter.WriteLine(sql);
            vWriter.Flush();
            vWriter.Close();*/
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Database.Log = (query) => Debug.Write("\n\n SQL GERADO \n\n" + query + "\n\n");
            // Remove unused conventions
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            #region EstruturaIndicadores
            modelBuilder.Configurations.Add(new T_Auditoria_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_Grupo_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_Configuracoes_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_Departamentos_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_Grupo_Indicador_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_Indicadores_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_Indicadores_Departamentos_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_Informacoes_Complementares_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_Medicoes_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_MESES_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_Metas_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_Negocio_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_Perfil_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_PlanoAcao_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_Tabela_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_Unidade_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_UNIUSER_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_USER_GRUPO_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_Usuario_ResultConfiguration());
            modelBuilder.Configurations.Add(new T_Favoritos_ResultConfiguration());
            modelBuilder.Configurations.Add(new vw_SGI_PARAMETRO_RELMEDICOES_ResultConfiguration());

            #endregion EstruturaIndicadores

            #region EstruturaDre
            modelBuilder.Configurations.Add(new Tr_Movimentos_ResultConfiguration());
            modelBuilder.Configurations.Add(new Tr_PlanoContas_ResultConfiguration());
            modelBuilder.Configurations.Add(new Tr_Unidade_ResultConfiguration());
            modelBuilder.Configurations.Add(new TR_CABVISAO_ResultConfiguration());
            modelBuilder.Configurations.Add(new Tr_Visoes_ResultConfiguration());
            #endregion EstruturaDre

            #region EstruturaPlugAndPlay
            modelBuilder.Configurations.Add(new CalendarioMap());
            modelBuilder.Configurations.Add(new ItensCalendarioMap());
            modelBuilder.Configurations.Add(new ColaboradorMap());
            modelBuilder.Configurations.Add(new EstruturaProdutoMap());
            modelBuilder.Configurations.Add(new MaquinaMap());
            modelBuilder.Configurations.Add(new FeedbackMap());
            modelBuilder.Configurations.Add(new MovimentoEstoqueMap());
            modelBuilder.Configurations.Add(new OcorrenciaMap());
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new ProdutoMap());
            modelBuilder.Configurations.Add(new RoteiroMap());
            modelBuilder.Configurations.Add(new TargetProdutoMap());
            modelBuilder.Configurations.Add(new TipoOcorrenciaMap());
            modelBuilder.Configurations.Add(new TurmaMap());
            modelBuilder.Configurations.Add(new TurnoMap());
            modelBuilder.Configurations.Add(new UnidadeMedidaMap());
            modelBuilder.Configurations.Add(new ViewClpMedicoesMap());
            modelBuilder.Configurations.Add(new GrupoMaquinaMap());
            modelBuilder.Configurations.Add(new FilaProducaoMap());
            modelBuilder.Configurations.Add(new ClpMedicoesMap());
            modelBuilder.Configurations.Add(new CorConfiguracaoGraficoMap());
            modelBuilder.Configurations.Add(new ViewFilaProducaoMap());
            modelBuilder.Configurations.Add(new ClienteMap());
            modelBuilder.Configurations.Add(new ViewFeedbackMap());
            modelBuilder.Configurations.Add(new MensagemMap());
            modelBuilder.Configurations.Add(new ParamMap());


            //modelBuilder.Configurations.Add(new IndicadorFatoDimensaoMap());
            //modelBuilder.Configurations.Add(new IndicadorPeriodoDimensaoMap());
            //modelBuilder.Configurations.Add(new IndicadorDimensaoMap());
            #endregion
        }



        #region Indiadores
        public DbSet<T_Auditoria> T_Auditoria { get; set; }
        public DbSet<T_Configuracoes> T_Configuracoes { get; set; }
        public DbSet<T_Grupo> T_Grupo { get; set; }
        public DbSet<T_Departamentos> T_Departamentos { get; set; }
        public DbSet<T_Grupo_Indicador> T_Grupo_Indicador { get; set; }
        public DbSet<T_Indicadores> T_Indicadores { get; set; }
        public DbSet<T_Indicadores_Departamentos> T_Indicadores_Departamentos { get; set; }
        public DbSet<T_Informacoes_Complementares> T_Informacoes_Complementares { get; set; }
        public DbSet<T_Medicoes> T_Medicoes { get; set; }
        public DbSet<T_MESES> T_MESES { get; set; }
        public DbSet<T_Metas> T_Metas { get; set; }
        public DbSet<T_Negocio> T_Negocio { get; set; }
        public DbSet<T_Perfil> T_Perfil { get; set; }
        public DbSet<T_PlanoAcao> T_PlanoAcao { get; set; }
        public DbSet<T_Tabela> T_Tabela { get; set; }
        public DbSet<T_Unidade> T_Unidade { get; set; }
        public DbSet<T_UNIUSER> T_UNIUSER { get; set; }
        public DbSet<T_USER_GRUPO> T_USER_GRUPO { get; set; }
        public DbSet<T_Usuario> T_Usuario { get; set; }
        public DbSet<T_Favoritos> T_Favoritos { get; set; }
        public DbSet<vw_SGI_PARAMETRO_RELMEDICOES> vw_SGI_PARAMETRO_RELMEDICOES { get; set; }
        #endregion Indiadores

        #region DRE
        public DbSet<Tr_PlanoContas> Tr_PlanoContas { get; set; }
        public DbSet<Tr_Unidade> Tr_Unidade { get; set; }
        public DbSet<Tr_Movimentos> Tr_Movimentos { get; set; }
        public DbSet<Tr_CabViscao> Tr_CabViscao { get; set; }
        public DbSet<Tr_Visoes> Tr_Visoes { get; set; }
        #endregion DRE
        #region PlugAndPlay
        public DbSet<Calendario> Calendario { get; set; }
        public DbSet<ItensCalendario> ItensCalendario { get; set; }
        public DbSet<Colaborador> Colaborador { get; set; }
        public DbSet<EstruturaProduto> EstruturaProduto { get; set; }
        public DbSet<Maquina> Maquina { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<MovimentoEstoque> MovimentoEstoque { get; set; }
        public DbSet<Ocorrencia> Ocorrencia { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Produto> Produto { get; set; }
        public DbSet<Roteiro> Roteiro { get; set; }
        public DbSet<TargetProduto> TargetProduto { get; set; }
        public DbSet<TipoOcorrencia> TipoOcorrencia { get; set; }
        public DbSet<Turma> Turma { get; set; }
        public DbSet<Turno> Turno { get; set; }
        public DbSet<UnidadeMedida> UnidadeMedida { get; set; }
        public DbSet<ViewClpMedicoes> ViewClpMedicoes { get; set; }
        public DbSet<GrupoMaquina> GrupoMaquina { get; set; }
        public DbSet<FilaProducao> FilaProducao { get; set; }
        public DbSet<ClpMedicoes> ClpMedicoes { get; set; }
        public DbSet<CorConfiguracaoGrafico> CorConfiguracaoGrafico { get; set; }
        public DbSet<ViewFilaProducao> VwFilaProducao { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<ViewFeedback> ViewFeedbacks { get; set; }
        public DbSet<V_MOTIVO_FEEDBACKS_DESEMPENHO> V_MOTIVOS_FEEDBACKS { get; set; }
        public DbSet<Mensagem> Mensagens { get; set; }
        public DbSet<Param> Parametros { get; set; }

        //public DbSet<Dimensao> IndicadorDimensao { get; set; }  
        //public DbSet<Fato> IndicadorFatoDimensao { get; set; }
        //public DbSet<Periodo> IndicadorPeriodoDimensao { get; set; }
        #endregion
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors) // <-- Coloque um Breakpoint aqui para conferir os erros de validação.
                {
                    System.Diagnostics.Debug.WriteLine("Entidade do tipo \"{0}\" no estado \"{1}\" tem os seguintes erros de validação:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("- Property: \"{0}\", Erro: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (DbUpdateException e)
            {
                foreach (var eve in e.Entries)
                {
                    System.Diagnostics.Debug.WriteLine("Entidade do tipo \"{0}\" no estado \"{1}\" tem os seguintes erros de validação:",
                        eve.Entity.GetType().Name, eve.State);
                }
                throw;
            }
            catch (SqlException s)
            {
                System.Diagnostics.Debug.WriteLine("- Message: \"{0}\", Data: \"{1}\"",
                            s.Message, s.Data);
                throw;
            }
        }
    }
}