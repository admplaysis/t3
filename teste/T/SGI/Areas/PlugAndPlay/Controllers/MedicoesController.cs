using SGI.Areas.PlugAndPlay.Models;
using SGI.Context;
using SGI.Controllers_Custom;
using SGI.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Z.EntityFramework.Plus;
using SGI.EntityExtensions;
using System.Data.Common;
using SGI.classes;

namespace SGI.Areas.PlugAndPlay.Controllers
{
    [CustomAuthorize(Roles = "Gerente, Operador, AdiminstradorTI, AdiminstradorPCP")]
    public class MedicoesController : BaseController
    {
        private JSgi bd = new JSgi();
        #region Feedbacks de producao
        // GET: PlugAndPlay/Medicoes
        public ActionResult Index(string id, string ip)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {

                GlobalFunctons.SendMensagem("SP_PLUG_SET_FASES_CLP_PENDENTES" + id, "PENDENTE");
                GlobalFunctons.SetPrimeiraProducao(bd, id);
                GlobalFunctons.SendMensagem("REFRESH_TELA_OPERADOR_INTERFACE_FILAS" + id, "PENDENTE");

                ViewBag.maquinaId = id;
                ViewBag.maquinaIp = ip;
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            return View();
        }
        [HttpPost]
        public ActionResult ObterMedicoes(string maquinaId)
        {
            ActionResult retorno = null;
            object filaProducao = new List<object>();
            object motivosPadada = new List<object>();
            object motivosProduzindo = new List<object>();
            object turnos = new List<object>();
            object turmas = new List<object>();
            List<ViewClpMedicoes> medicoes = new List<ViewClpMedicoes>();
            try
            {
                filaProducao = bd.VwFilaProducao.Where(f => f.RotMaqId == maquinaId)
                    .Select(x => new
                    {
                        opId = x.OrdId,
                        segTransformacao = x.RotSeqTransformacao,
                        seqRepeticao = x.FprSeqRepeticao,
                        proId = x.PaProId,
                        pecasPulso = x.RotQuantPecasPulso
                    }).ToList();

                motivosPadada = bd.Ocorrencia.Where(m => m.TipoOcorrenciaId == 1 || m.TipoOcorrenciaId == 2 && m.Spr != 1).Select(x => new
                {
                    Id = x.Id,
                    Descricao = x.Descricao,
                    Tipo = x.TipoOcorrenciaId
                }).ToList();
                motivosProduzindo = bd.Ocorrencia.Where(m => m.TipoOcorrenciaId == 5 && m.Spr != 1).Select(x => new
                {
                    Id = x.Id,
                    Descricao = x.Descricao,
                    Tipo = x.TipoOcorrenciaId
                }).ToList();
                turnos = bd.Turno.Select(t => new { Id = t.Id, Descricao = t.Descricao }).ToList();
                turmas = bd.Turma.Select(t => new { Id = t.Id, Descricao = t.Descricao }).ToList();

                medicoes = bd.ViewClpMedicoes
                      .Where(x => x.MaquinaId == maquinaId && x.FeedBackIdMov == -1)
                      .OrderBy(x => x.Grupo).ToList();
            }
            catch (Exception e)
            {
                retorno = new HttpStatusCodeResult(500);
            }
            retorno = Json(new
            {
                medicoes = ConvertToAjaxMedicoes(medicoes),
                fila = filaProducao,
                motivosPadada,
                motivosProduzindo,
                turnos = turnos,
                turmas = turmas
            });
            return retorno;
        }
        [HttpPost]
        public ActionResult ObterMedicoesTempoReal(string maquinaId, double ultimoGrupo)
        {
            List<ViewClpMedicoes> medicoes = bd.ViewClpMedicoes
                  .Where(x => x.MaquinaId == maquinaId
                  && x.Grupo >= ultimoGrupo)
                  .OrderBy(x => x.Grupo).ToList();
            return Json(new
            {
                medicoes = ConvertToAjaxMedicoes(medicoes)
            });
        }
        [HttpPost]
        public ActionResult GravarMedicoes(string medicaoJson)
        {
            List<string> msg = new List<string>();
            bool ok = true;
            Feedback medicao = new JavaScriptSerializer().Deserialize<Feedback>(medicaoJson);
            using (var db = new JSgi())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Entry(medicao);
                        //rever view fila
                        var PrimeiraOp = db.VwFilaProducao.Where(f => f.RotMaqId == medicao.MaquinaId && f.Produzindo == 1).OrderBy(f => f.FprDataInicioPrevista).Take(1).FirstOrDefault();
                        medicao.ProdutoId = PrimeiraOp.PaProId;
                        medicao.OrderId = PrimeiraOp.OrdId;
                        medicao.SequenciaTransformacao = PrimeiraOp.RotSeqTransformacao;
                        medicao.SequenciaRepeticao = PrimeiraOp.FprSeqRepeticao;
                        medicao.QuantidadePecasPorPulso = PrimeiraOp.RotQuantPecasPulso;

                        medicao.UsuarioId = User.Id;//Pegar da seçao depois
                        var proxMedicao = db.ViewClpMedicoes
                                .Where(x => x.MaquinaId == medicao.MaquinaId && x.Grupo > medicao.Grupo)
                                .OrderBy(x => x.Grupo).Take(1).FirstOrDefault();
                        var antMedicao = db.ViewClpMedicoes
                                .Where(x => x.MaquinaId == medicao.MaquinaId && x.Grupo < medicao.Grupo)
                                .OrderByDescending(x => x.Grupo).Take(1).FirstOrDefault();
                        //validar se o turno nao é nulo para em feedbacks com motivos do tipo 1 ou 5
                        int grupo = db.Ocorrencia.Where(t => t.Id == medicao.OcorrenciaId).Select(t => t.TipoOcorrenciaId).FirstOrDefault();
                        if ((grupo == 1 || grupo == 5) && (medicao.TurnoId == null || medicao.TurmaId == null))
                        {
                            ok = false;
                            msg.Add("Selecione a turma e o turno.");
                        }
                        if (antMedicao != null)
                        {
                            int cont = db.ViewFeedbacks.Count(x => x.MaquinaId == antMedicao.MaquinaId && x.Grupo == antMedicao.Grupo);
                            if (cont < 1)
                            {
                                msg.Add("Não é possível salvar este feedback sem salvar os feedbacks anteriores.");
                                ok = false;
                            }
                        }
                        if (ok)
                        {
                            /*db.Database.ExecuteSqlCommand(@"UPDATE T_CLP_MEDICOES SET STATUS = 1, 
                                                            FASE = CASE WHEN @OCORRENCIA = '5.1' AND FASE <> 2 THEN 3 WHEN (@OCORRENCIA = '1.2' OR (@OCORRENCIA = '5.1' AND FASE = 2)) THEN 2  WHEN @OCORRENCIA = '1.1' THEN 1 ELSE 0 END 
                                                            WHERE ID IN(SELECT ID FROM T_CLP_MEDICOES (NOLOCK) WHERE MAQUINA_ID = @MAQUINA_ID AND GRUPO = @GRUPO)"
                                               , new SqlParameter("GRUPO", medicao.Grupo)
                                               , new SqlParameter("OCORRENCIA", medicao.OcorrenciaId)

                                               , new SqlParameter("MAQUINA_ID", medicao.MaquinaId));
                                               */

                            if (medicao.Id == 0 && db.ViewFeedbacks.Count(f => f.MaquinaId == medicao.MaquinaId && f.Grupo == medicao.Grupo) == 0)
                            {
                                db.Entry(medicao).State = EntityState.Added;
                            }
                            else
                                db.Entry(medicao).State = EntityState.Modified;

                            if (db.SaveChanges() < 1)
                                ok = false;

                            tran.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        ok = false;
                        if (tran != null)
                            tran.Rollback();
                        msg.Add("Ocorreu um erro ao salvar o feedback.");
                    }
                }
            }
            return Json(new
            {
                ok = ok,
                id = medicao.Id,
                msg = msg
            });
        }
        [HttpPost]
        public ActionResult CancelarFeedback(int medId, string maquina, double grupo)
        {
            bool ok = true;
            List<string> msgError = new List<string>();
            using (DbContextTransaction tran = bd.Database.BeginTransaction())
            {
                try
                {
                    var medGravada = bd.Feedback.Find(medId);
                    if (medId == 0 || medGravada == null)
                    {
                        ok = false;
                        msgError.Add("Não é possível excluir um feedback que ainda não foi salvo.");
                    }
                    else if (bd.ViewFeedbacks
                            .Where(x => x.MaquinaId == maquina && x.Grupo > grupo).Count() > 0)
                    {
                        ok = false;
                        msgError.Add("Os feedbacks devem ser excluidos na mesma sequência que foram salvos.");
                    }
                    if (ok)
                    {
                        bd.Database.ExecuteSqlCommand(@"UPDATE T_CLP_MEDICOES SET STATUS = 0 
                                                        WHERE ID IN(SELECT ID FROM T_CLP_MEDICOES (NOLOCK) 
                                                                 WHERE MAQUINA_ID = {0} AND GRUPO = {1})",
                                                    maquina, grupo);
                        bd.Feedback.Remove(medGravada);
                        bd.SaveChanges();
                    }
                    tran.Commit();
                }
                catch (Exception e)
                {
                    ok = false;
                    tran.Rollback();
                }
            }
            return Json(new { ok = ok, msg = msgError });
        }
        [HttpPost]
        public ActionResult DesfazerMedicao(int medId, string maquina, double grupo)
        {
            bool ok = true; double grupoInteiro = Math.Floor(grupo);
            List<string> msgError = new List<string>();
            using (var db = new JSgi())
            {
                try
                {
                    using (DbContextTransaction tran = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //var feedbacksPatIntGrupo = bd.Feedback
                            //    .Where(f => f.MaquinaId == maquina && Math.Floor(f.Grupo) == grupoInteiro)
                            //    .Select(f=> new {
                            //        f.MaquinaId, f.ProdutoId, f.OrderId, f.SequenciaRepeticao, f.SequenciaTransformacao
                            //    }).ToList();
                            if (grupoInteiro == grupo)
                            {
                                ok = false;
                                msgError.Add("Não é possível desfazer um período que não foi modificado.");
                            }
                            if (db.ViewFeedbacks.Count(f => f.MaquinaId == maquina && f.Grupo == grupo) > 0)
                            {
                                ok = false;
                                msgError.Add("Remova o feedback antes de desfazer a divisão do periodo.");
                            }
                            //var opAtual = bd.VwFilaProducao.Where(f => f.RotMaqId == maquina).OrderBy(f => f.FprDataInicioPrevista).Take(1).FirstOrDefault();
                            if (db.ViewFeedbacks.Count(f => Math.Floor(f.Grupo) == grupoInteiro && f.MaquinaId == maquina && f.FeeIdMovEstoque != null) > 0)
                            {
                                ok = false;
                                msgError.Add("Não é possível desfazer este período sem desfazer a OP produzida anteriormente.");
                            }
                            if (ok)
                            {
                                var medicoes = db.ClpMedicoes.Where(m => m.MaquinaId == maquina && Math.Floor((double)m.Grupo) == grupoInteiro && m.Status != 9).Delete();
                                var feedbacks = db.Feedback.Where(f => f.MaquinaId == maquina && Math.Floor(f.Grupo) == grupoInteiro).Delete();
                                db.ClpMedicoes
                                    .Where(c => c.MaquinaId == maquina && Math.Floor((double)c.Grupo) == grupoInteiro)
                                    .Update(c => new ClpMedicoes()
                                    {
                                        Status = 0,
                                        Grupo = grupoInteiro
                                    });
                                tran.Commit();
                            }
                        }
                        catch (Exception e)
                        {
                            ok = false;
                            tran.Rollback();
                            msgError.Add("Ocorreu um erro ao desfazer o feedback");
                        }
                    }
                }
                catch (Exception e)
                {

                }

            }

            return Json(new
            {
                ok = ok,
                msg = msgError
            });
        }
        [HttpPost]
        public ActionResult DividirPeriodo(List<ClpMedicoes> medicoes)
        {
            List<string> msg = new List<string>();
            bool ok = true;
            double? grupo = medicoes[0].Grupo;
            string maquina = medicoes[0].MaquinaId;
            double aux = 0.1;
            DbContextTransaction trans = null;

            for (int i = 0; i < medicoes.Count; i++)
            {
                if (medicoes[i].DataFim <= medicoes[i].DataInicio)
                    ok = false;
            }
            var medicao = bd.ViewClpMedicoes.Where(m => m.MaquinaId == maquina && m.Grupo == grupo).Select(m => new { m.Quantidade, m.Grupo }).FirstOrDefault();
            if (medicao.Quantidade > 0)
            {
                ok = false;
                msg.Add("Não é permitido dividir um periodo com quantidade maior que 0.");
            }
            if (Math.Floor((double)medicao.Grupo) != grupo)
            {
                ok = false;
                msg.Add("Não é permitido dividir um periodo dividido anteriormente.");
            }
            if (bd.ViewClpMedicoes.Count(m => m.MaquinaId == maquina && m.Grupo > grupo) == 0)
            {
                ok = false;
                msg.Add("Aguarde o encerramento do periodo.");
            }
            if (bd.ViewFeedbacks.Count(f => f.MaquinaId == maquina && f.Grupo == grupo) > 0)
            {
                ok = false;
                msg.Add("Não é permitido dividir um periodo em que o feedback se encontra salvo.");
            }
            if (ok)
            {
                using (trans = bd.Database.BeginTransaction())
                {
                    try
                    {
                        bd.Database.ExecuteSqlCommand(@"UPDATE T_CLP_MEDICOES SET GRUPO = @GRUPODEC, STATUS = @STATUS
                                                        WHERE ID IN (SELECT ID FROM T_CLP_MEDICOES (NOLOCK) 
                                                                 WHERE MAQUINA_ID = @MAQUINA_ID AND GRUPO = @GRUPO) "
                                                            , new SqlParameter("GRUPODEC", (grupo + 0.999))
                                                            , new SqlParameter("MAQUINA_ID", maquina)
                                                            , new SqlParameter("GRUPO", grupo)
                                                            , new SqlParameter("@STATUS", 9));
                        foreach (var m in medicoes)
                        {
                            m.IdLoteClp = -1;
                            m.Grupo = grupo + aux;
                            m.Status = 0;
                            bd.ClpMedicoes.Add(m);
                            aux = aux + 0.1;
                        }
                        int cont = bd.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        if (trans != null)
                            trans.Rollback();
                        ok = false;
                    }
                }
            }
            return Json(new { status = ok, msgError = msg });
        }
        #endregion
        #region Feedbacks de quantidade
        [HttpGet]
        public ActionResult FeedbackQuantidade(string order, string maq, int seqTran, int seqRep, string produto, string url)
        {
            //ATUALIZA quabtidade dos feedbacks pois a quantidade pode nao estar batendo com a clp medicao 
            string sql = "  UPDATE T_FEEDBACK SET FEE_DIA_TURMA = dbo.DIATURMA(DTI),FEE_DATA_INICIAL = DTI,FEE_DATA_FINAL = DTF, FEE_QTD_PULSOS = QTD FROM T_FEEDBACK (nolock) " +
                        "   INNER JOIN(SELECT GRUPO, MAQUINA_ID, MIN(DATA_INI) DTI, MAX(DATA_FIM) DTF, SUM(QTD) QTD  FROM V_CLP_MEDICOES_PENDENTES (NOLOCK) " +
                        "   GROUP BY GRUPO, MAQUINA_ID ) AS CLP ON GRUPO = FEE_GRUPO AND MAQUINA_ID = MAQ_ID " +
                        "   WHERE MAQ_ID = '"+maq+"'";
            bd.Database.ExecuteSqlCommand(sql);

            ViewBag.urlAnterior = HttpUtility.UrlDecode(url);
            if (!string.IsNullOrEmpty(order)
                && !string.IsNullOrEmpty(maq)
                && !string.IsNullOrEmpty(produto))
            {
                var feedbacks = bd.ViewFeedbacks.Where(f => f.MaquinaId == maq && f.FeeIdMovEstoque == null)
                .Select(f => new
                {
                    quantidade = f.QuantidadePecasPorPulso * f.QuantidadePulsos,
                    id = f.Id
                }).ToList();
                if (feedbacks.Count > 0)
                {
                    var qtdProdOpAtual = feedbacks.Sum(f => f.quantidade); //quantidade produzida na parte da op que esta sendo confirmada
                    double qtdProdOpTotal = bd.MovimentoEstoque.Where(m => m.MaquinaId == maq // quantidade total baseada em todos os movimentos salvos da op
                    && m.OrderId == order && m.SequenciaRepeticao == seqRep
                    && m.SequenciaTransformacao == seqTran && m.Tipo == "001")
                    .Sum(m => (Double?)m.Quantidade) ?? 0;

                    ViewBag.qtdProdOpTotal = qtdProdOpTotal;

                    ViewBag.sltMotivos = new SelectList(bd.Ocorrencia.ToList(), "Id", "Descricao");
                    ViewBag.quantidade = qtdProdOpAtual;
                    ViewBag.produto = bd.Produto.Find(produto);
                    ViewBag.op = order + produto + seqTran + seqRep;
                    ViewBag.maquina = bd.Maquina.Where(m => m.Id == maq).Select(m => m.Descricao).FirstOrDefault();
                    ViewBag.pedido = order;
                    ViewBag.feedbacks = feedbacks.Select(x => new { Id = x.id });
                    ViewBag.maquinaId = maq;
                    ViewBag.seqTran = seqTran;
                    ViewBag.seqRep = seqRep;
                    //quantidades
                    var op = bd.FilaProducao.Include(f => f.Order).Where(f => f.MaquinaId == maq
                         && f.OrderId == order && f.ProdutoId == produto
                         && f.SequenciaTransformacao == seqTran
                         && f.SequenciaRepeticao == seqRep).Select(f => new { f.QuantidadePrevista, f.Order.ToleranciaMenos }).FirstOrDefault();
                    ViewBag.qtdMinPrevista = op.QuantidadePrevista - op.ToleranciaMenos;
                    var ocorrencias = bd.Ocorrencia.Where(o => o.TipoOcorrenciaId == 6).ToList();
                    var dlOcorrenciaOpParcial = new List<SelectListItem>();
                    foreach (var o in ocorrencias)
                    {
                        dlOcorrenciaOpParcial.Add(new SelectListItem()
                        {
                            Value = o.Id,
                            Text = o.Descricao
                        });
                    }
                    ViewBag.ddlOcorrenciaOpParcial = dlOcorrenciaOpParcial;
                }
                else
                {
                    throw new HttpException(500, "Bad Request");
                }
            }
            else
            {
                throw new HttpException(500, "Bad Request");
            }
            return View();
        }
        //public ActionResult ObterFeedbacksAgupOp(string maqId)
        //{
        //    List<Feedback> feedbacks = bd.Feedback.Include(i => i.Produto).Where(f => f.MaquinaId == maqId && !string.IsNullOrEmpty(f.OrderId) && !string.IsNullOrEmpty(f.TurnoId) && f.Grupo != -1)/*.Where(f=>f.MovimentosEstoque.Count() == 0)*/.ToList();
        //    var gupos = feedbacks.GroupBy(g => new { g.MaquinaId, g.OrderId, g.TurnoId, g.SequenciaTransformacao, g.SequenciaRepeticao, g.Produto })
        //                                          .Select(g => new
        //                                          {
        //                                              op = g.Key.OrderId,
        //                                              produtoId = g.Key.Produto.Id,
        //                                              produtoDescricao = g.Key.Produto.Descricao,
        //                                              inicio = g.Min(x => x.DataInicial).ToString(),
        //                                              fim = g.Max(x => x.Datafinal).ToString(),
        //                                              quantidade = g.Sum(x => x.QuantidadePulsos * x.QuantidadePecasPorPulso),
        //                                              seqTranId = g.Key.SequenciaTransformacao,
        //                                              seqRepId = g.Key.SequenciaRepeticao,
        //                                              turno = g.Key.TurnoId,
        //                                              maqId = g.Key.MaquinaId
        //                                          }).ToList();
        //    return Json(gupos);
        //}
        [HttpPost]
        public ActionResult GravarApontamentosProducao(List<MovimentoEstoque> movimentos, string filaStatus)
        {
            bool ok = true; int movId = 0; List<string> msg = new List<string>();
            using (var db = new JSgi())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        string proId = movimentos[0].ProdutoId;
                        string maquinaId = movimentos[0].MaquinaId;
                        var viewFeedback = bd.ViewFeedbacks.Where(f => f.MaquinaId == maquinaId && f.FeeIdMovEstoque == null).ToList();

                        var dbFeeds = new List<Feedback>();
                        foreach (var f in viewFeedback)
                        {
                            dbFeeds.Add(db.Feedback.Find(f.Id));
                        }
                        string queryFeeIds = string.Join(",", dbFeeds.Select(f => f.Id).ToList());
                        int feeMovCont = db.Database.SqlQuery<int>(string.Format("Select Count(*) from T_FEEDBACK_MOV_ESTOQUE where FEE_ID in ({0})", queryFeeIds)).FirstOrDefault();
                        if (feeMovCont == 0)
                        {
                            DateTime dataCriacao = dbFeeds.Max(f => f.Datafinal);
                            string ordId = dbFeeds[0].OrderId;
                            string maqId = dbFeeds[0].MaquinaId;
                            int seqTran = (int)dbFeeds[0].SequenciaTransformacao;
                            int seqRep = (int)dbFeeds[0].SequenciaRepeticao;

                            foreach (var m in movimentos)
                            {
                                m.OrderId = ordId;
                                m.MaquinaId = maqId;
                                m.SequenciaTransformacao = seqTran;
                                m.SequenciaRepeticao = seqRep;
                                m.UsuarioId = Convert.ToInt32(HttpContext.User.Identity.Name);
                                m.DataHoraCriacao = dataCriacao;
                                m.DataHoraEmissao = dataCriacao;
                                m.Feedbacks = dbFeeds;
                                m.TurmaId = dbFeeds[0].TurmaId;
                                m.TurnoId = dbFeeds[0].TurnoId;
                                db.MovimentoEstoque.Add(m);
                            }
                            if (filaStatus != null)
                            {
                                var fila = new FilaProducao()
                                {
                                    MaquinaId = movimentos[0].MaquinaId,
                                    OrderId = movimentos[0].OrderId,
                                    ProdutoId = movimentos[0].ProdutoId,
                                    SequenciaTransformacao = movimentos[0].SequenciaTransformacao,
                                    SequenciaRepeticao = movimentos[0].SequenciaRepeticao,
                                    Status = filaStatus
                                };
                                db.FilaProducao.Attach(fila);
                                db.Entry(fila).Property(f => f.Status).IsModified = true;
                            }

                            db.SaveChanges();
                            movId = movimentos.Where(x => x.Tipo == "001").Select(x => x.Id).FirstOrDefault();
                            var cont = db.ViewFeedbacks.Where(f => f.FeeIdMovEstoque == null && f.MaquinaId == maqId).Count();
                            if (cont == 0)
                            {
                                GlobalFunctons.SendMensagem("SP_PLUG_INTERFACE_APONTA_PRODUCAO", "PENDENTE");




                                tran.Commit();



                            }
                            else
                            {
                                msg.Add("Ocorreu um erro ao salvar os dados.");
                                tran.Rollback();
                            }
                        }
                        else
                        {
                            msg.Add("Ocorreu um erro ao salvar os dados porque os feedbacks para esta ordem de produção já foram realizados.");
                        }
                    }
                    catch (Exception e)
                    {
                        msg.Add("Ocorreu um erro ao salvar os dados.");
                        tran.Rollback();
                        ok = false;
                        msg.Add("Ocorreu um erro ao salvar os dados.");
                    }
                }
            }
            return Json(new
            {
                ok = ok,
                id = movId
            });
        }
        //public ActionResult ObterMovimentosEstoque(string orderId, string maqId, int seqRep, int seqTran, string proId)
        //{
        //    object movimentos = new List<object>();
        //    try
        //    {
        //        movimentos = bd.MovimentoEstoque
        //                .Include(m => m.Ocorrencia).Include(m => m.Produto)
        //                .Where(m => m.OrderId == orderId && m.MaquinaId == maqId && m.SequenciaRepeticao == seqRep && m.SequenciaTransformacao == seqTran && m.ProdutoId == proId)
        //                .Select(m => new
        //                {
        //                    id = m.Id,
        //                    quantidade = m.Quantidade,
        //                    observacao = m.Observacao == null ? "" : m.Observacao,
        //                    produtoId = m.Produto.Id,
        //                    produtoDescricao = m.Produto.Descricao,
        //                    ordemProducaoId = m.OrderId,
        //                    tipo = m.Tipo,
        //                    ocorrenciaId = m.OcorrenciaId,
        //                    ocorrenciaDescricao = m.Ocorrencia.Descricao,
        //                    maquinaId = m.MaquinaId
        //                }).ToList();
        //    }
        //    catch (Exception e)
        //    {
        //        return new HttpStatusCodeResult(500);
        //    }
        //    return Json(movimentos);
        //}
        public ActionResult ObterProdutos(string pesquisa)
        {
            var produtos = bd.Produto.Where(x => x.Descricao.Contains(pesquisa) || x.Id.Contains(pesquisa)).Take(20)
                .Select(x => new
                {
                    id = x.Id,
                    descricao = x.Descricao
                }).ToList();
            return Json(produtos);
        }
        #endregion
        #region Feedbacks de performace
        public ActionResult FeedbackPerformace(int movId, string url)
        {
            TargetProduto target;
            using (var db = new JSgi())
            {
                var movimento = db.MovimentoEstoque.Include(p => p.Produto).Include(m => m.Maquina).Where(m => m.Id == movId).FirstOrDefault();
                var qtdPerdaProduto = db.MovimentoEstoque.Where(m => m.MaquinaId == movimento.MaquinaId
                    && m.OrderId == movimento.OrderId && m.ProdutoId == movimento.ProdutoId
                    && m.SequenciaTransformacao == movimento.SequenciaTransformacao
                    && m.SequenciaRepeticao == movimento.SequenciaRepeticao
                    && m.Tipo == "501")
                    .Select(m => m.Quantidade).FirstOrDefault();

                var ocorrencias = db.Ocorrencia
                         .Where(o => o.TipoOcorrenciaId == 3 || o.TipoOcorrenciaId == 4 || o.TipoOcorrenciaId == 6).ToList();
                var sltOcoAltaPerform = new List<SelectListItem>();
                var sltOcoBaixaPerform = new List<SelectListItem>();
                var sliOcoOpParcial = new List<SelectListItem>();
                foreach (var o in ocorrencias)
                {
                    if (o.TipoOcorrenciaId == 3)
                    {
                        sltOcoBaixaPerform.Add(new SelectListItem()
                        {
                            Value = o.Id,
                            Text = o.Descricao
                        });
                    }
                    else if (o.TipoOcorrenciaId == 4)
                    {
                        sltOcoAltaPerform.Add(new SelectListItem()
                        {
                            Value = o.Id,
                            Text = o.Descricao
                        });
                    }
                    else if (o.TipoOcorrenciaId == 6)
                    {
                        sliOcoOpParcial.Add(new SelectListItem()
                        {
                            Value = o.Id,
                            Text = o.Descricao
                        });
                    }
                }
                //var op = db.FilaProducao.Include(f=>f.Order).Where(f => f.MaquinaId == movimento.MaquinaId
                //   && f.OrderId == movimento.OrderId && f.ProdutoId == movimento.ProdutoId
                //   && f.SequenciaTansformacao == movimento.SequenciaTranformacao
                //   && f.SequencaRepeticao == movimento.SequenciaRepeticao).Select(f => new { f.QuantidadePrevista, ToleranciaMenos = f.Order.ToleranciaMenos }).FirstOrDefault();
                //selects list item
                ViewBag.sltOcoAltaPerform = sltOcoAltaPerform;
                ViewBag.sltOcoBaixaPerform = sltOcoBaixaPerform;
                ViewBag.sliOcoOpParcial = sliOcoOpParcial;
                //informações
                ViewBag.op = movimento.OrderId + movimento.ProdutoId + movimento.SequenciaTransformacao + movimento.SequenciaRepeticao;
                ViewBag.movimento = movimento;
                //quantidades
                //ViewBag.qtdMinPrevista = op.QuantidadePrevista - op.ToleranciaMenos;
                //ViewBag.qtdPecaBoaProduzida = movimento.Quantidade - qtdPerdaProduto;
                db.Database.ExecuteSqlCommand("EXEC [SP_PLUG_TARGETS] " + movimento.Id + ",'" + movimento.ProdutoId + "','" + movimento.MaquinaId + "'");
                target = db.TargetProduto.Where(t => t.MovimentoEstoqueId == movId).OrderByDescending(t => t.MovimentoEstoqueId).Take(1).FirstOrDefault();
                var targetAnterior = new TargetProduto().ObterUltimaMeta(target.MaquinaId, target.ProdutoId, target.MovimentoEstoqueId, db);
                if (targetAnterior != null)
                {
                    ViewBag.target = new
                    {
                        PerformaceMinAmarelo = targetAnterior.PerformaceMinAmarelo,
                        PerformaceMinVerde = targetAnterior.PerformaceMinVerde,
                        PerformaceMaxVerde = targetAnterior.PerformaceMaxVerde,
                        SetupMinVerde = targetAnterior.SetupMinVerde,
                        SetupMaxVerde = targetAnterior.SetupMaxVerde,
                        SetupMaxAmarelo = targetAnterior.SetupMaxAmarelo,
                        SetupAjusteMinVerde = targetAnterior.SetupAjusteMinVerde,
                        SetupAjusteMaxVerde = targetAnterior.SetupAjusteMaxVerde,
                        SetupAjusteMaxAmarelo = targetAnterior.SetupAjusteMaxAmarelo,
                        RealizadoPerformace = target.RealizadoPerformace,
                        ProximaMetaPerformace = targetAnterior.ProximaMetaPerformace,
                        RealizadoTempoSetup = target.RealizadoTempoSetup,
                        ProximaMetaTempoSetup = targetAnterior.ProximaMetaTempoSetup,
                        RealizadoTempoSetupAjuste = target.RealizadoTempoSetupAjuste,
                        ProximaMetaTempoSetupAjuste = targetAnterior.ProximaMetaTempoSetupAjuste
                    };
                }
            }
            ViewBag.urlAnterior = !string.IsNullOrWhiteSpace(url) ? url : Url.Action("Index");
            return View(target);
        }
        [HttpPost]
        public ActionResult GravarFeedbackPerformace(TargetProduto target)
        {
            bool ok = true;
            using (DbContextTransaction tran = bd.Database.BeginTransaction())
            {
                try
                {
                    var tar = bd.TargetProduto.Find(target.Id);
                    tar.ObsPerformace = target.ObsPerformace;
                    tar.ObsSetup = target.ObsSetup;
                    tar.ObsSetupAjuste = target.ObsSetupAjuste;
                    tar.OcoIdPerformace = target.OcoIdPerformace;
                    tar.OcoIdSetup = target.OcoIdSetup;
                    tar.OcoIdSetupAjuste = target.OcoIdSetupAjuste;
                    //tar.MovimentoEstoqueId = target.MovimentoEstoqueId;
                    bd.SaveChanges();

                    Mensagem m = new Mensagem();
                    m.MEN_ID = "SP_PLUG_SET_FASES_CLP_PENDENTES" + tar.MaquinaId;
                    m.MEN_MENSAGEM = "PENDENTE";
                    m.AddMensagem(bd, m);
                    m = null;
                    //bd.Database.ExecuteSqlCommand("EXEC SP_PLUG_CALCULA_FILAS '" + tar.MaquinaId + "'");
                    tran.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    ok = false;
                }
            }
            return Json(ok);
        }
        #endregion
        #region Fila de Producao
        [HttpGet]
        public ActionResult ObterFilaProducao(string maquina)
        {
            object fila = new List<object>();
            object filaHistorico = new List<object>();
            object opsTarPendentes = new List<object>();
            using (JSgi db = new JSgi())
            {
                db.Database.Connection.Open();
                //Ops pendentes(abertas)
                fila = db.VwFilaProducao.Where(f => f.RotMaqId == maquina ).OrderBy(f => f.FprDataInicioPrevista).Take(15).ToList().Select(f => new
                {
                    op = f.OrdId + f.PaProId + f.RotSeqTransformacao + f.FprSeqRepeticao,
                    order = f.OrdId,
                    maqId = f.RotMaqId,
                    proId = f.PaProId,
                    proDesc = f.PcProDescricao,
                    dataInicio = f.FprDataInicioPrevista.ToString("dd/MM HH:mm"),
                    dataFim = f.FprDataFimPrevista,
                    seqTran = f.RotSeqTransformacao,
                    seqRep = f.FprSeqRepeticao,
                    obs = f.FprObsProducao,
                    qtd = f.FprQuantidadePrevista,
                    produzindo = f.Produzindo,
                }).ToList();
                //ops encerradas nos ultimos 10 dias
                DateTime diaAnterior = DateTime.Now.AddDays(-10);
                filaHistorico = (from f in db.FilaProducao
                                 join m in db.MovimentoEstoque on new
                                 {
                                     f.MaquinaId,
                                     f.ProdutoId,
                                     f.OrderId,
                                     f.SequenciaRepeticao,
                                     f.SequenciaTransformacao
                                 } equals new
                                 {
                                     m.MaquinaId,
                                     m.ProdutoId,
                                     m.OrderId,
                                     SequenciaRepeticao = (int?)m.SequenciaRepeticao,
                                     m.SequenciaTransformacao
                                 }
                                 join u in db.T_Usuario on m.UsuarioId equals u.ID_USUARIO
                                 where m.Tipo == "001" && f.MaquinaId == maquina && m.DataHoraEmissao > diaAnterior
                                 orderby new { m.Id } descending
                                 select new
                                 {
                                     f.OrderId,
                                     f.MaquinaId,
                                     f.ProdutoId,
                                     ProDesc = f.Produto.Descricao,
                                     f.DataInicioPrevista,
                                     f.DataFimPrevista,
                                     f.SequenciaTransformacao,
                                     f.SequenciaRepeticao,
                                     f.ObservacaoProducao,
                                     f.QuantidadePrevista,
                                     m.Quantidade,
                                     m.Id,
                                     m.DataHoraCriacao,
                                     u.NOME,
                                     m.TurnoId
                                 }).ToList().Select(f => new
                                 {
                                     order = f.OrderId,
                                     maqId = f.MaquinaId,
                                     proId = f.ProdutoId,
                                     proDesc = f.ProDesc.Length > 20 ? f.ProDesc.Substring(0, 19) : f.ProDesc,
                                     dataInicio = f.DataInicioPrevista,
                                     dataFim = f.DataFimPrevista,
                                     seqTran = f.SequenciaTransformacao,
                                     seqRep = f.SequenciaRepeticao,
                                     obs = f.ObservacaoProducao,
                                     qtd = f.QuantidadePrevista,
                                     qtdProduzida = f.Quantidade,
                                     movId = f.Id,
                                     dataDaProducao = f.DataHoraCriacao.ToString("dd/MM HH:mm"),
                                     usuarioNome = f.NOME.Length > 13 ? f.NOME.Substring(0, 12) : f.NOME,
                                     turno = f.TurnoId
                                 }).ToList();
                //Ops fechadas parcialmente, sem o target
                opsTarPendentes = db.Database.DynamicListFromSql(
                   @"SELECT MOV_ID as movId, 
                            MAQ_ID as maqId, 
                            ORD_ID as ordId, 
                            PRO_ID as proId, 
                            FPR_SEQ_REPETICAO as seqRep, 
                            FPR_SEQ_TRANFORMACAO as seqTran,
                            MOV_QUANTIDADE as QuantidadeProduzida,
                            NOME as UsuarioNome,
                            MOV_DATA_HORA_CRIACAO as dataDaProducao,
                            TURN_ID as TurnoId,
                            MOV_QUANTIDADE as Quantidade,
                            PRO_DESCRICAO as ProDesc
                    from V_TARGET_PENDENTES  WHERE MAQ_ID = @maquina",
                   new Dictionary<string, object> { { "maquina", maquina } }).Select(f => new
                   {
                       op = f.maqId + f.proId + f.seqTran + f.seqRep,
                       order = f.ordId,
                       maqId = f.maqId,
                       proId = f.proId,
                       seqTran = f.seqTran,
                       seqRep = f.seqRep,
                       movId = f.movId,
                       usuarioNome = f.UsuarioNome.Length > 13 ? f.UsuarioNome.Substring(0, 12) : f.UsuarioNome,
                       dataDaProducao = f.dataDaProducao.ToString("dd/MM HH:mm"),
                       turno = f.TurnoId,
                       qtdProduzida = f.Quantidade,
                       proDesc = f.ProDesc.Length > 20 ? f.ProDesc.Substring(0, 19) : f.ProDesc,
                   }).ToList();
            }
            Param Par = bd.Parametros.Find("FILA_PRODUCAO_PULA_FILA");
            double pulaFila = 1;
            if (Par != null)
            {
                pulaFila = Par.PAR_VALOR_N;
            }
            return Json(new { fila, filaHistorico, opsTarPendentes, pulaFila }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ObterObterOrdemProducao(string maqId, string proId)
        {
            var fila = bd.VwFilaProducao.ToList().Select(f => new
            {
                op = f.RotMaqId + f.OrdId + f.PaProId + f.RotSeqTransformacao + f.FprSeqRepeticao,
                maquina = f.OrdId,
                produto = f.PaProId,
                dataInicio = f.FprDataInicioPrevista,
                dataFim = f.FprDataFimPrevista,
                seqTran = f.RotSeqTransformacao,
                seqRep = f.FprSeqRepeticao,
                obs = f.FprObsProducao,
                quantidade = f.OrdQuantidade
            }).ToList();
            return Json(fila);
        }
        [HttpGet]
        public ActionResult ObterObterOrdemProducao(string maqId, string proId, string order, int seqRep, int seqTran, int movId)
        {
            object op = null;
            using (var db = new JSgi())
            {
                op = (from f in db.FilaProducao
                      join m in db.MovimentoEstoque on new
                      {
                          f.MaquinaId,
                          f.ProdutoId,
                          f.OrderId,
                          f.SequenciaRepeticao,
                          f.SequenciaTransformacao
                      } equals new
                      {
                          m.MaquinaId,
                          m.ProdutoId,
                          m.OrderId,
                          SequenciaRepeticao = (int?)m.SequenciaRepeticao,
                          m.SequenciaTransformacao
                      } into x from fm in x.DefaultIfEmpty()
                      join u in db.T_Usuario on fm.UsuarioId equals u.ID_USUARIO
                      where fm.Tipo == "001" && f.MaquinaId == maqId
                      && f.OrderId == order && f.ProdutoId == proId
                      && f.SequenciaTransformacao == seqTran && f.SequenciaRepeticao == seqRep
                      && fm.Id == movId
                      select new
                      {
                          f.OrderId,
                          f.MaquinaId,
                          f.ProdutoId,
                          ProDesc = f.Produto.Descricao,
                          f.DataInicioPrevista,
                          f.DataFimPrevista,
                          f.SequenciaTransformacao,
                          f.SequenciaRepeticao,
                          f.ObservacaoProducao,
                          f.QuantidadePrevista,
                          fm.Quantidade,
                          fm.Id,
                          fm.DataHoraCriacao,
                          u.NOME,
                          fm.TurnoId,
                          f.DataFimMaxima
                      }).ToList().Select(f => new
                      {
                          order = f.OrderId,
                          maqId = f.MaquinaId,
                          proId = f.ProdutoId,
                          proDesc = f.ProDesc.Length > 20 ? f.ProDesc.Substring(0, 19) : f.ProDesc,
                          dataInicio = f.DataInicioPrevista,
                          dataFim = f.DataFimPrevista,
                          seqTran = f.SequenciaTransformacao,
                          seqRep = f.SequenciaRepeticao,
                          obs = f.ObservacaoProducao,
                          qtdPrevista = f.QuantidadePrevista,
                          qtdProduzida = f.Quantidade,
                          movId = f.Id,
                          dataDaProducao = f.DataHoraCriacao.ToString("dd/MM HH:mm"),
                          usuarioNome = f.NOME.Length > 13 ? f.NOME.Substring(0, 12) : f.NOME,
                          turno = f.TurnoId,
                          dataFimMaxima = f.DataFimMaxima
                      }).ToList();
            }
            return Json(new { dados = op });
        }
        [HttpPost]
        public ActionResult DesfazerOpProduzida(string maquina, string produto, string order, int seqRep, int seqTran, int movId)
        {
            bool ok = true;
            //busca ids dos feedbacks salvos que estao pendentes
            using (var db = new JSgi())
            {
                db.Database.Connection.Open();
                var feedsPendenteIds = db.Database.DynamicListFromSql(@"SELECT F.FEE_ID as Id FROM T_FEEDBACK F
                                                                    LEFT JOIN T_FEEDBACK_MOV_ESTOQUE FM ON FM.FEE_ID = F.FEE_ID
                                                                    WHERE FM.FEE_ID IS NULL AND MAQ_ID = @maquina",
                                                            new Dictionary<string, object> { { "maquina", maquina } })
                                                            .ToList();
                //ids do movimento tipo = "001"
                var feedIds = db.Database.SqlQuery<int>("SELECT FEE_ID FROM T_FEEDBACK_MOV_ESTOQUE WHERE MOV_ID = {0}", movId).Distinct().ToList();
                //ids de todos os movimentos 
                var movIds = db.Database.SqlQuery<int>(String.Format("SELECT MOV_ID FROM T_FEEDBACK_MOV_ESTOQUE WHERE FEE_ID IN ({0})", String.Join(",", feedIds))).Distinct().ToList();

                //mecla os pendentes com os da OP a ser desfeita
                feedsPendenteIds.ForEach(f => feedIds.Add(f.Id));
                //busca a data da op atual
                var opAtual = db.VwFilaProducao
                    .Where(f => f.RotMaqId == maquina)
                    .OrderBy(x => x.FprDataInicioPrevista)
                    .Take(1)
                    .Select(x => new
                    {
                        x.FprDataInicioPrevista,
                        x.RotMaqId,
                        x.OrdId,
                        x.PaProId,
                        x.RotSeqTransformacao,
                        x.FprSeqRepeticao
                    }).FirstOrDefault();

                if (opAtual.RotMaqId != maquina || opAtual.OrdId != order || opAtual.PaProId != produto
                    || opAtual.FprSeqRepeticao != seqRep || opAtual.RotSeqTransformacao != seqTran)
                {
                    //abre a op na fila de producao
                    var fila = new FilaProducao()
                    {
                        MaquinaId = maquina,
                        OrderId = order,
                        ProdutoId = produto,
                        SequenciaTransformacao = seqTran,
                        SequenciaRepeticao = seqRep,
                        Status = "",
                        DataInicioPrevista = opAtual.FprDataInicioPrevista.AddMilliseconds(-10)
                    };

                    db.FilaProducao.Attach(fila);//adiciona ao contexto do entity
                    var entry = db.Entry(fila);

                    entry.Property(f => f.Status).IsModified = true;
                    entry.Property(f => f.DataInicioPrevista).IsModified = true;
                }
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.SaveChanges();
                        //gera a string separando os valores com "," para incluir na clausa "in" do sql
                        string sqlInMovIds = string.Join(", ", movIds);
                        string sqlInFeeIds = string.Join(", ", feedIds);
                        //deleta target, movimento estoque e feedback
                        db.Database.ExecuteSqlCommand(string.Format("DELETE FROM T_TARGET_PRODUTO WHERE MOV_ID IN ({0})", sqlInMovIds));
                        db.Database.ExecuteSqlCommand(string.Format("DELETE FROM T_FEEDBACK_MOV_ESTOQUE WHERE FEE_ID IN ({0})", sqlInFeeIds));
                        db.Database.ExecuteSqlCommand(string.Format("DELETE FROM T_MOVIMENTOS_ESTOQUE WHERE MOV_ID IN ({0})", sqlInMovIds));
                        db.Database.ExecuteSqlCommand(string.Format("DELETE FROM T_FEEDBACK WHERE FEE_ID IN ({0})", sqlInFeeIds));
                        tran.Commit();
                        GlobalFunctons.SendMensagem("SP_PLUG_INTERFACE_ESTORNA_PRODUCAO", "PENDENTE");

                    }
                    catch (Exception e)
                    {
                        ok = false;
                        tran.Rollback();
                    }
                }
            }
            return (Json(new { status = ok }));
        }
        [HttpGet]
        public ActionResult VerificarSeOpTemFeedback(string maquina, string produto, string order, int seqRep, int seqTran)
        {
            int cont = bd.ViewFeedbacks.Where(f => f.MaquinaId == maquina
                && f.OrderId == order && f.SequenciaRepeticao == seqRep
                && f.SequenciaTransformacao == seqTran && f.FeeIdMovEstoque == null)
                .Count();
            return Json(cont > 0 ? true : false, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult SetProduzindo(string maquina, string produto, string order, int seqRep, int seqTran)
        {
            GlobalFunctons.SetProduzindo(bd, maquina, produto, order, seqRep, seqTran);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region Monitoramento em tempo real
        [HttpGet]
        public ActionResult ObterTargetProduto(string maqId)
        {
            object opInfoJson = null, targetJson = null;
            //rever view fila
            var op = bd.VwFilaProducao.Where(f => f.RotMaqId == maqId && f.Produzindo == 1).OrderBy(f => f.FprDataInicioPrevista).Take(1).FirstOrDefault();
            if (op != null)
            {
                opInfoJson = new
                {
                    qtdProgramada = op.FprQuantidadePrevista,
                    op = op.OrdId + op.PaProId + op.RotSeqTransformacao + op.FprSeqRepeticao,
                };
                //var target = bd.TargetProduto.Where(t => t.ProdutoId == op.RotPcProId && t.MaquinaId == op.RotMaqId).OrderByDescending(x => x.Id).Take(1).FirstOrDefault();
                var target = new TargetProduto().ObterUltimaMeta(op.RotMaqId, op.PaProId, null, bd);
                if (target != null)
                {
                    targetJson = new
                    {
                        tarSetup = target.ProximaMetaTempoSetup == null ? 0 : target.ProximaMetaTempoSetup,
                        tarSetpuAjuste = target.ProximaMetaTempoSetupAjuste == null ? 0 : target.ProximaMetaTempoSetupAjuste,
                        tarPerformace = target.ProximaMetaPerformace == null ? 0 : target.ProximaMetaPerformace,
                        performaceMaxVerde = target.PerformaceMaxVerde == null ? 0 : target.PerformaceMaxVerde,
                        performaceMinVerde = target.PerformaceMinVerde == null ? 0 : target.PerformaceMinVerde,
                        setupMaxVerde = target.SetupMaxVerde == null ? 0 : target.SetupMaxVerde,
                        setupMinVerde = target.SetupMinVerde == null ? 0 : target.SetupMinVerde,
                        setupAjusteMaxVerde = target.SetupAjusteMaxVerde == null ? 0 : target.SetupAjusteMaxVerde,
                        setupAjusteMinVerde = target.SetupAjusteMinVerde == null ? 0 : target.SetupAjusteMinVerde,
                        setupMaxAmarelo = target.SetupMaxAmarelo == null ? 0 : target.SetupMaxAmarelo,
                        setupAjusteMaxAmarelo = target.SetupAjusteMaxAmarelo == null ? 0 : target.SetupAjusteMaxAmarelo,
                        performaceMinAmarelo = target.PerformaceMinAmarelo == null ? 0 : target.PerformaceMinAmarelo,
                    };
                }
            }
            return Json(new { status = true, op = opInfoJson, target = targetJson }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObterStatusMaquina(string maquinaId/*, double qtdProgramada, double metaQtdSegundo*/)
        {
            //rever view fila
            var dados = bd.VwFilaProducao.Where(f => f.RotMaqId == maquinaId && f.Produzindo == 1).OrderBy(f => f.FprDataInicioPrevista).Take(1).Select(f => new
            {
                segunPerfDecorrida = f.FprTempoDecorridoPerformacace,
                segunTempoRestante = f.FprTempoRestantePerformace,
                qtdPecasRestante = f.FprQuantidadeRestante,
                qtdPecasProduzidas = f.FprQuantidadeProduzida,
                tempoGastoPerformace = f.FprTempoDecorridoPerformacace,
                tempoDecorridoSetup = f.FprTempoDecorridoSetup,
                tempoDecorridoSetupA = f.FprTempoDecorridoSetupAjuste,
                tempoGastoSetup = f.FprTempoDecorridoSetup + f.FprTempoDecorridoSetupAjuste,
                segunPerfNecessaria = f.FprVelocidadeAtingirMeta,
                percProjVeloc = f.FprPerformaceProjetada != null ? f.FprPerformaceProjetada : 0,
                tempoDecorridoPequenasParadas = f.TempoDecorridoPequenasParadas,
                vlcAtualPcSegundo = f.FprVeloAtuPcSegundo,
            }).FirstOrDefault();
            return Json(new
            {
                dados = dados
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Uteis
        private object ConvertToAjaxMedicoes(List<ViewClpMedicoes> medicoes)
        {
            return medicoes != null ? medicoes.Select(m => new
            {
                MaquinaId = m.MaquinaId,
                DataIni = m.DataIni.ToString(),
                DataFim = m.DataFim.ToString(),
                Quantidade = m.Quantidade,
                Grupo = m.Grupo,
                TurmaId = !string.IsNullOrEmpty(m.TurmaId) ? m.TurmaId : "",
                TurnoId = !string.IsNullOrEmpty(m.TurnoId) ? m.TurnoId : "",
                Observacoes = !string.IsNullOrEmpty(m.FeedbackObs) ? m.FeedbackObs : "",
                MedicaoId = m.FeedBackId == null ? 0 : m.FeedBackId,
                OcorrenciaId = !string.IsNullOrEmpty(m.OcoId) ? m.OcoId : "",
            }).ToList() : null;
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                bd.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}