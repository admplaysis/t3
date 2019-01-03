var TabelaFilaProducao = new function () {
    //(1) variaveis globais ==>
    // 1.1 - 
    //(1) publicos ===>
    this.iniciar = function () {
        $(document).on('click', '#tabFilaProducao .btnEditar', clickBtnEditar);
        $(document).on('click', '#tabFilaProducao .btnExcluir', clickBtnExluir);
        //fecha indicador de atrazo(carregando) na resposta de todas as solicotacoes ajax
    }
    this.carregarTabela = function () {
        //monta tabela fila de produção
        abrirIC();
        $.get(UrlBase + 'ObterFila').done(function (fila) {
            if (fila.length > 0) {
                var trs = [];
                fila.forEach(function (f) {//gera as linhas da tabela
                    trs.push(
                        $('<tr>', {
                            'data-maq-id': f.maquinaId,
                            'data-pro-id': f.produtoId,
                            'data-seq-tran': f.seqTransform,
                            'data-seq-rep': f.seqRepet,
                            'data-ord-id': f.pedidoId,
                            'data-data-inicio': f.inicioPrevisto,
                            'data-data-fim': f.fimPrevisto,
                        }).html([
                            $('<td>', { class: 'maquina' }).text(f.maquina),
                            $('<td>', { class: 'pedido' }).text(f.pedidoId),
                            $('<td>', { class: 'produto' }).text(f.produto),
                            $('<td>', { class: 'inicioPrevisto' }).text(f.inicioPrevisto),
                            $('<td>', { class: 'fimPrevisto' }).text(f.fimPrevisto),
                            $('<td>', { class: 'seqTranform' }).text(f.seqTransform),
                            $('<td>', { class: 'seqTransform' }).text(f.seqRepet),
                            $('<td>', { class: 'qtd' }).text(f.qtd),
                            $('<td>', { class: 'text-right' }).html([
                                $('<div>', { style: 'width: 158px' }).html([
                                    $('<button>', { style: 'margin-left: 5px', class: 'btn btn-default btnEditar' }).html(
                                        $('<i>', { class: 'fa fa-pencil' })),
                                    $('<button>', { style: 'margin-left: 5px', class: 'btn btn-default btnDetalhes' }).html(
                                        $('<i>', { class: 'fa fa-info-circle' })),
                                    $('<button>', { style: 'margin-left: 5px', class: 'btn btn-default btnExcluir' }).html(
                                        $('<i>', { class: 'fa fa-trash' })),
                                ])
                            ]),
                        ]));
                });
                $('#divTabFilaProducao').html(
                    $('<table>', { class: 'table table-hover', id: 'tabFilaProducao' }).html([
                        $('<thead>').html([
                            $('<tr>').html([
                                $('<th>').text('Máquina'),
                                $('<th>').text('Pedido'),
                                $('<th>').text('Produto'),
                                $('<th>').text('Início previsto'),
                                $('<th>').text('Fim Previsto'),
                                $('<th>').text('Seq. Transfor.'),
                                $('<th>').text('Seq. Repetição'),
                                $('<th>').text('Quantidade'),
                                $('<th>').text(''),
                            ])
                        ]),
                        $('<tbody>').html(trs)//adiciona as lnhas com os dados
                    ]),
                );
            }
        }).fail(function () {
            Modal.erro('Ocorreu um erro ao carregar a fila de produção.');
        }).always(function () {
            fecharIC();
        });
    }
    //(2)Eventos de botoes ===>
    function clickBtnEditar() {//botao editar
        var tr = $(this).parents('tr');
        ModalAlterarFila.abrir(tr);
    }
    function clickBtnExluir() {//botao excluir
        var btn = $(this);
        var tr = btn.parents('tr');
        Modal.confExlusao([
            { t: 'Máquina', d: tr.find('.maquina').text() },
            { t: 'Pedido', d: tr.find('.pedido').text() },
            { t: 'Produto', d: tr.find('.produto').text() },
            { t: 'Quantidade', d: tr.find('.qtd').text() },
            { t: 'Inicio Previsto', d: tr.find('.inicioPrevisto').text() },
            { t: 'Fim Previsto', d: tr.find('.fimPrevisto').text() },
        ], function () {
            Carregando.abrir('Excluindo');
            $.post('/PlugAndPlay/FilaProducao/Delete', {
                ord: tr.attr('data-ord-id'),
                prod: tr.attr('data-pro-id'),
                seqTran: tr.attr('data-seq-tran'),
                seqRep: tr.attr('data-seq-rep'),
                maq: tr.attr('data-maq-id'),
            }).done(function (result) {
                TabelaFilaProducao.carregarTabela();
            }).fail(function () {
                Modal.erro('Não foi possivel excluir o ordem de producao.');
            }).always(function () {
                Carregando.fechar();
            });
        });
    }
    //(3) uteis ===>
    function abrirIC() {
        return Carregando.abrirParcial('secaoInfoCarregando');
    }
    function fecharIC() {
        return Carregando.fecharParcial('secaoInfoCarregando');
    }
}