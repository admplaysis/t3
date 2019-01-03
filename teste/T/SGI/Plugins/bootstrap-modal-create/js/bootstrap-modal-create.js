var Modal = (function () {
    $(document).ready(function () {
        //(1) Evento que ocorre quando fecha a modal
        // 1.1 - Confirmação 
        $(document).on('hidden.bs.modal', '#modalAlertConfirm', function () {
            $(document).off('click', '#modalAlertConfirm .btn-success');
            $('#modalAlertConfirm').remove();
        });
        // 1.2 - Erro
        $(document).on('hidden.bs.modal', '#modalAlertErro', function () {
            $('#modalAlertErro').remove();
        });
        // 1.3 - 
        $(document).on('hidden.bs.modal', '#modalAlertErro', function () {
            $('#modalAlertConfirm').remove();
            $(document).off('click', '#modalAlertErro .btn-danger');
        });
        // 1.4 - Confirmar Exclusao
        $(document).on('hidden.bs.modal', '#modalConfirmExclusao', function () {
            $('#modalConfirmExclusao').remove();
            $(document).off('click', '#modalConfirmExclusao .btn-danger');
        });
    });
    var util = {
        gerarEstruturaModal: function () {
            return $('<div>', { class: 'modal fade  in', id: '' }).html([
                $('<div>', { class: 'modal-dialog' }).html([
                    $('<div>', { class: 'modal-content' }).html([
                        $('<div>', { class: 'modal-header' }).html([
                            $('<button>', { type: 'button', class: 'close', 'data-dismiss': 'modal' }),
                            $('<h4>', { class: 'modal-title' })
                        ]),
                        $('<div>', { class: 'modal-body' }),
                        $('<div>', { class: 'modal-footer' })
                    ])
                ])
            ]);
        }
    }
    var publico = {
        erro: function (msg = 'Ocorreu um erro ao realizar a ação.') {//Erro
            var p;
            if (Array.isArray(msg)) {
                p = [];
                msg.forEach(function (m) {
                    p.push($('<p>').text(m));
                });
            }
            else if (typeof msg == 'string') {
                p = $('<p>').text(msg);
            }
            var modal = $('<div>', { class: 'modal fade  in', id: 'modalAlertErro' }).html([
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
            $('body').append(modal);
            $('#modalAlertErro').modal();
        },
        confirmacao: function (conf) {//Confirmacao
            var classe;
            if (conf.tipo != undefined) {
                if (conf.tipo == 'media')
                    classe = 'modal-dialog';
            } else
                classe = 'modal-dialog modal-sm';
            var modal = $('<div>', { class: 'modal fade  in', id: 'modalAlertConfirm' }).html([
                $('<div>', { class: classe }).html([
                    $('<div>', { class: 'modal-content' }).html([
                        $('<div>', { class: 'modal-header' }).html([
                            $('<button>', { type: 'button', class: 'close', 'data-dismiss': 'modal' }),
                            $('<h4>', { class: 'modal-title' }).text('Confirmação')
                        ]),
                        $('<div>', { class: 'modal-body' }).html(
                            conf.msg
                        ),
                        $('<div>', { class: 'modal-footer' }).html([
                            $('<button>', { type: 'button', class: 'btn btn-link', 'data-dismiss': 'modal' }).text(
                                'CANCELAR'
                            ),
                            $('<button>', { type: 'button', class: 'btn btn-success' }).text(
                                'OK'
                            )
                        ])
                    ])
                ])
            ]);
            $('body').append(modal);
            $('#modalAlertConfirm').modal();

            $(document).on('click', '#modalAlertConfirm .btn-success', function () {
                conf.fnSucesso();
                $('#modalAlertConfirm').modal('hide');
            });
        },
        confExlusao: function (info, fnExcluir) {//confirmar exclusao
            //grid de informacoes exibido no corpo da modal
            var div = $('<div>', { class: 'modal-grid-info' })
            var row = $('<div>', { class: 'row' });
            info.forEach(function (i) {
                row.append([
                    $('<div>', { class: 'col-sm-6 info' }).html([
                        $('<div>', { class: 'titulo '}).text(i.t),
                        $('<div>', { class: 'desc' }).text(i.d)
                    ])
                ])
            });
            div.html(row);
            //resto da modal
            var modal = util.gerarEstruturaModal();
            modal.attr('id', 'modalConfirmExclusao');
            modal.find('.modal-title').html([
               'Confirmação'
            ]);
            modal.find('.modal-body').html([
                div
            ]);
            modal.find('.modal-footer').html([
                $('<button>', { type: 'button', class: 'btn btn-link', 'data-dismiss': 'modal' }).text(
                    'CANCELAR'
                ),
                $('<button>', { type: 'button', class: 'btn btn-danger' }).text(
                    'EXCLUIR'
                )
            ]);
            $('body').append(modal);
            $('#modalConfirmExclusao').modal();
            $(document).on('click', '#modalConfirmExclusao .btn-danger', function () {
                fnExcluir();
                $('#modalConfirmExclusao').modal('hide');
            });
        }
    };
    return publico;
})();