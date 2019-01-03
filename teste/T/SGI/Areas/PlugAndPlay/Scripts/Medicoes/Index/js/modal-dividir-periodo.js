var _modalDividirPeriodo = (function () {
    var modal = $('#modalDividirPeriodo');
    $(document).ready(function () {
        //ocorre quando a modal é fechada
        $(document).on('hidden.bs.modal', '#modalDividirPeriodo', function () {
            $('#mdDiviPeriBtnSalvar').off();
        });
        //quando a data do datepicher é alterada
        $(document).on('dp.change', '#modalDividirPeriodo .jsDate', eventos.changeDatePicker);
        ////configura para abrir o date picker quando clica no campo
        //click do botao dividir periodo
        $(document).on('click', '#modalDividirPeriodo .table .btn-defaut', eventos.clickBtnDividirPeriodo);
        $(document).on('click', '#mdDivPerioBtnDesfazer', eventos.clickBtnDesfazer);

        $(document).on('click', ' #mdDivQtdPartInconMenos', eventos.clickBtnDecrementQuantPart);
        $(document).on('click', ' #mdDivQtdPartInconMais', eventos.clickBtnIncremetQuantPart);
    });
    var eventos = {
        clickBtnDividirPeriodo: function () {
            var tabBody = modal.find('.tabTop table tbody');
            var numPartes = modal.find('.tabTop .jsNumPartes').val();
            var dataIni = moment(tabBody.find('[data-data-ini]').attr('data-data-ini'), 'DD/MM/YYYY HH:mm:ss');
            var dataFim = moment(tabBody.find('[data-data-fim]').attr('data-data-fim'), 'DD/MM/YYYY HH:mm:ss');

            if (Number != '' && numPartes > 0) {
                for (var i = 0; i < numPartes; i++) {
                    var inputIni = $('<input>', { type: 'text', class: 'form-control jsIni' }).prop('disabled', true);
                    var inputFim = $('<input>', { type: 'text', class: 'form-control jsFim jsDate' });
                    if (i == 0) {
                        inputIni.val(dataIni.format('DD/MM/YYYY HH:mm:ss'));
                        inputIni.removeClass('jsDate');
                    }
                    if (i == numPartes - 1) {
                        inputFim.val(dataFim.format('DD/MM/YYYY HH:mm:ss')).prop('disabled', true);
                        inputFim.removeClass('jsDate');
                    }
                    tabBody.append([
                        $('<tr>', { class: 'jsTrInput' }).html([
                            $('<td>').html([
                                inputIni
                            ]),
                            $('<td>').html([
                                inputFim
                            ]),
                            $('<td>'),
                        ])
                    ]);
                }
                tabBody.append([
                    $('<tr>').html([
                        $('<td>', { colspan: '3', class: 'text-right' }).html([
                            $('<button>', { id: 'mdDivPerioBtnDesfazer', class: 'btn btn-default' }).text('Desfazer')
                        ])
                    ])
                ]);
                //configura date time picker para o campos de data e hora
                $('#modalDividirPeriodo .jsDate').datetimepicker({
                    sideBySide: true,
                    showClose: true,
                    useCurrent: false,
                    defaultDate: dataIni
                });
                //desabilita as acoes para dividir o periodo
                modal.find('.tabTop .jsNumPartes').prop("disabled", true);
                $(this).prop("disabled", true);
            }
        },
        clickBtnDesfazer: function () {
            var trs = modal.find('.tabTop table tbody tr');
            for (var i = 1; i < trs.length; i++) {
                trs.eq(i).remove();
            }
            modal.find('.tabTop .jsNumPartes').prop("disabled", false);
            modal.find('.tabTop .jsDividir').prop("disabled", false);
        },
        changeDatePicker: function (e) {
            var linha = $(this).parents('tr');
            var pLinha = linha.next();
            pLinha.find('.jsIni').val(e.date.format('DD/MM/YYYY HH:mm'));
        },
        clickBtnIncremetQuantPart: function () {
            var valor = $('#mdDivPeriodoBtnNumPartes').val();
            if (valor < 10)
                $('#mdDivPeriodoBtnNumPartes').val(Number(valor) + 1);
        },
        clickBtnDecrementQuantPart: function () {
            var valor = $('#mdDivPeriodoBtnNumPartes').val();
            if (valor > 2)
                $('#mdDivPeriodoBtnNumPartes').val(Number(valor) -1);
        }
    };
    var utils = {
        limparDivAlert: function () {
            DivAlertMsg.limpar('divMsgAlerts');
        }
    }
    var publico = {
        abrirModal: function (conf) {
            $('#mdDiviPeriBtnSalvar').click(function () {//envia ao servidor os periodos definidos pelo usuário
                var trs = modal.find('.table .jsTrInput');//seleciona todas as linhas da tabela que tem os inputs das data e hora
                if (trs.length > 0) {
                    var maquinaId = $('#mdDivPeriodoHfMaquinaId').val();
                    var grupo = $('#mdDivPeriodoHfGrupo').val();
                    var medicoes = [];
                    var validDates = true;
                    //data inicio e fim do periodo original
                    var tabBody = modal.find('.tabTop table tbody');
                    var dataIniOr = moment(tabBody.find('[data-data-ini]').attr('data-data-ini'), 'DD/MM/YYYY HH:mm:ss');
                    var dataFimOr = moment(tabBody.find('[data-data-fim]').attr('data-data-fim'), 'DD/MM/YYYY HH:mm:ss');
                    debugger
                    trs.each(function (index, linha) {
                        var dataIni = moment($(linha).find('.jsIni').val(), 'DD/MM/YYYY HH:mm:ss');
                        var dataFim = moment($(linha).find('.jsFim').val(), 'DD/MM/YYYY HH:mm:ss');
                        if (dataIni.isBefore(dataFim) && (dataIni.isBetween(dataIniOr, dataFimOr) || dataIni.isSame(dataIniOr) || dataIni.isSame(dataFimOr)) && (dataFim.isBetween(dataIniOr, dataFimOr) || dataFim.isSame(dataIniOr) || dataFim.isSame(dataFimOr))) {//valida o sequenciamento das datas
                            medicoes.push({
                                Grupo: grupo,
                                MaquinaId: maquinaId,
                                DataInicio: dataIni.format('YYYY/MM/DD HH:mm:ss'),
                                DataFim: dataFim.format('YYYY/MM/DD HH:mm:ss'),
                                Quantidade: $('#mdDivPeriodoHfQuantidade').val(),
                                TurnoId: $('#mdDivPeriodoHfTurno').val(),
                                TurmaId: $('#mdDivPeriodoHfTurma').val(),
                                OrdemProducaoId: $('#mdDivPeriodoHfOp').val(),
                            });
                        }
                        else {
                            validDates = false;
                        }
                    });
                    if (!validDates) {
                        DivAlertMsg.erro('divMsgAlerts', 'As datas precisam ser definidas na sequencia correta.', true);
                    }
                    if (validDates) {
                        Carregando.abrir();
                        $.ajax({
                            type: 'POST',
                            url: '/PlugAndPlay/Medicoes/DividirPeriodo',
                            data: JSON.stringify(medicoes),
                            contentType: "application/json",
                            success: function (resp) {
                                if (resp.status) {
                                    modal.modal('hide');
                                    Conexao.obterLinhaTempo(maquinaId)
                                }
                                else if (resp.msgError.length > 0) {
                                    DivAlertMsg.erro('divMsgAlerts', resp.msgError, true);
                                } else {divMsgAlerts
                                    DivAlertMsg.erro('divMsgAlerts', 'Ocorreu um erro ao salvar as alterações.', true);
                                }
                            },
                            error: function () {
                                DivAlertMsg.erro('divMsgAlerts', 'Ocorreu um erro ao salvar as alterações.', true);
                            },
                            complete: function () {
                                Carregando.fechar();
                            }
                        });
                    }
                }
                else {
                    DivAlertMsg.atencao('divMsgAlerts', 'Nenhuma alteração foi realizada.', true);
               }
            });
            modal.find('.tabTop').html([
                $('<input>', { type: 'hidden', id: 'mdDivPeriodoHfGrupo' }).val(conf.grupo),
                $('<input>', { type: 'hidden', id: 'mdDivPeriodoHfMaquinaId' }).val(conf.maquinaId),
                $('<input>', { type: 'hidden', id: 'mdDivPeriodoHfQuantidade' }).val(conf.quantidade),
                $('<input>', { type: 'hidden', id: 'mdDivPeriodoHfTurno' }).val(conf.turno),
                $('<input>', { type: 'hidden', id: 'mdDivPeriodoHfTurma' }).val(conf.turma),
                $('<input>', { type: 'hidden', id: 'mdDivPeriodoHfOp' }).val(conf.op),
                $('<table>', { class: 'table' }).html([
                    $('<thead>').html([
                        $('<tr>').html([
                            $('<td>').text('Início'),
                            $('<td>').text('Fim'),
                            $('<td>').text(''),
                        ])
                    ]),
                    $('<tbody>').html([//gera a tabela no topo da modal
                        $('<tr>', { class: 'warning' }).html([
                            $('<td>', { 'data-data-ini': conf.dataIni.format('DD/MM/YYYY HH:mm:ss') }).html([
                                $('<div>', { class:'dataTopHora'}).text(conf.dataIni.format('DD/MM/YYYY')),
                                $('<div>').text(conf.dataIni.format('HH:mm:ss')),
                            ]),
                            $('<td>', { 'data-data-fim': conf.dataFim.format('DD/MM/YYYY HH:mm:ss') }).html([
                                $('<div>', { class: 'dataTopHora' }).text(conf.dataFim.format('DD/MM/YYYY')),
                                $('<div>').text(conf.dataFim.format('HH:mm:ss')),
                            ]),
                            $('<td>', { class: 'text-right' }).html([
                                $('<div>', { class: 'group-controls'}).html([
                                    $('<input>', { id: 'mdDivPeriodoBtnNumPartes', type: 'text', class: 'jsNumPartes' }),
                                    $('<button>', { class: 'btn-incon', id: 'mdDivQtdPartInconMenos' }).html($('<i>', { class: 'fa fa-chevron-down'})),
                                    $('<button>', { class: 'btn-incon', id: 'mdDivQtdPartInconMais' }).html($('<i>', { class: 'fa fa-chevron-up' })),
                                    $('<button>', { style: 'margin-left:5px', class: 'btn btn-defaut jsDividir', type: 'button' }).html($('<i>', { class: 'fa fa-scissors'}))
                                ])
                            ]),
                        ])
                    ])
                ])
            ]);
            //mascara para os campos
            $('#mdDivPeriodoBtnNumPartes').inputmask({
                alias: 'integer',
                min: '2',
                max: '10',
                rightAlign: false,
            });
            $('#mdDivPeriodoBtnNumPartes').val(2);
            modal.modal('show');
        }
    };
    return publico;
})();