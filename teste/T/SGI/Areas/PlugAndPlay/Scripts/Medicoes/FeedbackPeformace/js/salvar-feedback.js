SalvarFeedback = (function () {
    var eventos = {
        clickBtnSalvar: function () {
            if (ValidarCampos.validarTodos()) {
                util.salvarFeedback();
            }
        }
    }
    var util = {
        salvarFeedback: function () {
            
            var OcoPerform = $('#sltOcorrenciaPerformace').val();
            var ObsPer = $('#txtJustificativaPerformace').val();
            var OcoSetupA = $('#sltOcorrenciaSetupA').val();
            var ObsSetpA = $('#txtJustificativaSetupA').val();
            var OcoSetup = $('#sltOcorrenciaSetup').val();
            var ObsSetup = $('#txtJustificativaSetup').val();
            var TargetId = $('#hfTargetId').val();
            var MovimentoId = $('#hfMovimentoId').val();

            var target = {
                ObsPerformace: ObsPer,
                ObsSetup: ObsSetup,
                ObsSetupAjuste: ObsSetpA,
                OcoIdPerformace: OcoPerform,
                OcoIdSetup: OcoSetup,
                OcoIdSetupAjuste: OcoSetupA,
                MovimentoEstoqueId: MovimentoId,
                Id: TargetId
            }; 
            Carregando.abrir();
            $.ajax({
                type: 'POST',
                url: '/PlugAndPlay/Medicoes/GravarFeedbackPerformace',
                data: JSON.stringify(target),
                contentType: "application/json; charset=utf-8",
                success: function (ok) {
                    if (ok) {
                        var url = decodeURIComponent((new URL(document.location.href)).searchParams.get('url'));
                        window.location.href = url;
                    }
                    else {
                        Carregando.fechar();
                        AlertPage.mostrar("erro",'Ocorreu um erro ao salvar os feedbacks.');
                    }
                },
                error: function () {
                    Carregando.fechar();
                    AlertPage.mostrar("erro",'Ocorreu um erro ao salvar os feedbacks.');
                }
            });
        }
    }
    var publico = {
        init: function () {
            $('#btnSalvarFeedback').click(eventos.clickBtnSalvar);
        },
        salvarFeedback: function (encerrar = false, ocorrencia = '', justifiativa = '') {
            return util.salvarFeedback(encerrar, ocorrencia, justifiativa);
        }
    }
    return publico;
})();