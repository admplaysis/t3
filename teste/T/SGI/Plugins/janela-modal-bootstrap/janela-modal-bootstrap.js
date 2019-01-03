var Modal = (function () {
    var util = {
        gerarEstrModal: function (confg) {
            return $('<div>', { class: 'modal fade  in', id: 'modalBootStrap' }).html([
                $('<div>', { class: 'modal-dialog' }).html([
                    $('<div>', { class: 'modal-content' }).html([
                        $('<div>', { class: 'modal-header' }).html([
                            $('<button>', { type: 'button', class: 'close', 'data-dismiss': 'modal' }),
                            $('<h4>', { class: 'modal-title' }).text('Erro')
                        ]),
                        $('<div>', { class: 'modal-body' }).html(
                            p
                        ),
                        $('<div>', { class: 'modal-footer' }).html([
                            $('<button>', { type: 'button', class: 'btn btn-success', 'data-dismiss': 'modal' }).text(
                                'OK'
                            )
                        ])
                    ])
                ])
            ]);
        }
    }
    var acoes = {

    }
})();