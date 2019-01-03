//variaveis de controle para as funcoes ajax que atualizam periodicamente as lista de grupos/lotes
// são necessarias para resetar completamente o setTimeOut e a requisicao ajax
var _valorSetTimeOut = null;
var _ajaxStatus = null;
//Caixas de seleção da tabela de feedbacks
var _mtSltQuant = null;
var _mtSltNoQuant = null;
var _turnoSlt = null;
var _turmaSlt = null;
//Caixas de seleção da tela
var _opSlt = null;
//-------------------
var _ultimoGrupo = '';
var _maquinaId = null;


var _metaPerfProduto = 0;
var _gfPercentMeta = null;
var _filaProducao = null;
var _ultimaDataInicio = '';
var _ultimoTurno = '';
var _linhaTab = null;
var _utilAddObs = null;
var _linhaTabSalvarMed = null;
var _iconErro = $('<i>', { class: 'fa fa-exclamation-circle ic-error' });
var _divInfo = $('<div>', { class: 'alert alert-info' });
//objeto compartilhado que carrega informações acerca da produção atual de acordo com a op selecionada
//é utilizado nas funçoes que atualizam as informacoes na tela de forma automatica e periodica.
var _monitor = {
    op: '',
    qtdProd: 0,
    dataIni: null,
    opsQuantAdd: [],
    inicioPlan: null,
    fimPlan: null
};
var teste = null;

_ipClpMaquina = "";
//variaveis de controle para as funcoes ajax que atualizam as dados em tempo real dos velocimetros
// são necessarias para resetar completamente o setTimeOut e a requisicao ajax
var _ajaxStatusMonitor = null;
var _valorSetTimeOutMonitor = null;

var App = {
    config: {
        modalSetIniSetup: $('#modalInicioSetup')
    },
    documentReady: function () {
        $('#divTabMedicoes').html(_divInfo.clone().text('Aguardando consulta.'));
        $('#exMedicoes').html(_divInfo.clone().text('Aguardando consulta.'));
        $('#modalAddObsPeriodo').on('hidden.bs.modal', App.hiddenModalAddObsPeriodo)

        $("#sltMaquina, #sltMaquinaFeed").change(App.sltMaquinaChage);
        $("#sltOp").change(App.sltOpChange);
        $(document).on('change', '.jsSltOpTab', App.sltOpTab);
        $(document).on('click', '.jsBtnAddObsPeriodo', App.clickBtnAddObsPeriodo);
        $(document).on('click', '#btnSalvarObsPeriodo', App.clickBtnSalvarObsPeriodo);
        $(document).on('click', '.jsBtnSalvarMedicao', App.clickBtnSalvarMedicao);
        $(document).on('change', 'table select', App.removerValidacao);
        $(document).on('click', 'table .fa-refresh', App.clickInconReflesh);
        $(document).on('click', '#btnHistorico', App.clickBtnHistorico);
        $(document).on('change', '#sltMaquinaHist', App.changeSltMaqHist);
        $(document).on('keypress focus', '#txtData', App.keyPressInputData);
        $(document).on('click', '.table .jsPeriodo input', App.clickInputSetPeriodo);
        $(document).on('click focus', '.table .jsPeriodo button', App.clickBtnSetPeriodo);
        $(document).on('click', '.jsBtnDesfazer', App.clickBtnDesfazer);
        $(document).on('click', '.jsBtnSincroniza', App.clickBtnSincroniza);
    },
    clickBtnSincroniza: function () {
        alert('teste')
    },
    clickBtnDesfazer: function () {
        var linha = $(this).parents('tr');
        var meId = linha.attr('data-id');
        var grupo = linha.attr('data-grupo');
        var maquina = ServerValues.maquinaId;
        var funcao = function () {
            Util.stausBtsTab(false);
            var icon = $('<i>', { class: 'fa fa-spinner fa-spin' });
            var btn = linha.find('.jsAcoes .jsBtnDesfazer');
            var tdAcoes = linha.find('.jsAcoes');
            btn.html(icon.clone());
            teste = $.ajax({
                type: 'POST',
                url: '/PlugAndPlay/Medicoes/DesfazerMedicao',
                data: {
                    medId: meId,
                    maquina: maquina,
                    grupo: grupo.replace('.', ',')
                },
                success: function (ok) {
                    if (ok.ok) {
                        Conexao.obterLinhaTempo(maquina);
                    }
                    else {
                        AlertPage.mostrar("erro", ok.msg);
                    }
                },
                error: function () {
                    tdAcoes.find('div').remove();
                    btn.before($('<div>').html(
                        $('<div>').html(
                            $('<i>', { class: 'fa fa-exclamation-circle ic-error' })
                        )));
                    AlertPage.mostrar("erro", 'Ocorreu um erro ao desfazer o feedback');
                },
                complete: function () {
                    btn.html($('<i>', { class: 'fa fa-undo' }));
                    Util.stausBtsTab(true);
                }
            });
        }
        Modal.confirmacao({
            fnSucesso: funcao,
            msg: 'Desfazer feedback?'
        });
    },
    clickBtnSetPeriodo: function () {
        var td = $(this).parent('td');
        $(this).remove();
        var input = td.find('input');
        var datafim = input.val();
        td.append($('<span>').text(' - ' + td.attr('data-data-fim').substring(11, 16)));
        td.attr('data-data-ini', td.attr('data-data-fim').substring(0, 11) + datafim + ':00');
        var trPai = td.parent().prev();
        tdPerPai = trPai.find('.jsPeriodo');
        tdPerPai.attr('data-data-fim', tdPerPai.attr('data-data-ini').substring(0, 11) + datafim + ':00')
        tdPerPai.text(tdPerPai.attr('data-data-ini').substring(11, 16) + ' - ' + datafim);
    },
    clickInputSetPeriodo: function () {
        var input = $(this);
        var td = input.parent('td');
        td.find('span').remove();
        input.val(td.attr('data-data-fim').substring(11, 16));
        if (td.find('button').length == 0) {
            input.after($('<button>')
                .html($('<i>', { class: 'fa fa-floppy-o' })));
        }
    },
    keyPressInputData: function () {
        var ic = $(this).parent('.form-group').find('.fa ');
        ic.popover('destroy');
        ic.remove();
    },
    changeSltMaqHist: function () {
        var ic = $(this).parent('.form-group').find('.fa ');
        ic.popover('destroy');
        ic.remove();
    },
    clickBtnHistorico: function () {
        var div = $(this).parent('div');
        div.find('[data-toggle="popover"]').popover('destroy');
        div.find('.ic-error').remove();
        var ok = true;
        icErro = T.iconError();
        var maq = $('#sltMaquinaHist').val();
        var data = $('#txtData').val();
        var turno = $('#sltTurno').val();
        if (maq == '') {
            ok = false;
            $('#sltMaquinaHist').parent('.form-group')
                .append(icErro.attr('data-content', 'Selecione a máquina.'));
        }
        if (data == '') {
            ok = false;
            $('#txtData').parent('.form-group')
                .append(T.iconError().attr('data-content', 'Informe a data.'));
        }
        else if (!Util.validarData(data)) {
            ok = false;
            $('#txtData').parent('.form-group')
                .append(T.iconError().attr('data-content', 'Data inválida.'));
        }
        if (ok) {
            Conexao.obterHistorico(maq, data, turno);
        }
        $('[data-toggle="popover"]').popover('show');
    },
    removerValidacao: function () {
        var td = $(this).parents('td');
        td.find('.div-incon').empty();
    },          //Tabela feedbacks pendentes 
    clickBtnSalvarMedicao: function () {
        var ok = true;
        var linha = $(this).parents('tr');
        var op = linha.find('.jsOp>select').val();
        var oc = linha.find('.jsOcorrencia>select').val();
        var grupo = linha.attr('data-grupo');
        var turno = linha.find('.jsTurno>select').val();
        var turma = linha.find('.jsTurma>select').val();
        //var seqTransformacao = linha.find('.jsOp>select>option:selected').attr('data-seg-transformacao');
        //var seqRepeticao = linha.find('.jsOp>select>option:selected').attr('data-seg-repeticao');
        //var pecasPulso = linha.find('.jsOp>select>option:selected').attr('data-pecas-pulso');
        //var proId = linha.find('.jsOp>select>option:selected').attr('data-pro-id');
        if (op == 0) op = null
        if (oc == 0) oc = null;
        if (turno == '') turno = null;
        if (turma == '') turma = null;
        var medicao = {
            Id: linha.attr('data-id'),
            QuantidadePulsos: linha.find('.jsQuantidade').text(),
            DataInicial: Util.dateFormatAm(linha.find('.jsPeriodo').attr('data-data-ini')),
            Datafinal: Util.dateFormatAm(linha.find('.jsPeriodo').attr('data-data-fim')),
            Observacoes: linha.find('.jsObservacao').attr('data-observacao').trim(),
            MaquinaId: linha.attr('data-maq-id'),
            OcorrenciaId: oc,
            //ProdutoId: proId == undefined ? null : proId,
            TurnoId: turno,
            TurmaId: turma,
            Grupo: grupo,
            //SequenciaTransformacao: seqTransformacao == '' || seqTransformacao == undefined ? null : seqTransformacao,
            //SequenciaRepeticao: seqRepeticao == '' || seqRepeticao == undefined ? null : seqRepeticao,
            //QuantidadePecasPorPulso: pecasPulso == '' || pecasPulso == undefined ? null : pecasPulso,
        };
        var inconError = $('<i>', { class: 'fa fa-exclamation-circle ic-error' });
        var tdOp = linha.find('.jsOp');
        var tdOco = linha.find('.jsOcorrencia');
        var tdObs = linha.find('.jsObservacao');

        var antLinha = linha.prev();
        if (medicao.Quantidade > 0 && medicao.OrdemProducaoId == null) {
            ok = false;
            tdOp.find('select').addClass('campo-invalido');
            tdOp.find('.div-incon').html(inconError.clone());
        }
        else {
            tdOp.find('select').removeClass('campo-invalido');
            tdOp.find('.div-incon').empty();
        }
        if (medicao.OcorrenciaId == null) {
            ok = false;
            tdOco.find('select').addClass('campo-invalido');
            tdOco.find('.div-incon').html(inconError.clone());
        }
        else {
            tdOco.find('select').removeClass('campo-invalido');
            tdOco.find('.div-incon').empty();
        }
        var meta = linha.find('.jsMeta').text();
        var vel = linha.find('.jsVelocidade').text();
        if (medicao.Quantidade > 0 && medicao.Observacoes == ''
            && typeof vel === 'number' && typeof meta === 'number' && vel < meta) {
            tdObs.find('span').html(inconError.clone());
        }
        if (ok) {
            if (medicao.OcorrenciaId != null) {
                var tipo = tdOco.find('select option:selected').attr('data-tipo');
                if (tipo == 2) {
                    var correntDataIni = moment(linha.find('.jsPeriodo').attr('data-data-ini'), 'DD/MM/YYYY HH:mm:ss');
                    var correntDataFim = moment(linha.find('.jsPeriodo').attr('data-data-fim'), 'DD/MM/YYYY HH:mm:ss');
                    var proxLinha = linha.nextAll(':not(.warning)').eq(0);
                    var proxQuant = proxLinha.find('.jsQuantidade').text();
                    var proxDataIni = moment(proxLinha.find('.jsPeriodo').attr('data-data-ini'), 'DD/MM/YYYY HH:mm:ss');
                    var proxDataFim = moment(proxLinha.find('.jsPeriodo').attr('data-data-fim'), 'DD/MM/YYYY HH:mm:ss');

                    if (proxQuant != '' && proxQuant > 0) {
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
                    }
                    else {
                        Conexao.gravarMedicao(medicao, grupo, linha);
                    }
                }
                else {
                    Conexao.gravarMedicao(medicao, grupo, linha);
                }
            }
        }
    },

    hiddenModalAddObsPeriodo: function () {
        $('#modalAddObsPeriodo .jsTxtLimpar').val('');
    },
    clickBtnSalvarObsPeriodo: function () {
        var obs = $('#txtObsAddPeriodo').val().trim();

        if (obs.length != 0) {
            _utilAddObs.linhaTab.find('.jsObservacao').attr('data-observacao', obs);
            if (obs.length > 7) {
                _utilAddObs.linhaTab.find('.jsObservacao span').text(obs.substring(0, 7) + '...');
            }
            else {
                _utilAddObs.linhaTab.find('.jsObservacao span').text(obs);
            }
        }
        else {
            _utilAddObs.linhaTab.find('.jsObservacao').attr('data-observacao', '');
            _utilAddObs.linhaTab.find('.jsObservacao span').text('-');
        }
        _utilAddObs.modal.modal('hide');
    },
    clickBtnAddObsPeriodo: function () {
        _utilAddObs = {
            linhaTab: $(this).parents('tr'),
            modal: $('#modalAddObsPeriodo')
        };
        var obs = _utilAddObs.linhaTab.find('.jsObservacao').attr('data-observacao');
        if (obs.trim() != '') {
            _utilAddObs.modal.find('#txtObsAddPeriodo').val(obs);
        }
        _utilAddObs.modal.modal();
    },
    sltOpTab: function () {
        Util.addQuantOpTab($(this).val(), $(this).parents('tr'));
        App.buscarMetaOp($(this).val(), $(this).parents('tr'));

    },
    buscarMetaOp: function (value, linhaTab) {
        var ok = true;
        var op = value;
        var quant = linhaTab.find('.jsQuantidade').text();
        if (op == 0) {
            ok = false;
            var tdMeta = linhaTab.find('.jsMeta');
            var tdVel = linhaTab.find('.jsVelocidade');
            tdMeta.text('-');
            tdVel.text('-');
        }
        if (quant == 0) {
            ok = false;
        }
        if (ok) {
            Conexao.obterMetaOp(op, _maquinaId, linhaTab);
        }
    },
    clickInconReflesh: function () {
        var linha = $(this).parents('tr');
        var op = linha.find('.jsOp select').val();
        App.buscarMetaOp(op, linha);
    },
    sltMaquinaChage: function () {
        FilaProducao.limparFila();
        Monitoramento.cancelar();

        _maquinaId = $(this).val();
        $('#sltMaquinaFeed option[value="' + _maquinaId + '"]').prop('selected', true);
        $('#sltMaquina option[value="' + _maquinaId + '"]').prop('selected', true);
        var op = $('#sltOp');
        var div = op.parent('.form-group');
        div.find('.ic-error').popover('destroy');
        div.find('.ic-error').remove();
        op.find('option:not([value=""])').remove();
        op.prop('disabled', true);
        if (_maquinaId != '') {
            Global.atualizarPaginaAjax($(this).val())
        }
        else {
            ReqTimeOut.ResetAtGrupos();
            $('#exMedicoes').html(T.divInfo('Selecione a máquina para visualizar as medições.'));
        }
    },
    sltOpChange: function () {
        var maqId = $('#sltMaquina').val();
        var op = $('#sltOp').val();
        _monitor.op = '';
        _monitor.qtdProd = 0;
        _monitor.dataIni = null;
        _monitor.opsQuantAdd = [];
        if (op != '' && maqId != '') {
            Conexao.obterMetasProduto(maqId, op);
        }
    }
};
var Conexao = {
    obterLinhaTempo: function (maquina) {
        ReqTimeOut.ResetAtGrupos();
        divTable = $('#exMedicoes');
        divTable.html(T.LoadTable());
        var op = $('#sltOp');
        var div = op.parent('.form-group');
        div.append(T.icLoad());
        $.ajax({
            type: 'POST',
            url: '/PlugAndPlay/Medicoes/ObterMedicoes',
            data: { maquinaId: maquina },
            success: function (dados) {
                if (dados != null) {
                    console.log(dados)
                    if (dados.medicoes.length) {
                        var table = Dom.gerarEstruturaTab();
                        var opSltAtual = $('#sltOp');
                        //Cria os componentes de layout para os todos os selects da tabela
                        if (dados.fila.length > 0) {
                            opSltAtual.prop('disabled', false);
                            opSltAtual.html($('<option>').text('Selecione...').attr('value', ''));
                            _opSlt = $('<select>', { class: 'form-control jsSltOpTab', style: 'width: 90px' }).html([
                                $('<option>')
                                    .attr('value', '0')
                                    .text('...')
                            ]);
                            dados.fila.forEach(function (f) {
                                _opSlt.append($('<option>',
                                    {
                                        'data-op-id': f.opId,
                                        'data-seg-transformacao': f.segTransformacao == null ? '' : f.segTransformacao,
                                        'data-seg-repeticao': f.seqRepeticao == null ? '' : f.seqRepeticao,
                                        'data-pro-id': f.proId,
                                        'data-pecas-pulso': f.pecasPulso
                                    })
                                    .attr('value', f.opId)
                                    .text(f.opId + f.proId + f.segTransformacao + f.seqRepeticao));
                                opSltAtual.append($('<option>')
                                    .attr('value', f.opId)
                                    .text(f.opId + f.proId + f.segTransformacao + f.seqRepeticao));
                            });
                        }
                        else {
                            _opSlt = $('<select>', { class: 'form-control erro' })
                                .prop('disabled', true)
                                .html($('<option>').text('...'));
                            var div = opSltAtual.parent('.form-group');
                            div.append(T.iconError().attr('data-content', 'Erro ao obter OPs.'));
                        }
                        if (dados.motivosPadada.length > 0 && dados.motivosProduzindo.length) {
                            _mtSltQuant = $('<select>', { class: 'form-control jsSltOpMotivoTab', style: 'width: 90px' }).html([
                                $('<option>')
                                    .attr('value', '0')
                                    .text('...')
                            ]);
                            _mtSltNoQuant = $('<select>', { class: 'form-control jsSltOpMotivoTab', style: 'width: 90px' }).html([
                                $('<option>')
                                    .attr('value', '0')
                                    .text('...')
                            ]);
                            dados.motivosPadada.forEach(function (m) {
                                _mtSltNoQuant.append($('<option>')
                                    .attr('value', m.Id)
                                    .attr('data-tipo', m.Tipo)
                                    .text(m.Descricao));
                            });
                            dados.motivosProduzindo.forEach(function (m) {
                                _mtSltQuant.append($('<option>')
                                    .attr('value', m.Id)
                                    .attr('data-tipo', m.Tipo)
                                    .text(m.Descricao));
                            });
                        }
                        else {
                            _mtSltQuant = $('<select>', { class: 'form-control erro' })
                                .prop('disabled', true)
                                .html($('<option>').text('...'));
                            _mtSltNoQuant = $('<select>', { class: 'form-control erro' })
                                .prop('disabled', true)
                                .html($('<option>').text('...'));
                        }
                        if (dados.turnos.length > 0) {
                            _turnoSlt = $('<select>', { class: 'form-control jsTurnosTab', style: 'width: 90px' }).html([
                                $('<option>')
                                    .attr('value', '')
                                    .text('...')
                            ]);
                            dados.turnos.forEach(function (t) {
                                _turnoSlt.append($('<option>')
                                    .attr('value', t.Id)
                                    .text(t.Id));
                            });
                        }
                        else {
                            _turnoSlt = $('<select>', { class: 'form-control erro' })
                                .prop('disabled', true)
                                .html($('<option>').text('...'));
                        }
                        if (dados.turmas.length > 0) {
                            _turmaSlt = $('<select>', { class: 'form-control jsTurmasTab', style: 'width: 90px' }).html([
                                $('<option>')
                                    .attr('value', '')
                                    .text('...')
                            ]);
                            dados.turmas.forEach(function (t) {
                                _turmaSlt.append($('<option>')
                                    .attr('value', t.Id)
                                    .text(t.Id));
                            });
                        }//fim da criação dos selects
                        var trs = [];
                        //var data = dados.medicoes[0].DataIni.substring(0, 10);
                        //var turno = dados.medicoes[0].TurnoId;
                        //var trDate = $('<tr>', { class: 'warning group' }).html([
                        //    $('<td>', { style: 'padding: 2px 8px 2px 8px', colspan: '10', class: 'text-center' }).html([
                        //        $('<div>', { class: 'grupo-tab text-left' }).html([
                        //            $('<span>', { class: 'font-negrito' }).text('Turno: '),
                        //            $('<span>', { class: '' }).text(turno)
                        //        ]),
                        //        $('<div>', { class: 'grupo-tab text-right' }).html([
                        //            $('<span>', { class: 'font-negrito' }).text('Data: '),
                        //            $('<span>', { class: '' }).text(data)
                        //        ]),
                        //    ])
                        //]);
                        //table.find('tbody').append(trDate);

                        var ultimoIndice = dados.medicoes.length - 1;
                        _ultimoGrupo = dados.medicoes[ultimoIndice].Grupo;
                        var ultimoGrupo = dados.medicoes[ultimoIndice];
                        var btn = true;//variavel para controlar a ativação o botão salvar na interface
                        dados.medicoes.forEach(function (m, index) {
                            //verifica se a data ou a turma mudou para gerar a linha que agrupa por data e tuno
                            //if (m.DataIni.substring(0, 10) != data || m.TurnoId != turno) {
                            //    data = m.DataIni.substring(0, 10);
                            //    turno = m.TurnoId;
                            //    trs.push(Dom.geratLinhaTabPenData(m));
                            //}
                            var tr = Dom.gerarLinhaTabPen(m, _opSlt, _mtSltQuant, _mtSltNoQuant, _turnoSlt, _turmaSlt, ServerValues.maquinaId);
                            table.find('tbody').append(tr);
                        });

                        //_ultimaDataInicio = data;
                        //_ultimoTurno = turno;
                        //----------------
                        $('#exMedicoes').html(table);
                        //Inicia o ajax polling recursiva para atualizar a tabela
                        Conexao.obterMedicoesTempoReal(_maquinaId, _ultimoGrupo);
                    }
                    else {
                        $('#exMedicoes').html(T.divInfo('Nenhuma medição foi encontrada.'));
                        var opSltAtual = $('#sltOp');
                        if (dados.fila.length > 0) {
                            opSltAtual.prop('disabled', false);
                            opSltAtual.html($('<option>').text('Selecione...').attr('value', ''));
                            dados.fila.forEach(function (o) {
                                opSltAtual.append($('<option>')
                                    .attr('value', o.Id)
                                    .text(o.Id));
                            });
                        }
                        else {
                            var div = opSltAtual.parent('.form-group');
                            div.append(T.iconError().attr('data-content', 'Erro ao obter OPs.'));
                            div.find('.ic-error').popover('show');
                        }
                    }
                }
                //CtxTabFechamentoProducao.gerarTabFechamProducao(maquina);
            },
            error: function () {
                divTable.html(T.divDanger('Ocorreu um erro ao obter as medições.'));
            },
            complete: function () {
                div.find('.fa-spin').remove();
            }
        });
    },
    atualizarMonitorTemporeal: function () {
        _ajaxStatusMonitor = $.ajax({
            type: 'GET',
            url: 'http://' + _ipClpMaquina + '/speed',
            dataType: 'jsonp',
            jsonpCallback: 'funcao',
            crossDomain: true,
            timeout: 3000,
            success: function (result) {
                if (_monitor.op != '') {
                    //if (_monitor.dataIni == null) {
                    //    var trs = $('#exMedicoes>table>tbody>tr:not(.group)');
                    //    var datasOp = [];
                    //    if (trs.length > 0) {
                    //        trs.each(function () {
                    //            if ($(this).find('.jsOp .jsSltOpTab').val() == _monitor.op) {
                    //                datasOp.push(moment($(this).find('.jsPeriodo').attr('data-data-ini'), 'DD/MM/YYYY HH:mm:ss'));
                    //            }
                    //        });
                    //    }
                    //    if (datasOp.length > 0) {
                    //        var dataMenor = datasOp[0];
                    //        datasOp.forEach(function (data) {
                    //            if (data.isBefore(dataMenor)) {
                    //                dataMenor = data;
                    //            }
                    //        });
                    //        _monitor.dataIni = dataMenor;
                    //        if (dataMenor.isBefore(_monitor.inicioPlan)) {
                    //            $('#inicioR').text(dataMenor.format('HH:mm:ss')).addClass('color-red');
                    //        }
                    //        else {
                    //            $('#inicioR').text(dataMenor.format('HH:mm:ss')).addClass('color-cinza');
                    //        }
                    //    }
                    //    else if (dados.medicoes.length > 0) {
                    //        var dataMenor = moment(dados.medicoes[0].DataIni, 'DD/MM/YYYY HH:mm:ss');
                    //        dados.medicoes.forEach(function (m) {
                    //            var data = moment(m.DataIni, 'DD/MM/YYYY HH:mm:ss');
                    //            if (data.isBefore(dataMenor)) {
                    //                dataMenor = data;
                    //            }
                    //        });
                    //        var med = dados.medicoes.find(function (m) {
                    //            if (m.DataIni == dataMenor.format('DD/MM/YYYY HH:mm:ss')) {
                    //                return true;
                    //            }
                    //        });
                    //        if (ultimaMedicao.Quantidade > 0) {
                    //            _monitor.dataIni = dataMenor;
                    //            if (_monitor.inicioPlan.isBefore(dataMenor))
                    //                var classe = 'color-red';
                    //            else
                    //                var classe = 'color-cinza';
                    //            $('#inicioR').text(dataMenor.format('HH:mm:ss')).addClass(classe);
                    //        }
                    //    }
                    //}
                    var tempEntPecas = Number(result.tDecEntrePecas) >= Number(result.tDecUltPNow) ? result.tDecEntrePecas : result.tDecUltPNow;
                    var qtdSegundo = 1000 / tempEntPecas;
                    var velocAtual = qtdSegundo * 60;//em minnuto
                    //var quantProduzida = _monitor.qtdProd + ultimaMedicao.Quantidade;
                    //$('#quantProduzida').text(quantProduzida).addClass('color-cinza');
                    //if (_filaProducao != null && _filaProducao.quantidade > 0 && quantProduzida > 0) {
                    //    var horaAtual = new Date();
                    //    var tempoRestante = 0;
                    //    var quantRestante = _filaProducao.quantidade - quantProduzida;
                    //    if (qtdSegundo > 0 && quantRestante > 0) {
                    //        tempoRestante = (quantRestante * 1) / qtdSegundo;
                    //        var horaTermino = new Date();
                    //        horaTermino.setSeconds(horaAtual.getSeconds() + tempoRestante);

                    //        var h = Util.addZero(horaTermino.getHours());
                    //        var m = Util.addZero(horaTermino.getMinutes());
                    //        var s = Util.addZero(horaTermino.getSeconds());

                    //        var terminoPrevisto = moment(horaTermino.toLocaleString(), 'DD/MM/YYYY HH:mm:ss');

                    //        if (_monitor.fimPlan.isBefore(terminoPrevisto)) {
                    //            var classe = 'color-red';
                    //        }
                    //        else {
                    //            var classe = 'color-cinza';
                    //        }
                    //        $('#terminoR').text(h + ':' + m + ':' + s).addClass(classe);
                    //        $('#quantRestante').text(quantRestante).addClass('color-cinza');
                    //    } else if (quantRestante < 0) {
                    //        $('#quantRestante').text('0').addClass('color-cinza');
                    //    }
                    //}
                    if (_metaPerfProduto != null) {
                        var meta = _metaPerfProduto.valor;
                        if (_metaPerfProduto.unidadeMedida.escalaTempo == 'H') {
                            qtdMetaSegundo = meta / 3600;
                        }
                        else if (_metaPerfProduto.unidadeMedida.escalaTempo == 'M') {
                            qtdMetaSegundo = meta / 60;
                        }
                        else if (_metaPerfProduto.unidadeMedida.escalaTempo == 'S') {
                            qtdMetaSegundo = meta;
                        }
                        var porcentMeta = qtdSegundo * 100 / qtdMetaSegundo;
                        var grafico = document.gauges.get('gfPercentMeta');
                        grafico.update({
                            value: porcentMeta
                        });
                    }
                    $('#velocAtual').text(velocAtual + '/Segundo');//exibir em outro grafico
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                //alert(textStatus + '' + jqXHR.status);
                //pensar em como informar ao usuário que  está ocorrendo um problema na comuniccação em tempo real
            },
            complete: function (xhr, textStatus) {
                if (textStatus != "abort") {
                    _valorSetTimeOutMonitor = setTimeout(Conexao.atualizarMonitorTemporeal, 3000);
                }
            }
        });
    },
    obterMedicoesTempoReal: function () {
        _ajaxStatus = $.ajax({
            type: 'POST',
            url: '/PlugAndPlay/Medicoes/ObterMedicoesTempoReal',
            data: { maquinaId: ServerValues.maquinaId, ultimoGrupo: String(_ultimoGrupo).replace('.', ',') },
            success: function (dados) {
                if (dados.medicoes.length > 0) {
                    var ultimoIndice = dados.medicoes.length - 1;
                    var ultimaMedicao = dados.medicoes[ultimoIndice];
                    var trs = $('#exMedicoes>table>tbody>tr');
                    var tr = trs.eq(trs.length - 1);
                    if (dados.medicoes.length > 1) {
                        $('#exMedicoes div alert-warning').remove();
                        $('#exMedicoes table').removeClass('hide');

                        tr.find('.jsPeriodo').attr('data-data-ini', dados.medicoes[0].DataIni).attr('data-data-fim', dados.medicoes[0].DataFim)
                            //.text(dados.medicoes[0].DataIni.substring(11, 16) + ' - ' + dados.medicoes[0].DataFim.substring(11, 16));
                            .html([
                                $('<div>', { style: 'float:left' }).html([
                                    $('<div>', { style: 'font-size: 12px' }).text(dados.medicoes[0].DataIni.substring(0, 5)),
                                    $('<div>', { class: 'text-primary' }).text(dados.medicoes[0].DataIni.substring(11, 16)),
                                ]),
                                $('<div>', { style: 'float:left; margin: 0 8px 0 8px; position: relative; width: 14px; height: 14px' }).html([
                                    $('<i>', { class: 'fa fa-arrows-h', style: 'display:block; position: absolute; top:11px' })
                                ]),
                                $('<div>', { style: 'float:left' }).html([
                                    $('<div>', { style: 'font-size: 12px' }).text(dados.medicoes[0].DataFim.substring(0, 5)),
                                    $('<div>', { class: 'text-primary' }).text(dados.medicoes[0].DataFim.substring(11, 16))
                                ])
                            ]);
                        tr.find('.jsQuantidade').text(dados.medicoes[0].Quantidade);

                        dados.medicoes.splice(0, 1);

                        _ultimoGrupo = dados.medicoes[dados.medicoes.length - 1].Grupo;
                        //dados.medicoes.splice(ultimoIndice, 1);
                        dados.medicoes.forEach(function (m) {
                            _monitor.qtdProd += m.Quantidade;
                            _monitor.opsQuantAdd.push({ dataIni: m.DataIni, quant: m.Quantidade });
                            Dom.gerarLinhaTabPen(m, _opSlt, _mtSltQuant, _mtSltNoQuant, _turnoSlt, _turmaSlt, ServerValues.maquinaId).appendTo('#exMedicoes>table>tbody').hide().fadeIn(2000);
                        });
                    }
                    else {
                        tr.find('.jsPeriodo').attr('data-data-ini', dados.medicoes[0].DataIni).attr('data-data-fim', dados.medicoes[0].DataFim)
                            //.text(dados.medicoes[0].DataIni.substring(11, 16) + ' - ' + dados.medicoes[0].DataFim.substring(11, 16));
                            .html([
                                $('<div>', { style: 'float:left' }).html([
                                    $('<div>', { style: 'font-size: 12px' }).text(dados.medicoes[0].DataIni.substring(0, 5)),
                                    $('<div>', { class: 'text-primary' }).text(dados.medicoes[0].DataIni.substring(11, 16)),
                                ]),
                                $('<div>', { style: 'float:left; margin: 0 8px 0 8px; position: relative; width: 14px; height: 14px' }).html([
                                    $('<i>', { class: 'fa fa-arrows-h', style: 'display:block; position: absolute; top:11px' })
                                ]),
                                $('<div>', { style: 'float:left' }).html([
                                    $('<div>', { style: 'font-size: 12px' }).text(dados.medicoes[0].DataFim.substring(0, 5)),
                                    $('<div>', { class: 'text-primary' }).text(dados.medicoes[0].DataFim.substring(11, 16))
                                ])
                            ]);
                        tr.find('.jsQuantidade').text(dados.medicoes[0].Quantidade);
                    }
                }
            },
            error: function () {
                //pensar em como informar ao usuário que  está ocorrendo um problema na comunicação em tempo real
            },
            complete: function (xhr, textStatus) {
                if (textStatus != "abort") {
                    _valorSetTimeOut = setTimeout(Conexao.obterMedicoesTempoReal, 5000);
                }
            }
        });
    },
    obterMetasProduto: function (maquinaId, op) {
        $.ajax({
            type: 'POST',
            url: '/PlugAndPlay/Medicoes/ObterMetasProduto',
            data: { opId: op, maquinaId: maquinaId },
            success: function (dados) {
                if (dados.op != null) {
                    _ipClpMaquina = dados.ipClpMaquina;
                    _monitor.op = dados.op.id;
                    var ic = _iconErro.clone().attr('title', 'Não foi possível obter a informação');
                    if (dados.op != null) {
                        $('#nomeProduto').text(dados.op.produtoDesc).addClass('color-cinza');
                        $('#nomeProduto').parents('.panel-ind')
                            .find('.fa-exclamation-circle').remove();
                    }
                    else {
                        $('#nomeProduto').parents('.panel-ind')
                            .append(ic.clone());
                    }
                    if (dados.fp != null) {
                        $('#inicioP').parents('.panel-ind')
                            .find('.fa-exclamation-circle').remove();
                        $('#terminoP').parents('.panel-ind')
                            .find('.fa-exclamation-circle').remove();
                        _monitor.inicioPlan = moment(dados.fp.dataInicioPrevista, 'DD/MM/YYYY HH:mm:ss');
                        _monitor.fimPlan = moment(dados.fp.dataFimPrevista, 'DD/MM/YYYY HH:mm:ss');
                        $('#inicioP').text(dados.fp.dataInicioPrevista.substring(11, 19)).addClass('color-cinza');
                        $('#terminoP').text(dados.fp.dataFimPrevista.substring(11, 19)).addClass('color-cinza');
                    }
                    else {
                        $('#inicioP').parents('.panel-ind')
                            .append(ic.clone());
                        $('#terminoP').parents('.panel-ind')
                            .append(ic.clone());
                    }
                    _metaPerfProduto = dados.tp;
                    _filaProducao = dados.fp;
                    if (_metaPerfProduto == null) {

                    }
                    if (_filaProducao == null) {
                        $('#quantProduzida').parents('.panel-ind')
                            .find('.fa-exclamation-circle').remove();
                        $('#quantRestante').parents('.panel-ind')
                            .find('.fa-exclamation-circle').remove();
                        $('#quantProduzida').parents('.panel-ind')
                            .append(ic.clone());
                        $('#quantRestante').parents('.panel-ind')
                            .append(ic.clone());
                    }
                    var trs = $('#exMedicoes tr');
                    if (trs.length > 0) {
                        trs.each(function () {
                            op = $(this).find('.jsOp select').val();
                            if (op == _monitor.op) {
                                var dataIni = $(this).find('.jsPeriodo').attr('data-data-ini');
                                var quant = Number($(this).find('.jsQuantidade').text());
                                _monitor.opsQuantAdd.push({ dataIni: dataIni, quant: quant });
                                _monitor.qtdProd += quant;
                            }
                        });
                    }
                    Conexao.atualizarMonitorTemporeal();
                }
            },
            error: function () {
                alert('Erro')
            }
        });
    },
    obterMetaOp: function (opId, maquinaId, linhaTab) {
        var icon = $('<i>', { class: 'fa fa-spinner fa-spin' });
        linhaTab.find('.jsMeta').html(icon.clone());

        var icError = $('<i>', { class: 'fa fa-exclamation-circle ic-error' });
        var erroMeta = 'Não foi posível obter a meta da OP selecionada! tente novamente.';
        var erroVel = 'Não é possível calcular a média de velocidade sem obter a meta do produto.';
        var g = [
            $('<div>').html(
                $('<div>').html(
                    icError.clone().attr('title', erroMeta)
                )
            ),
            $('<div>').html(
                $('<div>').html(
                    $('<i>', { class: 'fa fa-refresh', title: 'Obter Meta' })
                )
            ),
        ];

        linhaTab.find('.jsVelocidade').html(icon.clone());
        $.ajax({
            type: 'POST',
            url: '/PlugAndPlay/Medicoes/ObterMetaOp',
            data: { opId: opId, maquinaId: maquinaId },
            success: function (dados) {
                if (dados.tp != null) {
                    var dataIni = moment(linhaTab.find('.jsPeriodo').attr('data-data-ini'), 'DD/MM/YYYY HH:mm:ss');
                    var dataFim = moment(linhaTab.find('.jsPeriodo').attr('data-data-fim'), 'DD/MM/YYYY HH:mm:ss');
                    var periodoSeg = dataFim.diff(dataIni, 'seconds');
                    var quant = linhaTab.find('.jsQuantidade').text();
                    var velMediaS = periodoSeg / quant;
                    var velMediaEscala = 0;
                    if (dados.tp.unidadeMedida.escalaTempo == 'H') {
                        velMediaEscala = velMediaS * 60 * 60;
                    } else if (dados.tp.unidadeMedida.escalaTempo == 'M') {
                        velMediaEscala = velMediaS * 60;
                    } else if (dados.tp.unidadeMedida.escalaTempo == 'S') {
                        velMediaEscala = velMediaS;
                    }

                    if (Math.floor(dados.tp.valor) != dados.tp.valor) {
                        dados.tp.valor = dados.dados.tp.valor.toFixed(1);
                    }
                    if (Math.floor(velMediaEscala) != velMediaEscala) {
                        velMediaEscala = velMediaEscala.toFixed(1);
                    }
                    linhaTab.find('.jsMeta').text(dados.tp.valor + ' U/' + dados.tp.unidadeMedida.escalaTempo);
                    linhaTab.find('.jsVelocidade').text(velMediaEscala + ' U/' + dados.tp.unidadeMedida.escalaTempo);
                }
                else {
                    linhaTab.find('.jsMeta').html(g);
                    linhaTab.find('.jsVelocidade').html(icError.clone().attr('title', erroVel));
                }
            },
            error: function () {
                linhaTab.find('.jsMeta').html(g);
                linhaTab.find('.jsVelocidade').html(icError.clone().attr('title', erroVel));
            }
        });
    },
    gravarMedicao: function (medicao, grupo, linha, reload = false) {
        Util.stausBtsTab(false);
        var icon = $('<i>', { class: 'fa fa-spinner fa-spin' });
        var btn = linha.find('.jsAcoes .jsBtnSalvarMedicao');
        var tdAcoes = linha.find('.jsAcoes');
        linha.find('.fa-check').remove();
        btn.html(icon.clone());
        teste = $.ajax({
            type: 'POST',
            url: '/PlugAndPlay/Medicoes/GravarMedicoes',
            data: { medicaoJson: JSON.stringify(medicao) },
            success: function (dados) {
                if (dados.ok > 0) {
                    if (reload) {
                        Conexao.obterLinhaTempo(medicao.MaquinaId)
                    }
                    else {
                        tdAcoes.find('div').remove();
                        linha.attr('data-id', dados.id);
                        btn.before($('<div>').html(
                            $('<div>').html(
                                $('<i>', { class: 'fa fa-check' })
                            )));
                        linha.find('.jsOp select').prop('disabled', true);
                        linha.find('.jsOcorrencia select').prop('disabled', true);
                        linha.find('.jsBtnAddObsPeriodo').prop('disabled', true);

                        btn.prop('disabled', true);
                        linha.find('.jsBtnDesfazer').prop('disabled', false);

                        linha.prevAll(':not(.warning)').eq(0).find('.jsBtnDesfazer').prop('disabled', true);

                        linha.nextAll(':not(.warning)').eq(0).find('.jsBtnSalvarMedicao').prop('disabled', false);
                    }
                    /*CtxTabFechamentoProducao.gerarTabFechamProducao(medicao.MaquinaId);*///Atualiza a lista de ops finanalizadas no fechamento de produção
                }
                else {
                    tdAcoes.find('div').remove();
                    btn.before($('<div>').html(
                        $('<div>').html(
                            $('<i>', { class: 'fa fa-exclamation-circle ic-error' })
                        )));
                    AlertPage.mostrar("erro", dados.msg);
                }
            },
            error: function () {
                tdAcoes.find('div').remove();
                btn.before($('<div>').html(
                    $('<div>').html(
                        $('<i>', { class: 'fa fa-exclamation-circle ic-error' })
                    )));
                AlertPage.mostrar("erro", 'Ocorreu um erro ao gravar o feedback.');
            },
            complete: function () {
                btn.html($('<i>', { class: 'fa fa-floppy-o' }));
                Util.stausBtsTab(true);
            }
        });
    },
    obterHistorico: function (maqId, data, turno) {
        var div = $('#divTabMedicoes');
        div.html(T.LoadTable());
        $.ajax({
            type: 'POST',
            url: '/PlugAndPlay/Medicoes/ObterHistorico',
            data: { maqId: maqId, data: data, turno: turno },
            success: function (dados) {
                if (dados.medicoes.length > 0) {
                    var table = Dom.gerarEstruturaTab();
                    //> inicio carregar selects de ops e movitos
                    if (dados.ops.length > 0) {
                        var opSlt = $('<select>', { class: 'form-control jsSltOpTab', style: 'width: 90px' }).html([
                            $('<option>')
                                .attr('value', '0')
                                .text('...')
                        ]);
                        dados.ops.forEach(function (o) {
                            opSlt.append($('<option>')
                                .attr('value', o.Id)
                                .text(o.Id));
                        });
                    }
                    else {
                        var opSlt = $('<select>', { class: 'form-control erro' })
                            .prop('disabled', true)
                            .html($('<option>').text('...'));
                    }
                    if (dados.motivos.length > 0) {
                        var mtSltQuant = $('<select>', { class: 'form-control jsSltOpMotivoTab', style: 'width: 90px' }).html([
                            $('<option>')
                                .attr('value', '0')
                                .text('...')
                        ]);
                        var mtSltNoQuant = $('<select>', { class: 'form-control jsSltOpMotivoTab', style: 'width: 90px' }).html([
                            $('<option>')
                                .attr('value', '0')
                                .text('...')
                        ]);
                        dados.motivos.forEach(function (m) {
                            if (m.StatusProducao == 0) {
                                mtSltNoQuant.append($('<option>')
                                    .attr('value', m.Id)
                                    .text(m.Descricao));
                            }
                            else {
                                mtSltQuant.append($('<option>')
                                    .attr('value', m.Id)
                                    .text(m.Descricao));
                            }
                        });
                    }
                    else {
                        var mtSltQuant = $('<select>', { class: 'form-control erro' })
                            .prop('disabled', true)
                            .html($('<option>').text('...'));
                        var mtSltNoQuant = $('<select>', { class: 'form-control erro' })
                            .prop('disabled', true)
                            .html($('<option>').text('...'));
                    }
                    //> gerar a primeira linha de divisão do primeiro grupo de turno e data 
                    var trs = [];
                    var data = dados.medicoes[0].DataIni.substring(0, 10);
                    var turno = dados.medicoes[0].TurnoId;
                    var trDate = $('<tr>', { class: 'warning group' }).html([
                        $('<td>', { style: 'padding: 2px 8px 2px 8px', colspan: '10', class: 'text-center' }).html([
                            $('<div>', { class: 'grupo-tab text-left' }).html([
                                $('<span>', { class: 'font-negrito' }).text('Turno: '),
                                $('<span>', { class: '' }).text(turno)
                            ]),
                            $('<div>', { class: 'grupo-tab text-right' }).html([
                                $('<span>', { class: 'font-negrito' }).text('Data: '),
                                $('<span>', { class: '' }).text(data)
                            ]),
                        ])
                    ]);
                    trs.push(trDate);
                    trs = Dom.gerarTabelaMedPen(dados.medicoes, data, turno, opSlt, mtSltQuant, mtSltNoQuant, maqId, trs)
                    table.find('tbody').html(trs);
                    div.html(table);
                }
                else {
                    div.html(T.divInfo('Nenhuma medição foi encontrada.'));
                }
            },
            error: function () {
                div.html(T.divDanger('Ocorreu um erro ao obter as medições.'));
            },
        });
    }
}
var Dom = {
    gerarEstruturaTab: function () {
        var table = $('<table>').addClass('table table-bordered table-hover').html([
            $('<thead>').html($('<tr>').html([
                $('<th>').text('Período'),
                $('<th>').text('Qtde'),
                //$('<th>').text('OP'),
                $('<th>').text('Ocorrencia'),
                $('<th>').text('Observações').css('display', 'none'),
                $('<th>').text('Turno'),
                $('<th>').text('Turma'),
                $('<th>').text('Acões'),
            ])),
            $('<tbody>')
        ]);
        return table;
    },
    geratLinhaTabPenData: function (m) {
        var trDate = $('<tr>', { class: 'group warning' }).html([
            $('<td>', { style: 'padding: 2px 8px 2px 8px', colspan: '10', class: 'text-center' }).html([
                $('<div>', { class: 'grupo-tab text-left' }).html([
                    $('<span>', { class: 'font-negrito' }).text('Turno: '),
                    $('<span>', { class: '' }).text(m.TurnoId)
                ]),
                $('<div>', { class: 'grupo-tab text-right' }).html([
                    $('<span>', { class: 'font-negrito' }).text('Data: '),
                    $('<span>', { class: '' }).text(m.DataIni.substring(0, 10))
                ]),
            ])
        ]);
    },
    gerarLinhaTabPen: function (m, opSlt, mtSltQuant, mtSltNoQuant, turnoSlt, turmaSlt, maqId) {
        if (m.Quantidade > 0) {
            var sltMotivo = mtSltQuant.clone();
        }
        else {
            var sltMotivo = mtSltNoQuant.clone();
        }
        if (m.OcorrenciaId != '' && m.OcorrenciaId != null) {
            sltMotivo.find('[value="' + m.OcorrenciaId + '"]')
                .prop('selected', true);
        }
        if (m.Observacoes != null) {
            if (m.Observacoes.length > 7) {
                var obsEx = m.Observacoes.substring(0, 7) + '...';
            }
            else if (m.Observacoes != '') {
                var obsEx = m.Observacoes;
            }
            else {
                var obsEx = '-';
            }
        }
        else {
            var obsEx = '-';
        }
        var id = m.Id;
        var tr = $('<tr>', { 'data-id': m.MedicaoId, 'data-maq-id': maqId, 'data-grupo': m.Grupo, 'data-turno': m.TurnoId, 'data-turma': m.TurmaId }).html([
            $('<td>', { class: 'jsPeriodo', 'data-data-ini': m.DataIni, 'data-data-fim': m.DataFim })
                .html([
                    $('<div>', { style: 'width: 123px; display: table' }).html([
                        $('<div>', { style: 'float:left' }).html([
                            $('<div>', { style: 'font-size: 12px' }).text(m.DataIni.substring(0, 5)),
                            $('<div>', { class: 'text-primary' }).text(m.DataIni.substring(11, 16)),
                        ]),
                        $('<div>', { style: 'float:left; margin: 0 8px 0 8px; position: relative; width: 14px; height: 14px' }).html([
                            $('<i>', { class: 'fa fa-arrows-h', style: 'display:block; position: absolute; top:11px' })
                        ]),
                        $('<div>', { style: 'float:left' }).html([
                            $('<div>', { style: 'font-size: 12px' }).text(m.DataFim.substring(0, 5)),
                            $('<div>', { class: 'text-primary' }).text(m.DataFim.substring(11, 16))
                        ])
                    ])
                ]), 
            $('<td>', { class: 'jsQuantidade' }).text(m.Quantidade),
            //$('<td>', { class: 'jsOp' }).html([
            //    $('<div>', { class: 'div-incon' }),
            //    sltOp,
            //]),
        ]);
        tr.append($('<td>', { class: 'jsOcorrencia' }).html([
            $('<div>', { class: 'div-incon' }),
            sltMotivo
        ]));
        //observação
        tr.append([
            $('<td>', { class: 'jsObservacao', 'data-observacao': m.Observacoes }).append([
                $('<div>').html([
                    $('<div>').html(
                        $('<span>').text(obsEx),
                    )
                ]),
                $('<button>', { class: 'btn btn-default jsBtnAddObsPeriodo', style: 'float: right' }).html([
                    $('<i>', { class: 'fa fa-pencil-square-o pull-right' }),
                ]),
            ]).css('display', 'none')
        ]);
        //turno
        var sltTurno = turnoSlt.clone();
        if (m.TurnoId != '') {
            sltTurno.find('option[value="' + m.TurnoId + '"]')
                .prop('selected', true);
            sltTurno.prop('disabled', true);
        }
        tr.append([
            $('<td>', { class: 'jsTurno' }).append([
                sltTurno
            ])
        ]);
        //turma
        var sltTurma = turmaSlt.clone();
        if (m.TurmaId != '') {
            sltTurma.find('option[value="' + m.TurmaId + '"]')
                .prop('selected', true);
            sltTurma.prop('disabled', true);
        }
        tr.append([
            $('<td>', { class: 'jsTurma' }).append([
                sltTurma
            ])
        ]);
        //coluna de ações
        //cria os botoes
        var btnAddObs = $('<button>', { class: 'btn btn-default jsBtnAddObsPeriodo' }).html([
            $('<i>', { class: 'fa fa-pencil-square-o' }),
        ]);

        var btnDividirPeriodo = $('<button>', { type: 'button', class: 'jsBtnDividir btn btn-default', title: 'Dividir Período' }).html(
            $('<i>', { class: 'fa fa-scissors' })
        ).prop('disabled', false);
        var btnCancelarFeedback = $('<button>', { type: 'button', class: 'jsCancelarFeedback btn btn-default', title: 'Excluir' }).html(
            $('<i>', { class: 'fa fa-trash-o' })
        ).prop('disabled', false);
        var btnSalvarMedicao = $('<button>', { type: 'button', class: 'jsBtnSalvarMedicao btn btn-default', title: 'Salvar' }).html(
            $('<i>', { class: 'fa fa-floppy-o' })
        ).prop('disabled', false);
        var btnDesfazer = $('<button>', { type: 'button', class: 'jsBtnDesfazer btn btn-default', title: 'Desfazer Período Alterado' }).html(
            $('<i>', { class: 'fa fa-undo' })
        ).prop('disabled', true)

        //adciona na linha da tabela
        tr.append($('<td>', { class: 'jsAcoes text-right' }).html([
            btnAddObs,
            btnDesfazer,
            btnDividirPeriodo,//desfaz um um periodo dividido
            btnCancelarFeedback,//
            btnSalvarMedicao,//mudar depois
        ]));
        if (!Number.isInteger(Number(m.Grupo))) {
            btnDesfazer.prop('disabled', false)
        }
        if (m.MedicaoId > 0) {
            btnDividirPeriodo.prop('disabled', true);
            tr.find('.jsAcoes').prepend($('<div>').html(
                $('<div>').html(
                    $('<i>', { class: 'fa fa-check' })
                )));
            //tr.find('select').prop('disabled', true);
            //tr.find('.jsBtnAddObsPeriodo').prop('disabled', true);
        }
        return tr;
    },
    gerarTabelaMedPen: function (medicoes, data, turno, opSlt, mtSltQuant, mtSltNoQuant, maqId, trs = []) {
        var btn = true;
        medicoes.forEach(function (m, index) {
            if (m.Quantidade > 0) {
                var sltMotivo = mtSltQuant.clone();
            }
            else {
                var sltMotivo = mtSltNoQuant.clone();
            }
            if (m.OcorrenciaId != '' && m.OcorrenciaId != null) {
                sltMotivo.find('[value="' + m.OcorrenciaId + '"]')
                    .prop('selected', true);
            }
            var sltOp = opSlt.clone();
            if (m.OrdemProducaoId != '' && m.OrdemProducaoId != null) {
                sltOp.find('[value="' + m.OrdemProducaoId + '"]')
                    .prop('selected', true)
            }
            if (m.Observacoes != null) {
                if (m.Observacoes.length > 7) {
                    var obsEx = m.Observacoes.substring(0, 7) + '...';
                }
                else if (m.Observacoes != '') {
                    var obsEx = m.Observacoes;
                }
                else {
                    var obsEx = '-';
                }
            }
            else {
                var obsEx = '-';
            }
            var id = m.Id;
            var tr = $('<tr>', { 'data-id': m.MedicaoId, 'data-maq-id': maqId, 'data-grupo': m.Grupo, 'data-turno': m.TurnoId, 'data-turma': m.TurmaId }).html([
                $('<td>', { class: 'jsPeriodo', 'data-data-ini': m.DataIni, 'data-data-fim': m.DataFim })
                    .text(m.DataIni.substring(11, 16) + ' - ' + m.DataFim.substring(11, 16)),
                $('<td>', { class: 'jsQuantidade' }).text(m.Quantidade),
                $('<td>', { class: 'jsMeta' }).text('-'),
                $('<td>', { class: 'jsVelocidade' }).text('-'),
                //$('<td>', { class: 'jsOp' }).html([
                //    $('<div>', { class: 'div-incon' }),
                //    sltOp,
                //]),
            ]);
            tr.append($('<td>', { class: 'jsOcorrencia' }).html([
                $('<div>', { class: 'div-incon' }),
                sltMotivo
            ]));
            tr.append([
                $('<td>', { class: 'jsObservacao', 'data-observacao': m.Observacoes }).append([
                    $('<div>').html([
                        $('<div>').html(
                            $('<span>').text(obsEx),
                        )
                    ]),
                    $('<button>', { class: 'btn btn-default jsBtnAddObsPeriodo', style: 'float: right' }).html([
                        $('<i>', { class: 'fa fa-pencil-square-o pull-right' }),
                    ]),
                ]),
                $('<td>').text(m.TurmaId)
            ]);
            tr.append($('<td>', { class: 'jsAcoes text-right' }).html([
                $('<button>', { type: 'button', class: 'jsBtnDesfazer btn btn-default' }).html(
                    $('<i>', { class: 'fa fa-undo' })
                ).prop('disabled', true),
                $('<button>', { type: 'button', class: 'jsBtnSalvarMedicao btn btn-default' }).html(
                    $('<i>', { class: 'fa fa-floppy-o' })
                ).prop('disabled', true)
            ]));
            trs.push(tr);
            if (m.MedicaoId > 0) {
                tr.find('.jsAcoes').prepend($('<div>').html(
                    $('<div>').html(
                        $('<i>', { class: 'fa fa-check' })
                    )));
                tr.find('select').prop('disabled', true);
                tr.find('.jsBtnAddObsPeriodo').prop('disabled', true);
            }
            else if (btn && trs[trs.length - 1].attr('data-id') == 0 && trs.length > 1) {
                //trs[trs.length - 2].not('.warning').find('.jsBtnDesfazer').prop('disabled', false);
                tr.find('.jsBtnSalvarMedicao').prop('disabled', false);
                btn = false;
            }
            if (m.MedicaoId > 0) {
                tr.find('.jsBtnDesfazer').prop('disabled', false);
            }

        });
        _ultimaDataInicio = data;
        _ultimoTurno = turno;
        //trs[1].find('.jsBtnSalvarMedicao').prop('disabled', false);
        return trs;
    }
}

var Util = {
    addZero: function (i) {
        if (i < 10) {
            i = "0" + i;
        }
        return i;
    },
    addQuantOpTab: function (opSel, linhaTab) {
        var dataIni = linhaTab.find('.jsPeriodo').attr('data-data-ini');
        if (opSel == _monitor.op) {
            var index = _monitor.opsQuantAdd.findIndex(function (d) {
                return d.dataIni == dataIni;
            });
            if (index == -1) {
                var quant = Number(linhaTab.find('.jsQuantidade').text());
                _monitor.qtdProd += quant;
                _monitor.opsQuantAdd.push({ dataIni: dataIni, quant: quant });
            }
        }
        else {
            var index = _monitor.opsQuantAdd.findIndex(function (d) {
                return d.dataIni == dataIni;
            });
            if (index > -1) {
                var quant = Number(linhaTab.find('.jsQuantidade').text());
                _monitor.qtdProd -= quant;
                _monitor.opsQuantAdd.splice(index, 1);
            }
        }
        if (_monitor.op != '') {
            var trs = $('#exMedicoes>table>tbody>tr:not(.group)');
            var datasOp = [];
            if (trs.length > 0) {
                trs.each(function () {
                    if ($(this).find('.jsOp .jsSltOpTab').val() == _monitor.op) {
                        datasOp.push(moment($(this).find('.jsPeriodo').attr('data-data-ini'), 'DD/MM/YYYY HH:mm:ss'));
                    }
                });
            }
            if (datasOp.length > 0) {
                var dataMenor = datasOp[0];
                datasOp.forEach(function (data) {
                    if (data.isBefore(dataMenor)) {
                        dataMenor = data;
                    }
                });
                _monitor.dataIni = dataMenor;
                if (dataMenor.isBefore(_monitor.inicioPlan)) {
                    $('#inicioR').text(dataMenor.format('HH:mm:ss')).addClass('color-red');
                }
                else {
                    $('#inicioR').text(dataMenor.format('HH:mm:ss')).addClass('color-cinza');
                }
            }
            else {
                _monitor.dataIni = null;
                $('#inicioR').text('00:00:00').removeClass('color-cinza color-red');
            }
        }
    },
    dateFormatAm: function (data) {
        var array1 = data.split('/');
        var array2 = array1[2].split(' ');
        var dataOk = array2[0] + '/' + array1[1] + '/' + array1[0] + ' ' + array2[1];
        return dataOk;
    },
    validarData: function (data) {
        if (data.length != 10) return false;
        // verificando data
        var dia = data.substr(0, 2);
        var barra1 = data.substr(2, 1);
        var mes = data.substr(3, 2);
        var barra2 = data.substr(5, 1);
        var ano = data.substr(6, 4);
        if (data.length != 10 || barra1 != "/" || barra2 != "/" || isNaN(dia) || isNaN(mes) || isNaN(ano) || dia > 31 || dia < 1 || mes > 12 || mes < 1) return false;
        if ((mes == 4 || mes == 6 || mes == 9 || mes == 11) && dia == 31) return false;
        if (mes == 2 && (dia > 29 || (dia == 29 && ano % 4 != 0))) return false;
        if (ano < 1900) return false;
        return true;
    },
    resetarPainel: function () {
        _monitor.op = '';
        _monitor.qtdProd = 0;
        _monitor.dataIni = null;
        _monitor.opsQuantAdd = [];
        var color = 'color-cinza color-red';
        $('#inicioR').text('00:00:00').removeClass(color);
        $('#inicioP').text('00:00:00').removeClass(color);
        $('#terminoR').text('00:00:00').removeClass(color);
        $('#terminoP').text('00:00:00').removeClass(color);
        $('#quantProduzida').text('0').removeClass(color);
        $('#quantRestante').text('0').removeClass(color);
        $('#nomeProduto').text('Indefinido').removeClass(color);
    },
    stausBtsTab: function (status) {
        var st = status ? false : true;
        $('#exMedicoes table select').not('.jsTurnosTab, .jsTurmasTab').prop('disabled', st);
        $('#exMedicoes table button').prop('disabled', st);
    }
};

var T = {
    LoadTable: function () {
        return $('<div>', { class: 'alert alert-info' }).html([
            $('<i>', { class: 'fa fa-circle-o-notch fa-spin fa-3x fa-fw' }),
            $('<span>').text('Obtendo...')
        ]);
    },
    divInfo: function (texto) {
        return $('<div>', { class: 'alert alert-info' }).text(texto);
    },
    divDanger: function (texto) {
        return $('<div>', { class: 'alert alert-danger' }).text(texto);
    },
    divWarning: function (texto) {
        return $('<div>', { class: 'alert alert-warning' }).text(texto);
    },
    iconError: function () {
        return $('<i>', {
            class: 'fa fa-exclamation-circle ic-error',
            'data-container': 'body',
            'data-toggle': 'popover',
            'data-placement': 'top',
            'data-placement': 'top',
            'data-trigger': 'hover',
        });
    },
    icLoad: function () {
        return $('<i>', { class: 'fa fa-circle-o-notch fa-spin fa-3x fa-fw' });
    },
    icErrorSlt() {
        return $('<i>', { class: 'fa fa-exclamation-circle ic-erro ic-alert-slt ic-error' });
    }
}

var ConectEsp = {
    documentRead: function () {
        $(document).on('change', '#exMedicoes .jsSltOpTab', ConectEsp.changeSltOp);
    },
    changeSltOp: function () {
        //'data-op-id': f.opId,
        //'data-seg-transformacao': f.segTransformacao == null ? '' : f.segTransformacao,
        //'data-seg-repeticao': f.seqRepeticao == null ? '' : f.seqRepeticao,
        //'data-pro-id' : f.proId
        var sltOp = $(this);
        var option = sltOp.find(':selected');
        var tr = $(this).parents('tr');
        var op = sltOp.val();
        var maqId = ServerValues.maquinaId;
        var dataIniSetup = tr.find('.jsPeriodo').attr('data-data-ini');
        var seqTrans = option.attr('data-seg-transformacao');
        var seqRep = option.attr('data-seg-repeticao');
        var proId = option.attr('data-pro-id');
        $.ajax({
            type: 'POST',
            url: '/PlugAndPlay/Medicoes/EnviarOpContoladorMaquina',
            data: {
                orderId: op,
                maquinaId: maqId,
                dataIniSetup: dataIniSetup,
                prodId: proId,
                seqTrans: seqTrans,
                seqRep: seqRep
            },
            success: function (resp) {
                if (!resp.ok) {
                    AlertPage.mostrar("erro", resp.msg);
                }
            },
            error: function () {
                AlertPage.mostrar("erro", 'Não foi posível enviar OP ao controlador da máquina monitorada.')
            },
            complete: function () {

            }
        });
    }
}
var ReqTimeOut = {
    ResetAtGrupos: function () {
        if (_ajaxStatus != null) {
            _ajaxStatus.abort();
            clearTimeout(_valorSetTimeOut);
            _valorSetTimeOut = null;
        }
    },
    ResetAtVelocimetro: function () {
        if (_ajaxStatusMonitor != null) {
            _ajaxStatusMonitor.abort();
            clearTimeout(_valorSetTimeOutMonitor);
            _valorSetTimeOutMonitor = null;
        }
    }
}
