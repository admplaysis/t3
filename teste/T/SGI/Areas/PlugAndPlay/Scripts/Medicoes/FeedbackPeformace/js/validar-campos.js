var ValidarCampos = (function () {
    var eventos = {
        removerErro: function () {
            var div = $(this).parents('.has-feedback');
            div.removeClass('has-error')
            div.find('.help-block').empty();
        }
    }
    var util = {
        informarErro: function (jqCampo, msg) {
            var div = jqCampo.parents('.has-feedback');
            div.addClass('has-error');
            div.find('.help-block').text(msg);
        },
    }
    var publico = {
        init: function () {
            $('textarea').keypress(eventos.removerErro);
            $('select').change(eventos.removerErro);
        },
        validarTodos: function () {
            var ok = true;
            var tipoFeedPerformace = $('#hfTipoFeedPerformace').val();
            var tipoFeedSetupA = $('#hfTipoFeedSetupA').val();
            var tipoFeedSetup = $('#hfTipoFeedSetup').val();
            if (tipoFeedPerformace != 'N') {
                var inptOcoPerform = $('#sltOcorrenciaPerformace');
                var inputObsPer = $('#txtJustificativaPerformace');
                if (inptOcoPerform.val() == '') {
                    util.informarErro(inptOcoPerform, "Selecione a ocorrencia.");
                    if (ok)
                        inptOcoPerform.focus();
                    ok = false;
                }
                if (inputObsPer.val() == '') {
                    util.informarErro(inputObsPer, "Descreva uma justificativa.");
                    if (ok)
                        inputObsPer.focus();
                    ok = false;
                }
            }
            if (tipoFeedSetupA != 'N') {
                var inptOcoSetupA = $('#sltOcorrenciaSetupA');
                var inputObsSetpA = $('#txtJustificativaSetupA');
                if (inptOcoSetupA.val() == '') {
                    util.informarErro(inptOcoSetupA, "Selecione a Ocorrencia.");
                    if (ok)
                        inptOcoSetupA.focus();
                    ok = false;
                }
                if (inputObsSetpA.val() == '') {
                    util.informarErro(inputObsSetpA, "Descreva uma justificativa.");
                    if (ok)
                        inputObsSetpA.focus();
                    ok = false;
                }
            }
            if (tipoFeedSetup != 'N') {
                var inptOcoSetup = $('#sltOcorrenciaSetup');
                var inptObsSetup = $('#txtJustificativaSetup');
                if (inptOcoSetup.val() == '') {
                    util.informarErro(inptOcoSetup, "Selecione a Ocorrencia.");
                    if (ok)
                        inptOcoSetup.focus();
                    ok = false;
                }
                if (inptObsSetup.val() == '') {
                    util.informarErro(inptObsSetup, "Descreva uma justificativa.");
                    if (ok)
                        inptObsSetup.focus();
                    ok = false;
                }
            }
            return ok;
        },
    }
    return publico;
})()