var ModalAlterarFila = (function () {
    var eventos = {
        clickBtnSalvar: function () {
            $('#mdAlterarFila').modal('hide');
            Carregando.abrir('Alterando');
            $.ajax({
                url: '/PlugAndPlay/FilaProducao/Edit',
                type: "POST",
                data: JSON.stringify({
                    OrderId: $('#mdAltFilaOrder').val(),
                    MaquinaId: $('#mdAltFilaMaquina').val(),
                    ProdutoId: $('#mdAltFilaProduto').val(),
                    SequenciaTansformacao: $('#mdAltFilaSequenciaTran').val(),
                    SequencaRepeticao: $('#mdAltFilaSequenciaRep').val(),
                    DataInicioPrevista: $('#mdAltFilaDataIni').data("DateTimePicker").date().format('YYYY/MM/DD HH:mm:ss'),
                    DataFimPrevista: $('#mdAltFilaDataFim').data("DateTimePicker").date().format('YYYY/MM/DD HH:mm:ss')
                }),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    if (result) {
                        TabelaFilaProducao.carregarTabela();
                    }
                    else {
                        alert('erro')
                    }
                },
                error: function () {
                    alert('erro')
                },
                complete: function () {
                    Carregando.fechar();
                }
            });
        }
    }
    var publico = {
        inicio: function () {
            $('#mdAltFilaDataIni').datetimepicker({
                sideBySide: true,
                showClose: true,
                widgetPositioning: {
                    horizontal: 'auto',
                    vertical: 'bottom'
                },
                useCurrent: false
            });
            $('#mdAltFilaDataFim').datetimepicker({
                sideBySide: true,
                showClose: true,
                widgetPositioning: {
                    horizontal: 'auto',
                    vertical: 'bottom'
                },
                useCurrent: false
            });
            $('#mdAltFilaBtnSalvar').click(eventos.clickBtnSalvar);
            $('#mdAlterarFila').on('hidden.bs.modal', function (e) {
                $('#mdAltFilaDataIni').data().DateTimePicker.date(null);
                $('#mdAltFilaDataFim').data().DateTimePicker.date(null);
            })
        },
        abrir: function (tr) {
            $('#mdAltFilaMaquina').val(tr.attr('data-maq-id'));
            $('#mdAltFilaProduto').val(tr.attr('data-pro-id'));
            $('#mdAltFilaSequenciaTran').val(tr.attr('data-seq-tran'));
            $('#mdAltFilaSequenciaRep').val(tr.attr('data-seq-rep'));
            $('#mdAltFilaOrder').val(tr.attr('data-ord-id'));

            var stringDataIni = tr.attr('data-data-inicio');
            var stringDataFim = tr.attr('data-data-fim');

            //$('#mdAltFilaDataIni').val(stringDataIni);
            $('#mdAltFilaDataIni').data("DateTimePicker").defaultDate(moment(stringDataIni, 'DD/MM/YYYY HH:mm:ss'));
            //$('#mdAltFilaDataFim').val(stringDataFim);
            $('#mdAltFilaDataFim').data("DateTimePicker").defaultDate(moment(stringDataFim, 'DD/MM/YYYY HH:mm:ss'));

            $('#mdAlterarFila').modal();
        }
    }
    return publico;
})();