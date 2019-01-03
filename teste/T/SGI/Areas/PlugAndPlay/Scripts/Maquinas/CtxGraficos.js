var CtxGraficos = (function () {
    var publico = {
        init: function () {
            privado.gerarGraficos();
        }
    }
    var privado = {
        gerarGraficos: function () {
            $.ajax({
                type: 'POST',
                url: 'PlugAndPlay/Maquinas/GerarGraficos',
                data: {},
                success: function (dados) {
                    if (dados.length > 0) {
                        var divGraficos = $('#divGraficos');
                        dados.forEach(function (m, index) {
                            divGrafico.append($('<div>', { id: 'grafico-' + index }));
                            Highcharts.stockChart('grafico-' + index, {
                                rangeSelector: {
                                    buttons: [{
                                        count: 1,
                                        type: 'minute',
                                        text: '1M'
                                    }, {
                                        count: 5,
                                        type: 'minute',
                                        text: '5M'
                                    }, {
                                        type: 'all',
                                        text: 'All'
                                    }],
                                    inputEnabled: false,
                                    selected: 0
                                },
                                title: {
                                    text: 'Live random data'
                                },
                                exporting: {
                                    enabled: false
                                },
                                series: [{
                                    name: 'Random data',
                                    data: m.pontos
                                }]
                            });
                        });
                    }
                },
                error: function () {

                },
                complete: function () {

                }
            });
        }
    }
    return publico;
})();