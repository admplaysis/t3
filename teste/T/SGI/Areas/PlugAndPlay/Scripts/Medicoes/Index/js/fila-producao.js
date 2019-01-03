var FilaProducao = new function () {
    //<documento pronto>
    this.documentRead = function () {
        //botoes
        $(document).on('click', '.btnDesfazerOp', clickBtnDesfazerOP);
        $(document).on('click', '.btnEncerrarOp', ClickBtnEncerrarOp);
        $(document).on('click', '.btnProduzindoOp', ClickbtnProduzindoOp);
        $(document).on('click', '.btnDetalhesOp', ClckBtnDetalhesOp);
    }

    //<eventos> Clicks botoes
    function ClckBtnDetalhesOp() {
        var btn = $(this);
        $.get($UrlLocal.concat('ObterObterOrdemProducao'), {
            maqId: btn.attr('data-maquina'),
            proId: btn.attr('data-produto'),
            order: btn.attr('data-order'),
            seqRep: btn.attr('data-seqRep'),
            seqTran: btn.attr('data-seqTran'),
            movId: btn.attr('data-movId')
        }).done(function (result) {

        }).fail(function () {

        }).always(function () {

        })
    }
    function clickBtnDesfazerOP() {// botao desfazer op
        var btn = $(this);
        Modal.confirmacao({
            msg: 'Essa ação desfaz todos os feedbacks de producão, quantidade e performance relacionados a esta OP.',
            fnSucesso: function () {
                openAnLoad()
                $.post($UrlLocal.concat('DesfazerOpProduzida'), {
                    maquina: btn.attr('data-maquina'),
                    produto: btn.attr('data-produto'),
                    order: btn.attr('data-order'),
                    seqRep: btn.attr('data-seqRep'),
                    seqTran: btn.attr('data-seqTran'),
                    movId: btn.attr('data-movId')
                }).done(function (result) {
                    if (result.status) {
                        Global.atualizarPaginaAjax(ServerValues.maquinaId);
                    }
                    else {
                        AlertPage.erro('Ocorreu um erro que impediu desfazer os feedbaks desta OP.');
                    }
                }).fail(function () {
                    AlertPage.mostrar('erro', "Ocorreu um erro ao desfazer o OP.");
                }).always(function () {
                    closeAnLoad();
                });
            }
        });
    }
    function ClickBtnEncerrarOp() {
        var btn = $(this);
        var op = {
            maquina: btn.attr('data-maqId'),
            produto: btn.attr('data-proId'),
            order: btn.attr('data-order'),
            seqRep: btn.attr('data-seqRep'),
            seqTran: btn.attr('data-seqTran')
        }
        openAnLoad();
        $.get($UrlLocal.concat('VerificarSeOpTemFeedback'), op).done(function (result) {
            if (result) {
                var url = $UrlLocal + "index?id=" + ServerValues.maquinaId;
                var urlEncode = encodeURIComponent(url);//codifica a url atual
                document.location.href = $UrlLocal.concat('FeedbackQuantidade?order=' + op.order
                    + ' &maq=' + op.maquina + ' &seqTran=' + op.seqTran
                    + ' &seqRep=' + op.seqRep + ' &produto=' + op.produto + ' &url=' + urlEncode)
            }
            else {
                AlertPage.mostrar('erro', 'Não é possivel apontar a produção sem salvar feedbacks para esta OP.');
            }
        }).fail(function () {
            AlertPage.mostrar('erro', 'Ocorreu um erro ao validar OP');
        }).always(function () {
            closeAnLoad();
        })
    }

    function ClickbtnProduzindoOp() {
        var btn = $(this);
        var op = {
            maquina: btn.attr('data-maqId'),
            produto: btn.attr('data-proId'),
            order: btn.attr('data-order'),
            seqRep: btn.attr('data-seqRep'),
            seqTran: btn.attr('data-seqTran')
        }
        openAnLoad();
        $.get($UrlLocal.concat('SetProduzindo'), op).done(function (result) {
            if (result) {
                /*var url = $UrlLocal + "index?id=" + ServerValues.maquinaId;
                var urlEncode = encodeURIComponent(url);//codifica a url atual
                document.location.href = $UrlLocal.concat('FeedbackQuantidade?order=' + op.order
                    + ' &maq=' + op.maquina + ' &seqTran=' + op.seqTran
                    + ' &seqRep=' + op.seqRep + ' &produto=' + op.produto + ' &url=' + urlEncode)*/
                debugger
                Global.atualizarPaginaAjax(ServerValues.maquinaId);
            }
            else {
                AlertPage.mostrar('erro', 'Não é possivel apontar a produção sem salvar feedbacks para esta OP.');
            }
        }).fail(function () {
            AlertPage.mostrar('erro', 'Ocorreu um erro ao validar OP');
        }).always(function () {
            closeAnLoad();
        })
    }


    //<>

    this.obterFila = function (maquina) {
        openAnLoad();
        $.get($UrlLocal + 'ObterFilaProducao', {
            maquina: maquina
        }).done(function (result) {
            var url = $UrlLocal + "index?id=" + ServerValues.maquinaId;
            var urlEncode = encodeURIComponent(url);//codifica a url atual
            $('#divFilaProducao').empty();
            $('#divFilaProducaoHistorico').empty();
            $('#divGridListTargetsPendentes').empty();
            debugger
            if (result.fila.length > 0) {
                var conta = 0;
                var list = [];
                result.fila.forEach(function (f, index) {
                    var addBtn = false;
                    /*if (index == 0)
                        addBtn = true
             */
                    if (f.produzindo == 1)
                        addBtn = true
                    var divGridList = $('<div>', { class: 'grid-list' }).html([
                        $('<div>', { class: 'conteudo' }).html([
                            $('<div>', { class: 'row' }).html([
                                $('<div>', { class: 'col-xs-12 col-sm-4' }).html([
                                    $('<div>', { class: 'titulo' }).text(f.order.toUpperCase() + ' ' + f.seqTran + ' ' + f.seqRep)
                                ]),
                                $('<div>', { class: 'col-xs-12 col-sm-8' }).html([
                                    $('<div>', { class: 'info' }).text(f.proId + ' ' + f.proDesc.toUpperCase())
                                ]),
                                $('<div>', { class: 'col-xs-12 col-sm-4' }).html([
                                    $('<div>', { class: 'info' }).text('Qtd: ' + f.qtd),
                                ]),
                                $('<div>', { class: 'col-xs-12 col-sm-8' }).html([
                                    $('<div>', { class: 'info' }).html([
                                        $('<span>').text('Previsão: '),
                                        $('<span>', { class: 'text-primary' }).text(f.dataInicio)
                                    ]),
                                ])
                            ])
                        ])
                    ]);

                    if (conta < result.pulaFila) {
                        divGridList.find('.conteudo .row').append(
                            $('<div>', { class: 'col-xs-1' }).html([
                                $('<div>', { class: 'acoes' }).html([
                                    $('<button>', {
                                        title: 'OP Produzindo',
                                        class: 'btn btn-success btnProduzindoOp btn-xs',
                                        'data-maqId': f.maqId,
                                        'data-order': f.order,
                                        'data-seqTran': f.seqTran,
                                        'data-seqRep': f.seqRep,
                                        'data-proId': f.proId
                                    }).html(
                                        'P'
                                        //$('<i>', { class: 'fa fa-pencil-square-o btn-incon-azul' })
                                    ),
                                ])
                            ])
                        );
                    }

                    if (addBtn) 
                        divGridList.find('.conteudo .row').append(
                            $('<div>', { class: 'col-xs-6' }).html([
                                $('<div>', { class: 'acoes' }).html([
                                    $('<button>', {
                                        title: 'Apontar Produção',
                                        class: 'btn btn-success btnEncerrarOp btn-xs',
                                        'data-maqId': f.maqId,
                                        'data-order': f.order,
                                        'data-seqTran': f.seqTran,
                                        'data-seqRep': f.seqRep,
                                        'data-proId': f.proId
                                    }).html(
                                        'Apontar Produção'
                                        //$('<i>', { class: 'fa fa-pencil-square-o btn-incon-azul' })
                                        ),
                                ])
                            ])
                        );
                    
                    conta++;

                    list.push(divGridList)
                });
                $('#divFilaProducao').html(list);
            }
            if (result.filaHistorico.length > 0) {
                var listHistorico = [];
                result.filaHistorico.forEach(function (f, index) {
                    var addBtn = false;
                    if (index == 0)
                        addBtn = true
                    var divGridListHistorico = $('<div>', { class: 'grid-list' }).html([
                        $('<div>', { class: 'conteudo' }).html([
                            $('<div>', { class: 'row' }).html([
                                $('<div>', { class: 'col-xs-12 col-sm-4' }).html([
                                    $('<div>', { class: 'titulo' }).text(f.order.toUpperCase() + ' ' + f.seqTran + ' ' + f.seqRep)
                                ]),
                                $('<div>', { class: 'col-xs-12 col-sm-8' }).html([
                                    $('<div>', { class: 'info' }).text(f.proId + ' ' + f.proDesc.toUpperCase()),
                                ]),
                                $('<div>', { class: 'col-xs-12 col-sm-4' }).html([
                                    $('<div>', { class: 'info' }).text(f.usuarioNome),
                                ]),
                                $('<div>', { class: 'col-xs-12 col-sm-8' }).html([
                                    $('<div>', { class: 'info' }).text('Turno: ' + f.turno),
                                ]),
                                $('<div>', { class: 'col-xs-12 col-sm-4' }).html([
                                    $('<div>', { class: 'info' }).text('Qtd: ' + f.qtdProduzida),
                                ]),
                                $('<div>', { class: 'col-xs-12 col-sm-8' }).html([
                                    $('<div>', { class: 'info' }).html([
                                        $('<span>').text('Data: '),
                                        $('<span>', { class: 'text-primary' }).text(f.dataDaProducao)
                                    ])
                                ])
                            ])
                        ])
                    ]);
                    if (addBtn)
                        divGridListHistorico.find('.conteudo .row').append(
                            $('<div>', { class: 'col-xs-12' }).html([
                                $('<div>', { class: 'acoes' }).html([
                                    $('<button>', {
                                        title: 'Defazer',
                                        class: 'btn btn-xs btnDesfazerOp btn-danger',
                                        'data-maquina': f.maqId,
                                        'data-produto': f.proId,
                                        'data-order': f.order,
                                        'data-seqRep': f.seqRep,
                                        'data-seqTran': f.seqTran,
                                        'data-movId': f.movId
                                    }).html(
                                        'Desfazer'
                                        /*$('<i>', { class: 'fa fa-undo btn-incon-azul' })*/
                                        )
                                ])
                            ])
                        );

                    listHistorico.push(divGridListHistorico)
                });
                $('#divFilaProducaoHistorico').html(listHistorico);
            }
            if (result.opsTarPendentes.length > 0) {
                var listTargetsPendentes = [];
                result.opsTarPendentes.forEach(function (f, index) {
                    var addBtn = false;
                    if (index == 0)
                        addBtn = true
                    var divGridListTargetsPendentes = $('<div>', { class: 'grid-list' }).html([
                        $('<div>', { class: 'conteudo' }).html([
                            $('<div>', { class: 'row' }).html([
                                $('<div>', { class: 'col-xs-12 col-sm-4' }).html([
                                    $('<div>', { class: 'titulo' }).text(f.order.toUpperCase() + ' ' + f.seqTran + ' ' + f.seqRep)
                                ]),
                                $('<div>', { class: 'col-xs-12 col-sm-8' }).html([
                                    $('<div>', { class: 'info' }).text(f.proId + ' ' + f.proDesc.toUpperCase()),
                                ]),
                                $('<div>', { class: 'col-xs-12 col-sm-4' }).html([
                                    $('<div>', { class: 'info' }).text(f.usuarioNome),
                                ]),
                                $('<div>', { class: 'col-xs-12 col-sm-8' }).html([
                                    $('<div>', { class: 'info' }).text('Turno: ' + f.turno),
                                ]),
                                $('<div>', { class: 'col-xs-12 col-sm-4' }).html([
                                    $('<div>', { class: 'info' }).text('Qtd: ' + f.qtdProduzida),
                                ]),
                                $('<div>', { class: 'col-xs-12 col-sm-8' }).html([
                                    $('<div>', { class: 'info' }).html([
                                        $('<span>').text('Data: '),
                                        $('<span>', { class: 'text-primary' }).text(f.dataDaProducao)
                                    ])
                                ])
                            ])
                        ])
                    ]);
                    if (addBtn)
                        divGridListTargetsPendentes.find('.conteudo .row').append(

                            $('<div>', { class: 'col-xs-12' }).html([
                                $('<div>', { class: 'acoes' }).html([
                                    $('<a>', {
                                        title: 'Feedback de performance',
                                        class: 'btn btn-success btn-xs',
                                        href: $UrlLocal + 'FeedbackPerformace?movId=' + f.movId + '&url=' + urlEncode
                                    }).html('Feedback de Desempenho'/*$('<i>', { class: 'fa fa-pencil-square-o btn-incon-azul' })*/)
                                ])
                            ])
                        );
                    listTargetsPendentes.push(divGridListTargetsPendentes)
                });
                $('#divGridListTargetsPendentes').html(listTargetsPendentes);
            }
        }).fail(function () {
            alert('Erro ao obter a fila de produção')
        }).always(function () { closeAnLoad() });
    }
    //<uteis>
    this.limparFila = function () {
        $('#divFilaProducao').html("Selecione a Máquina");
    }
    function openAnLoad() {//abre a informacao "carregando"
        $('#dvLoadFila').show();
    }
    function closeAnLoad() {//fecha a informacao "carregando"
        $('#dvLoadFila').hide();
    }
};