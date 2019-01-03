var ModalEncerramentoOp = (function () {
    var eventos = {
        clickCkEncerrarOP: function () {
            $('#divFormEncOpParcial').slideToggle();
            $('#divAlertEncOpParcial').slideToggle();
            eventos.limparErrosCampos();
        },
        clickBtnSalvar: function () {
            var encerrar = $('#ckEncerrarOpParcial').prop('checked');
            var ddlOcorencia = $('#ddlOcorrenciaOpParcial');
            var txtJustificativa = $('#txtJustificativaEncOpParcial');
            if (encerrar) {
                var ok = true;
                if (txtJustificativa.val().trim() == '') {
                    txtJustificativa.parents('.has-feedback').addClass('has-error');
                    txtJustificativa.next().text('informe a justificativa');
                    ok = false;
                }
                if (ddlOcorencia.val() == '') {
                    ddlOcorencia.parents('.has-feedback').addClass('has-error');
                    ddlOcorencia.next().text('Selecione a ocorrencia');
                    ok = false;
                }
                if (ok) {
                    ApApp.salvarApontamentos(encerrar, txtJustificativa.val(), ddlOcorencia.val(), );
                }
            }
            else {
                ApApp.salvarApontamentos(encerrar);
            }
        },
        resetar: function () {
            eventos.limparErrosCampos();
            $('#ckEncerrarOpParcial').prop('checked', false);
            $('#divFormEncOpParcial').hide();
            $('#divAlertEncOpParcial').show()

        },
        limparErroCampo: function (campo) {
            var campo = $(this);
            campo.parents('.has-feedback').removeClass('has-error');
            campo.next().empty();
        },
        limparErrosCampos: function () {
            $('#mdConfirmEncOp .help-block').empty();
            $('#mdConfirmEncOp .has-feedback').removeClass('has-error');
            $('#mdConfirmEncOp select').find('option [value=""]').prop('selected', true);
            $('#mdConfirmEncOp textarea').val('');
        }
    }
    var publico = {
        documentReady: function () {
            $('#btnSalvarFeedbackModal').click(eventos.clickBtnSalvar);
            $('#mdConfirmEncOp select').change(eventos.limparErroCampo);
            $('#mdConfirmEncOp textarea').click(eventos.limparErroCampo);
            $('#mdConfirmEncOp #labelCkEnOp').change(eventos.clickCkEncerrarOP);
            $('#mdConfirmEncOp').on('hidden.bs.modal', eventos.resetar);
        },
        abrir: function (qtdPecaBoaProduzida) {
            var confirm = $('#mdConfirmEncOp'); 
            $('#qtdPecasBoasProd').text(qtdPecaBoaProduzida);
            confirm.modal('show');
        }
    }
    return publico;
})();