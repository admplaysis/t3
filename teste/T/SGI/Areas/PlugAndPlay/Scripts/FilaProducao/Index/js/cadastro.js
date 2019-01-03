var Cadastro = (function () {
    var util = {
        configurarDateWidget: function () {
            $('[name="DataInicioPrevista"]').datetimepicker({
                sideBySide: true,
                showClose: true,
                widgetPositioning: {
                    horizontal: 'auto',
                    vertical: 'bottom'
                }
            });
            $('[name="DataFimPrevista"]').datetimepicker({
                sideBySide: true,
                showClose: true,
                widgetPositioning: {
                    horizontal: 'auto',
                    vertical: 'bottom'
                }
            });
            $('[name="DataFimMaxima"]').datetimepicker({
                sideBySide: true,
                showClose: true,
                widgetPositioning: {
                    horizontal: 'auto',
                    vertical: 'bottom'
                }
            });
        },
        addEmptyForm: function () {
            var formTemplate = $('#formNovoItem');
            var divLisForms = $('#divListFroms');
            divLisForms.hide();
            var pedido = $('#txtPedido').val();
            var form = formTemplate.clone();
            var formId = 'formOrdemProducao'
            form.find('[name="OrderId"]').val(pedido);
            form.attr('id', formId)
            form.find('[data-group="btnSalvar"]').attr('data-form', formId)
            divLisForms.html(form);
            ValidacaoFormulario.configurar(form);
            util.configurarDateWidget();
            divLisForms.slideToggle('slow');
        },
        salvarOrdemProducao: function (formId) {
            var form = $('#' + formId);
            form.find('input, select').prop('disabled', false);
            if (ValidacaoFormulario.validar(formId)) {
                $.post('/PlugAndPlay/FilaProducao/Create', $('#' + formId).serialize())
                    .done(function (result) {
                        if (result.ok == 1) {
                            util.removerFormulario($('#' + formId));
                            location.reload();
                        }
                        else if (result.ok == -2) {
                            Modal.confirmacao({
                                tipo: 'media',
                                msg: [
                                    $('<p>').text('Na fila de produção existe uma ordem de produção com essas configurações.'),
                                    $('<p>').text('Deseja adicionar novamente?')
                                ],
                                fnSucesso: function () {
                                    var form = $('#' + formId);
                                    form.find('[name="SequencaRepeticao"]').val(result.maxSeq + 1);
                                    util.salvarOrdemProducao(formId);
                                }
                            })
                        }
                        else if (result.msg.length > 0) {
                            Modal.erro(result.msg);
                        }
                        else {
                            Modal.erro("Ocorreu um erro ao adicionar a ordem de produção.");
                        }
                    }).fail(function () {
                        Modal.erro("Ocorreu um erro ao adicionar a ordem de produção.");
                    });
            }
        },
        removerFormulario: function (form) {
            form.fadeToggle('slow', function () {
                $(this).remove();
            });
        }
    }
    var eventos = {
        clickBtnAddOp: function () {
            if (ValidacaoFormulario.validar('formPedido')) {
                util.addEmptyForm();
            }
        },
        clickBtnCancelar: function () {
            util.removerFormulario($(this).parents('form'));
        },
        clickBtnSalvar: function () {//adiciona uma ordem de produção a fila
            var formId = $(this).attr('data-form');
            util.salvarOrdemProducao(formId);
        },
        changeSltProduto: function () {
            var sltProduto = $(this);
            var options = $('#MaquinaId option:not([value=""])');
            $.post("/PlugAndPlay/FilaProducao/BuscarMaquinas", {
                proId: $(this).val()
            }).done(function (dados) {
                if (dados.length > 0) {
                    _jqComponente.select(dados, 'MaquinaId');
                }
                else {
                    options.remove();
                    sltProduto.prop('disabled', true);
                }
            }).fail(function () {
                options.remove();
                alert('erro')
            });
        },
        changeSltMaquina: function () {
            var sltmaquina = $(this);
            var options = $('#SequenciaTansformacao option:not([value=""])');
            $.post("/PlugAndPlay/FilaProducao/BuscarSequenciasTranformacao", {
                proId: $('#ProdutoId').val(),
                maqId: $('#MaquinaId').val()
            }).done(function (dados) {
                if (dados.length > 0) {
                    _jqComponente.select(dados, 'SequenciaTansformacao', 'SequanciaTransformacao', 'SequanciaTransformacao');
                }
                else {
                    options.remove();
                    sltmaquina.prop('disabled', true);
                }
            }).fail(function () {
                options.remove();
                alert('erro')
            });
        },
        clickBtnAddFila: function () {
            var formTemplate = $('#formNovoItem');
            var divLisForms = $('#divListFroms');
            var pedido = $('#txtPedido').val();
            $.post('/PlugAndPlay/FilaProducao/ObterRoteiroProduto', { pedidoId: pedido }).done(function (result) {
                if (result.length > 0) {
                    divLisForms.empty();
                    result.forEach(function (i, index) {
                        var form = formTemplate.clone();
                        var formId = 'formOrdemProducao_' + index;
                        form.attr('id', formId)
                        form.find('[data-group="btnSalvar"]').attr('data-form', formId)
                        form.find('[name="OrderId"]').val(pedido);
                        form.find('[name="Roteiro.SequenciaTransformacao"]').val(i.sequenciaTransformacao).prop('disabled', true);
                        form.find('[name="Roteiro.PecasPorPulso"]').val(i.pecasPulso).prop('disabled', true);
                        form.find('[name="Roteiro.MaquinaId"]').val(i.maquinaId).prop('disabled', true);
                        form.find('[name="Roteiro.ProdutoId"]').val(i.produtoId).prop('disabled', true);
                        divLisForms.append(form);
                        ValidacaoFormulario.configurar(form);
                        util.configurarDateWidget();
                    });
                }
                else {
                    divLisForms.html($('<p>').text('Nenhum roteiro foi encontrado.'));
                }
            }).fail(function () {
                alert('erro!')
            });
        },
    };
    var publico = {
        inicio: function () {
            $('#btnAddItem').click(eventos.clickBtnAddItem);
            $('#ProdutoId').change(eventos.changeSltProduto);
            $('#MaquinaId').change(eventos.changeSltMaquina);
            $('#btnAdicionarFila').click(eventos.clickBtnAddFila);
            $('#btnAddProduto').click(eventos.clickBtnAddOp);

            $(document).on('click', '[data-group="btnSalvar"]', eventos.clickBtnSalvar);
            $(document).on('click', '[data-group="btnCancelar"]', eventos.clickBtnCancelar);
        }
    }
    return publico;
})();