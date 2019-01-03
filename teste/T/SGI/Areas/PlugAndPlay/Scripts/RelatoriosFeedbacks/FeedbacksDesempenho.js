$(document).ready(function () {
    $('.datepicker').datepicker({
        format: 'dd/mm/yyyy',
        language: 'pt-BR'
    });
    $('.datepicker').datepicker({
        format: 'dd/mm/yyyy',
        language: 'pt-BR'
    });

    $('.table-responsive').responsiveTable({
        addFocusBtn: false,
        i18n: { focus: 'Foco', display: 'Exibir', displayAll: 'Mostrar Tudo' }
    });
    obterFeedbacksDesempenho();
    $('#btnFiltar').click(clickbtnFiltrar);

    function clickbtnFiltrar() {
        obterFeedbacksDesempenho();
    }

    function obterFeedbacksDesempenho() {
        $.post("/PlugAndPlay/RelatoriosFeedbacks/ObterFeedbacksDeDesenpenho", {
            maquina: $('#maquinaId').val() == "" ? null : $('#maquinaId').val(),
            dataDe: $('#dataDe').val() == "" ? null : $('#dataDe').val(),
            dataAte: $('#dataAte').val() == "" ? null : $('#dataAte').val(),
            tipo: $('#tipo').val() == "" ? null : $('#tipo').val(),
            status: $('#status').val() == "" ? null : $('#status').val()
        }).done(function (result) {
            var tab = $('#tabTargets');
            var trs = []; 
            if (result.data.length > 0) {
                result.data.forEach(function (t) {
                    var tr = $('<tr>');
                    var cont = 13;
                    t.forEach(function (item, index) {
                        tr.append($('<td>', { 'data-priority': cont - index, 'data-columns': 'tabTargets-col-id' + index }).text(item))
                    })
                    trs.push(tr);
                })
                tab.find('tbody').html(trs);
            }
            else {
                tab.find('tbody').html($('<tr>').html($('<td>', {colspan: '14'}).text('Nenhum resultado.')));
            }
            
        }).fail(function () {
            alert('erro')
        })
    }
});
