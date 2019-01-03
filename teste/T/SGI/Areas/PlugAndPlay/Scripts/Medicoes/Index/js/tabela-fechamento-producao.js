var CtxTabFechamentoProducao = (function () {
    $(document).ready(function () {
        $('#divOpsPendentes').html(DivAlertMsg.info('Aguardando.'));
        $(document).on('click', '#divOpsPendentes .btn-default', eventos.clickBtnFecharProducao)
    })
    var eventos = {
        clickBtnFecharProducao: function () {
            var url = "/PlugAndPlay/Medicoes?maqId=" + ServerValues.maquinaId + "&p=" + $(document).scrollTop();
            var urlEncode = encodeURIComponent(url);
            var maquiId = $(this).attr('data-maq-id');
            var op = $(this).attr('data-op');
            var turno = $(this).attr('data-turno');
            var seqTranId = $(this).attr('data-seq-tran-id');
            var seqRepId = $(this).attr('data-seq-rep-id');
            var produtoId = $(this).attr('data-produto-id');
            window.location.href = '/PlugAndPlay/Medicoes/FechamentoProducao?op=' + op + '&maq=' + maquiId + '&turno=' + turno + '&seqTran=' + seqTranId + '&seqRep=' + seqRepId + '&produto=' + produtoId + '&url=' + urlEncode;

        }
    }
    var publico = {
        gerarTabFechamProducao: function (maqId) {
            $.ajax({
                type: 'POST',
                url: '/PlugAndPlay/Medicoes/ObterFeedbacksAgupOp',
                data: { maqId: maqId },
                success: function (feedbacks) {
                    if (feedbacks.length > 0) {
                        var table = $('<table>').addClass('table table-bordered table-hover').html([
                            $('<thead>').html($('<tr>').html([
                                $('<th>').text('Pedido'),
                                $('<th>').text('Produto'),
                                $('<th>').text('Início'),
                                $('<th>').text('Fim'),
                                $('<th>').text('Quantidade'),
                                $('<th>').text('...')
                            ])),
                            $('<tbody>')
                        ]);
                        var tbody = table.find('tbody');
                        feedbacks.forEach(function (f) {
                            tbody.append([
                                $('<tr>', { 'data-produto-id': f.produtoId }).html([
                                    $('<td>', { class: 'op' }).text(f.op),
                                    $('<td>', { class: 'prod' }).text(f.produtoDescricao),
                                    $('<td>', { class: 'ini' }).text(f.inicio),
                                    $('<td>', { class: 'fim' }).text(f.fim),
                                    $('<td>', { class: 'quant' }).text(f.quantidade),
                                    $('<td>', { class: 'acoes' }).html([
                                        $('<button>', { 'data-maq-id': f.maqId, 'data-op': f.op, 'data-produto-id': f.produtoId, 'data-turno': f.turno, 'data-seq-tran-id': f.seqTranId, 'data-seq-rep-id': f.seqRepId, class: 'btn btn-default' }).html([
                                            $('<i>', { class: 'fa fa-pencil-square-o' })
                                        ])
                                    ]),
                                ])
                            ])
                        });
                        $('#divOpsPendentes').html(table);
                        _feedsGoup = feedbacks;
                    }
                    else {
                        $('#divOpsPendentes').html(DivAlertMsg.info('Nenhuma OP foi encontrada.'));
                    }
                },
                error: function () {
                    $('#divOpsPendentes').html(DivAlertMsg.info('Ocorreu um erro ao obter as OPs produzidas.'));
                }
            });
        }
    }
    return publico
})();