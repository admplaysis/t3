var ApApp = {
    documentReady: function () {
        var _sltMotivoApt = $('<select>', { class: 'form-control' }).html([
            $('<option>').text('Selecione...')
        ]);

        var op = (new URL(document.location)).searchParams.get('op');
        if (op != '') {
            //ApApp.obterMovimentos(op);
        }

        $(document).on('click', '#divOpsPendentes table tbody .btn-success', ApApp.obterMovimentos);
        $(document).on('click', '.btn-add', ApApp.clickBtnAddMov);
        $(document).on('click', '.btn-ex', ApApp.clickBtnExLinhaTab);
        $(document).on('click', '#btnSalvarApontamento', ApApp.clickBtnSalvarApont);
        $(document).on('keyup', '#produtoDesIs', ApApp.evetInputPesquisar);
        $(document).on('click', '#divResultPesq table tr', ApApp.clickTrPesq);
        $(document).on('click', '#btnAddAc', ApApp.clickBtnAddAc);
        $(document).on('keypress', 'input', ApApp.selecionarCampo);
        $(document).on('change', 'select', ApApp.selecionarCampo);
        $('#btnAbrirModal').click(ApApp.clicBtnAbrirModal);
        $('#btnAbrirModalInsumos').click(ApApp.clicBtnAbrirModalInsumos);
        $(document).on('click', '#divResultPesq>ul>li', ApApp.clickProdutoListPesq);

        $('#modalAddPerdaInsumo').on('hidden.bs.modal', function () {
            $('#produtoDesIs').val('').attr('data-id', '');
            $('#quantPerdasIs').val('');
            $('#obsPerdasIs').val('');
            $('#sltMotivoPerdaIs option').first().prop('selected', true);
            $('#divResultPesq').hide();
            ApApp.removerErrosAll();
        })
        $('#modalAddPerdaProduto').on('hidden.bs.modal', function () {
            $('#txtProduto').val('');
            $('#quantPerdasAc').val('');
            $('#obsPerdasAc').val('');
            $('#sltMotivoPerdaAc option').first().prop('selected', true);
            ApApp.removerErrosAll();
        })
    },
    clicBtnAbrirModalInsumos: function () {
        $('#modalAddPerdaInsumo').modal('show');
    },
    clicBtnAbrirModal: function () {
        $('#txtProduto').val($('#txtProdutoDesc').val());
        $('#modalAddPerdaProduto').modal('show');
        //
    },
    clickBtnAddAc: function () {
        var ok = true;
        var quant = $('#quantPerdasIs').val();
        var motivo = $('#sltMotivoPerdaIs').val();
        var obs = $('#obsPerdasIs').val();
        var produto = $('#produtoDesIs').val();
        var produtoId = $('#produtoDesIs').attr('data-id');
        if (produto == '') {
            ok = false;
            ApApp.exibirMensagemErro($('#produtoDesIs')
                , 'Selecione o Produto.');
        }
        if (quant == '') {
            ok = false;
            ApApp.exibirMensagemErro($('#quantPerdasIs')
                , 'Informe a quantidade.');
        }
        else if (!$.isNumeric(quant)) {
            ok = false;
            ApApp.exibirMensagemErro($('#quantPerdasIs')
                , 'Informe somente números.');
        }
        if (motivo == '') {
            ok = false;
            ApApp.exibirMensagemErro($('#sltMotivoPerdaIs')
                , 'Selecione um motivo.');
        }
        if (ok) {
            var divTabPerdasAc = $('#divTabPerdasAc');
            if (divTabPerdasAc.find('table').length > 0) {
                divTabPerdasAc.find('table tbody').prepend(ApApp.gerarLinhaTabAc(0, produtoId, produto, quant, motivo, $('#sltMotivoPerdaIs option:selected').text(), obs))
            }
            else {
                var table = ApApp.gerarTabAc();
                table.find('tbody').html(ApApp.gerarLinhaTabAc(0, produtoId, produto, quant, motivo, $('#sltMotivoPerdaIs option:selected').text(), obs));
                divTabPerdasAc.html(table);
            }
            $('#quantPerdasIs').val('').focus();
            $('#sltMotivoPerdaIs option:selected').prop('selected', false);
            $('#obsPerdasIs').val('');
            $('#modalAddPerdaInsumo').modal('hide');
        }
    },
    clickTrPesq: function () {
        var tr = $(this);
        var id = tr.find('td:nth-child(1)').text();
        var desc = tr.find('td:nth-child(2)').text();
        var input = $('#produtoDesIs').val(desc);
        input.attr('data-id', id);
        ApApp.removerErro($('#produtoDesIs'));
    },
    clickProdutoListPesq: function () {//
        var proDesc = $(this).text();
        var proID = $(this).attr('data-id');
        if (proID != undefined && proID != '') {
            $('#produtoDesIs').val(proDesc).attr('data-id', proID);
        }
        else {
            $('#produtoDesIs').val("").attr('data-id', '')
        }
        $('#divResultPesq').hide();
    },
    evetInputPesquisar: function (e) {
        var pesq = $('#produtoDesIs').val();
        //var key = e.which;
        //if (key == 13)  // the enter key code
        //{

        //}
        $.ajax({
            type: 'POST',
            url: '/PlugAndPlay/Medicoes/ObterProdutos',
            data: { pesquisa: pesq },
            success: function (produtos) {
                var area = $('#divResultPesq');
                if (produtos.length > 0) {
                    var liList = []
                    produtos.forEach(function (p) {
                        liList.push($('<li>').attr('data-id', p.id).text(p.descricao))
                    });
                    area.html($('<ul>', { class: 'list-pesq-produto' }).html(liList));
                    area.show();
                }
                else {
                    area.html($('<ul>', { class: 'list-pesq-produto' }).html([
                        $('<li>').text('Nenhum resultado')
                    ]));
                    area.show();
                }
            },
            error: function () {

            }
        });
    },
    clickBtnExLinhaTab: function () {
        $(this).parents('tr').remove();
        var table = $('#divTabPerdasPos table');
        if (table.find('tbody tr').length == 0) {
            table.remove();
        }
    },
    clickBtnAddMov: function () {
        var ok = true;
        var quant = $('#quantPerdasAc').val();
        var motivo = $('#sltMotivoPerdaAc').val();
        var obs = $('#obsPerdasAc').val();
        if (quant == '') {
            ok = false;
            ApApp.exibirMensagemErro($('#quantPerdasAc')
                , 'Informe a quantidade.');
        }
        else if (!$.isNumeric(quant)) {
            ok = false;
            ApApp.exibirMensagemErro($('#quantPerdasAc')
                , 'Informe somente números.');
        }
        if (motivo == '') {
            ok = false;
            ApApp.exibirMensagemErro($('#sltMotivoPerdaAc')
                , 'Selecione um motivo.');
        }
        if (ok) {
            var divTabPerdasPos = $('#divTabPerdasPos');
            if (divTabPerdasPos.find('table').length > 0) {
                divTabPerdasPos.find('table tbody').prepend(ApApp.gerarLinhaTab(0, quant, motivo, $('#sltMotivoPerdaAc option:selected').text(), obs))
            }
            else {
                var table = ApApp.gerarTab();
                table.find('tbody').html(ApApp.gerarLinhaTab(0, quant, motivo, $('#sltMotivoPerdaAc option:selected').text(), obs));
                divTabPerdasPos.html(table);
            }
            $('#quantPerdasAc').val('').focus();
            $('#sltMotivoPerdaAc option:selected').prop('selected', false);
            $('#obsPerdasAc').val('');
            $('#modalAddPerdaProduto').modal('hide');
            //
        }
    },
    exibirMensagemErro: function (campo, mensagem) {
        var div = campo.parents('.has-feedback').addClass('has-error');
        div.find('span').text(mensagem);
    },
    obterMovimentos: function () {
        debugger
        $.ajax({
            type: 'POST',
            url: '/PlugAndPlay/Medicoes/ObterMovimentosEstoque',
            data: _actionValues.op,
            success: function (movimentos) {
                if (movimentos.length > 0) {
                    var table = ApApp.gerarTab();
                    var table2 = ApApp.gerarTabAc();
                    var perdPos = [];
                    var perdIs = [];
                    movimentos.forEach(function (m) {
                        if (m.tipo == '001') {debugger
                            $('#txtInsuProcQuant').val(m.quantidade);
                            $('#txtInsuProcObs').val(m.observacao);
                            $('#hfInsuProcMovId').val(m.id);
                        }
                        else if (m.tipo == '501') {
                            perdPos.push(ApApp.gerarLinhaTab(m.id, m.quantidade, m.ocorrenciaId, m.ocorrenciaDescricao, m.observacao));
                        }
                        else if (m.tipo == '502') {
                            perdIs.push(ApApp.gerarLinhaTabAc(m.id, m.produtoId, m.produtoDescricao, m.quantidade, m.ocorrenciaId, m.ocorrenciaDescricao, m.observacao));
                        }
                    });
                    if (perdPos.length > 0) {
                        table.find('tbody').html(perdPos);
                        $('#divTabPerdasPos').html(table);
                    }
                    if (perdIs.length > 0) {
                        table2.find('tbody').html(perdIs);
                        $('#divTabPerdasAc').html(table2);
                    }
                }
            },
            error: function () {
                alert('Erro!')
            },
            complete: function () {

            }
        });
    },
    gerarTab: function () {
        return $('<table>', { class: 'table' }).html([
            $('<thead>').html([
                $('<tr>').html([
                    $('<td>').text('Quantidade'),
                    $('<td>').text('Motivo'),
                    $('<td>').text('Obervações'),
                    $('<td>', { class: 'text-right' }).text('...'),
                ])
            ]),
            $('<tbody>')
        ]);
    },
    gerarTabAc: function () {
        return $('<table>', { class: 'table' }).html([
            $('<thead>').html([
                $('<tr>').html([
                    $('<td>').text('Produto'),
                    $('<td>').text('Quantidade'),
                    $('<td>').text('Motivo'),
                    $('<td>').text('Obervações'),
                    $('<td>', { class: 'text-right' }).text('...'),
                ])
            ]),
            $('<tbody>')
        ]);
    },
    gerarLinhaTabAc: function (movId, proId, ProDesc, quant, motivo, motivoDesc, obs) {
        return $('<tr>', { 'data-id': movId }).html([
            $('<td>', { class: 'prod', 'data-produto-id': proId }).text(ProDesc),
            $('<td>', { class: 'quant' }).text(quant),
            $('<td>', { class: 'motivo', 'data-motivo-id': motivo }).text(motivoDesc),
            $('<td>', { class: 'obs' }).text(obs),
            $('<td>', { class: 'text-right' }).html(
                $('<button>', { class: 'btn btn-default btn-ex' }).html([
                    $('<i>', { class: 'fa fa-trash' })
                ])),
        ]);
    },
    gerarLinhaTab: function (movId, quant, motivo, motivoDesc, obs) {
        return $('<tr>', { 'data-id': movId }).html([
            $('<td>', { class: 'quant' }).text(quant),
            $('<td>', { class: 'motivo', 'data-motivo-id': motivo }).text(motivoDesc),
            $('<td>', { class: 'obs' }).text(obs),
            $('<td>', { class: 'text-right' }).html(
                $('<button>', { class: 'btn btn-default btn-ex' }).html([
                    $('<i>', { class: 'fa fa-trash' })
                ])),
        ]);
    },
    clickBtnSalvarApont: function () {debugger
        var qtdMinPrevista = Number($('#hfQtdMinPrevista').val());
        var qtdTotalProduzida = Number($('#txtInsuProcQuant').val());

        var qtdPerdasDeProduto = 0;
        //soma as quantidades das perdas de produto adicionada
        var trs = $('#divTabPerdasPos table tbody tr');
        trs.each(function () {
            qtdPerdasDeProduto += Number($(this).find('.quant').text());
        })
        var qtdProdOpHistorico = Number($('#hfQtdProdOpTotal').val()); 
        var qtdPecaBoaProduzida = qtdTotalProduzida - qtdPerdasDeProduto + qtdProdOpHistorico;
        if (qtdPecaBoaProduzida >= qtdMinPrevista) {
            ApApp.salvarApontamentos(true);
        }
        else {
            ModalEncerramentoOp.abrir(qtdPecaBoaProduzida);
        }
    },
    removerErro: function (input) {
        var div = input.parents('.has-feedback');
        div.removeClass('has-error');
        div.find('span').empty();
    },
    selecionarCampo: function () {
        var campo = $(this);
        ApApp.removerErro(campo);
    },
    removerErrosAll: function () {
        var inputs = $('.has-feedback input, .has-feedback select, .has-feedback textarea')
        inputs.each(function () {
            var div = $(this).parents('.has-feedback');
            div.removeClass('has-error');
            div.find('span').empty();
        })
    },
    salvarApontamentos: function (encerrarOP, obsOpParcial = null, ocoIdOpParcial = null) {
        var maquinaId = $('#hfMaquinaId').val();
        var tab = $('#tabTopInfo');
        var prodId = $('#txtProdutoId').val();
        var op = tab.find('.op').text();
        var movimentos = [];
        var cont = $('#opsQuantGrupo').val();
        movimentos.push({
            Id: $('#hfInsuProcMovId').val(),
            Quantidade: $('#txtInsuProcQuant').val(),
            Observacao: $('#txtInsuProcObs').val(),
            OrderId: op,
            ProdutoId: prodId,
            Tipo: '001',
            OcorrenciaId: null,
            MaquinaId: maquinaId,
            Feedbacks: _actionValues.feedbacks,
            ObsOpParcial: obsOpParcial,
            OcoIdOpParcial: ocoIdOpParcial
        });
        var trs = $('#divTabPerdasPos table tbody tr');
        trs.each(function () {
            var l = $(this);
            movimentos.push({
                Id: l.attr('data-id'),
                Quantidade: l.find('.quant').text(),
                Observacao: l.find('.obs').text(),
                OrderId: op,
                ProdutoId: prodId,
                Tipo: '501',
                OcorrenciaId: l.find('.motivo').attr('data-motivo-id'),
                MaquinaId: maquinaId,
                Feedbacks: _actionValues.feedbacks
            });
        });
        trs = $('#divTabPerdasAc table tbody tr');
        trs.each(function () {
            var l = $(this);
            movimentos.push({
                Id: l.attr('data-id'),
                Quantidade: l.find('.quant').text(),
                Observacao: l.find('.obs').text(),
                OrderId: op,
                ProdutoId: prodId,
                Tipo: '502',
                OcorrenciaId: l.find('.motivo').attr('data-motivo-id'),
                MaquinaId: maquinaId,
                Feedbacks: _actionValues.feedbacks
            });
        });
        var jsonPost = {
            movimentos: movimentos,
            filaStatus: encerrarOP == true ? "EN" : null
        }
        Carregando.abrir();
        $.ajax({
            type: 'POST',
            url: '/PlugAndPlay/Medicoes/GravarApontamentosProducao',
            data: JSON.stringify(jsonPost),
            contentType: "application/json; charset=utf-8",
            success: function (resp) {
                if (resp.ok) {
                    var url = encodeURIComponent((new URL(document.location.href)).searchParams.get('url'));
                    window.location.href = "/PlugAndPlay/Medicoes/FeedbackPerformace?movId=" + resp.id + "&url=" + url;
                }
                else {
                    Carregando.fechar();
                    AlertPage.mostrar("erro", "Não foi possível salvar os dados");
                }
            },
            error: function () {
                Carregando.fechar();
                AlertPage.mostrar("erro", "Não foi possível salvar os dados");
            },
            complete: function () {

            }
        });
    }
};