var GaugePerformaceProducao = (function () {
    var util = {
        getInstance: function (htmlId) {

        }
    }
    var publico = {
        inicio: function () {

        },
        criar: function () {
            var divChart = $('#divCanvVelocimetroPerformace');
            var chartSize = 200;
            if (divChart.width() < divChart.height()) {
                if (divChart.width() <= 200) {
                    chartSize = divChart.width();
                }
            }
            else {
                if (divChart.height() <= 200) {
                    chartSize = divChart.height();
                }
            }
            var gfVelocidade = new RadialGauge({
                renderTo: 'canvVelocimetroPerformace',
                title: 'Performance',
                width: chartSize,
                height: chartSize,
                units: "%",
                minValue: 0,
                maxValue: 120,
                majorTicks: [0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120],
                minorTicks: 2,
                strokeTicks: false,
                highlights: [{ from: 0, to: 150, color: '#DDD' }],
                colorPlate: "#fff",

                colorMajorTicks: '#fff',
                colorMinorTicks: '#ddd',

                borderShadowWidth: 0,
                borders: true,
                needleType: "arrow",
                needleWidth: 2,
                needleCircleSize: 10,
                needleCircleOuter: true,
                needleCircleInner: false,
                animationDuration: 1500,
                animationRule: "linear"
            }).draw();
            var chart = $('#canvVelocimetroPerformace');
            $(window).resize(function () {
                var chartSize = 200;
                if (divChart.width() < divChart.height()) {
                    if (divChart.width() <= 200) {
                        chartSize = divChart.width();
                    }
                }
                else {
                    if (divChart.height() <= 200) {
                        chartSize = divChart.height();
                    }
                }
                gfVelocidade.update({
                    width: chartSize,
                    height: chartSize,
                });
            });
        },
        update: function (htmlid, conf) {
            var gauge = document.gauges.get('canvVelocimetroPerformace');
        },
    }
    return publico;
})();