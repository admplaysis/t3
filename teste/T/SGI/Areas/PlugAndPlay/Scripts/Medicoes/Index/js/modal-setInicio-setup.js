var ModalSetInicioSetup = (function () {
    $(document).ready(function () {
        $(document).on('hidden.bs.modal', '#modalInicioSetup', function () {
            $('#txtDataIniSetup').data("DateTimePicker").date(null);
            ModalSetInicioSetup.config.jqElement.find('.help-block');
        });
        $('#txtDataIniSetup').datetimepicker({
            inline: true,
            sideBySide: true
        });
    });
    var interface = {
        config: {
            jqElement: $('#modalInicioSetup')
        },
        jqCampos: {
            horaIni: $('#txtDataIniSetup')
        },
        abrir: function (conf) {
            var modal = ModalSetInicioSetup.config.jqElement;
            $(document).off('click', '#modalInicioSetup #btnSalvarHora');
            $(document).on('click', '#modalInicioSetup #btnSalvarHora', function () {
                if (conf.salvarFuncao()) {
                    modal.modal('hide');
                };
            });
            var table = $('<table>', { class: 'table' }).html([
                $('<thead>').html([
                    $('<tr>').html([
                        $('<td>').text('Data'),
                        $('<td>').text('Início'),
                        $('<td>').text('Fim'),
                        $('<td>').text('Quantidade'),
                    ])
                ]),
                $('<tbody>').html([
                    $('<tr>', { class: 'warning' }).html([
                        $('<td>').text(conf.tableTop.data),
                        $('<td>').text(conf.tableTop.inicio),
                        $('<td>').text(conf.tableTop.fim),
                        $('<td>').text(conf.tableTop.quant),
                    ])
                ])
            ])
            modal.find('.divInfoPeriodo').html(table);
            modal.modal();
        }
    }
    return interface;
})();