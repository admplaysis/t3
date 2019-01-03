var _tabelaFeedback = (function () {
    var eventos = {
        clickBtnDividir: function () {
            //addLoading
            var linha = $(this).parents('tr');
            var dataIni = moment(linha.find('.jsPeriodo').attr('data-data-ini'), 'DD/MM/YYYY HH:mm:ss');
            var dataFim = moment(linha.find('.jsPeriodo').attr('data-data-fim'), 'DD/MM/YYYY HH:mm:ss');
            var maquinaId = ServerValues.maquinaId;
            var grupo = linha.attr('data-grupo');
            var quantidade = linha.find('.jsQuantidade').text();
            var turno = linha.find('.jsTurnosTab').val();
            var turma = linha.find('.jsTurmasTab').val();
            var op = linha.find('.jsSltOpTab').val();
            _modalDividirPeriodo.abrirModal({
                dataIni: dataIni,
                dataFim: dataFim,
                maquinaId: maquinaId,
                grupo: grupo,
                quantidade: quantidade,
                turno: turno,
                turma: turma,
                op: op
            });
        },
        clickBtnExcluirFeedBack: function () {
            var tr = $(this).parents('tr');
            var feeId = tr.attr('data-id');
            var maqId = tr.attr('data-maq-id');
            var grupo = tr.attr('data-grupo');
            Modal.confirmacao({
                msg:'Excluir Feedback?',
                fnSucesso: function () {
                    $.ajax({
                        type: 'POST',
                        url: '/PlugAndPlay/Medicoes/CancelarFeedback',
                        data: { medId: feeId, maquina: maqId, grupo: grupo.replace('.', ',') },
                        success: function (result) {
                            if (result.ok) {
                                Conexao.obterLinhaTempo(maqId);
                            }
                            else if (result.msg.length > 0) {
                                AlertPage.mostrar("erro",result.msg);
                            }
                            else {
                                AlertPage.mostrar("erro",'Ocorreu um erro ao excluir o feedback')
                            }
                        },
                        error: function () {
                            AlertPage.mostrar("erro",'Ocorreu um erro ao excluir o feedback')
                        }
                    });
                }
            });
        }
    };
    var util = {
        abrirModalDivPeriodo: function (linha) {
            
        }
    }
    var publico = {
        documentRead: function () {
            $(document).on('click', '#exMedicoes .jsBtnDividir', eventos.clickBtnDividir);
            $(document).on('click', '#exMedicoes .jsCancelarFeedback', eventos.clickBtnExcluirFeedBack);
        }
    };
    return publico;
})();