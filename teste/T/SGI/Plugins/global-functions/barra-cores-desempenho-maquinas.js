function BarraCoresDesempenhoMaquinas() {
    //<Propriedades>
    var gauge = document.gauges.get('canvVelocimetroPerformace');
    var percenEcalaTotalSetups = 84;
    //setup
    var divPositionIconeSetup = $('#divPositionIconeSetup');
    var spanTempoGastoSetup = $('#spanTempoGastoSetup');
    var spanIconeTempoGastoSetup = $('#spanIconeTempoGastoSetup');
    //setupAjuste
    var divPositionIconeSetupAjuste = $('#divPositionIconeSetupAjuste');
    var spanTempoGastoSetupAjuste = $('#spanTempoGastoSetupAjuste');
    var spanIconeTempoGastoSetupAjuste = $('#spanIconeTempoGastoSetupAjuste');
    //<performace>
    this.ConfCoresVelocimentroPerformace = function (target) {
        //define as cores do grafico de velocimetro
        var minAmarelo = 0, minVerde = 0, maxVerde = 0;
        if (target.ProximaMetaPerformace > 0) {
            minAmarelo = (target.PerformaceMinAmarelo * 100) / target.ProximaMetaPerformace;
            minVerde = (target.PerformaceMinVerde * 100) / target.ProximaMetaPerformace;
            maxVerde = (target.PerformaceMaxVerde * 100) / target.ProximaMetaPerformace;
        }
        if (minAmarelo != 0 && minVerde != 0 && maxVerde != 0) {
            //usa max verde como referencia para definir a o valor maximo da escala do grafico
            var gaugeMaxPerEscalaTemp = maxVerde + 20;

            var gaugeDifEntreValoresDaEscala = 10;

            if (gaugeMaxPerEscalaTemp > 100 && gaugeMaxPerEscalaTemp < 150) {
                gaugeDifEntreValoresDaEscala = 15;
            }
            else if (gaugeMaxPerEscalaTemp < 200) {
                gaugeDifEntreValoresDaEscala = 15;
            }
            else if (gaugeMaxPerEscalaTemp < 400) {
                gaugeDifEntreValoresDaEscala = 30;
            }
            else if (gaugeMaxPerEscalaTemp < 600) {
                gaugeDifEntreValoresDaEscala = 45;
            }
            else {
                gaugeDifEntreValoresDaEscala = 70;
            }
            var majorTicks = [];//array que representa todos os valores mostrados composição visual da escala
            var gaugeUltimoValCalculadoDaEscala = 0;
            for (var i = 0; i < gaugeMaxPerEscalaTemp / gaugeDifEntreValoresDaEscala; i++) {
                majorTicks.push(gaugeUltimoValCalculadoDaEscala);
                gaugeUltimoValCalculadoDaEscala += gaugeDifEntreValoresDaEscala;
            }
            //adicona sempre mais um valor que vai ser o ultimo ou maximo da escala
            majorTicks.push(gaugeUltimoValCalculadoDaEscala);

            gauge.update({
                maxValue: gaugeUltimoValCalculadoDaEscala,//define o valor maximo da escla para o ultimo valor inserido no array "majorTicks"
                majorTicks: majorTicks,
                highlights: [
                    { from: 0, to: minAmarelo, color: '#d9534f' },
                    { from: minAmarelo, to: minVerde, color: '#f0ad4e' },
                    { from: minVerde, to: maxVerde, color: '#5cb85c' },
                    { from: maxVerde, to: gaugeUltimoValCalculadoDaEscala, color: '#337ab7' },
                ]
            });
        }
    }
    //<setup>
    this.ConfCoresBarraSetup = function (target) {
        var setupMaxAmarelo = target.SetupMaxAmarelo;
        //percentual das cores para a barra de progresso 
        var percMinVerdeSetup = (target.SetupMinVerde * percenEcalaTotalSetups) / setupMaxAmarelo;
        var percMaxVerdeSetup = (target.SetupMaxVerde * percenEcalaTotalSetups) / setupMaxAmarelo;
        var percMaxAmareloSetup = (target.SetupMaxAmarelo * percenEcalaTotalSetups) / setupMaxAmarelo;
        //define as porcentagens da barra de progresso do setup para todas as cores
        $('#divProgressSetupAzul').css('width', percMinVerdeSetup + '%');
        $('#divProgressSetupVerde').css('width', percMaxVerdeSetup - percMinVerdeSetup + '%');
        $('#divProgressSetupAmarelo').css('width', percMaxAmareloSetup - percMaxVerdeSetup + '%');
        $('#divProgressSetupVermelho').css('width', percMaxAmareloSetup == 0 ? 0 : 100 - percMaxAmareloSetup + '%');
        //denine as descricao do intervalo que é a meta de setup 
        $('#spanTempoSegundoSetupMinVerde').text(GlobalFunctions.secondsToTimeDinamic(target.SetupMinVerde));
        $('#spanTempoSegundoSetupMaxVerde').text(GlobalFunctions.secondsToTimeDinamic(target.SetupMaxVerde));
    }
    //<setup Ajuste>
    this.ConfCoresBarraSetupAjuste = function (target) {
        var setupAjusteMaxAmarelo = target.SetupAjusteMaxAmarelo;
        //percentual das cores para a barra de progresso 
        var percMinVerdeSetupA = (target.SetupAjusteMinVerde * percenEcalaTotalSetups) / setupAjusteMaxAmarelo;
        var percMaxVerdeSetupA = (target.SetupAjusteMaxVerde * percenEcalaTotalSetups) / setupAjusteMaxAmarelo;
        var percMaxAmareloSetupA = (target.SetupAjusteMaxAmarelo * percenEcalaTotalSetups) / setupAjusteMaxAmarelo;
        //define as porcentagens da barra de progresso do setup ajuste para todas as cores
        $('#divProgressSetupAjusteAzul').css('width', percMinVerdeSetupA + '%');
        $('#divProgressSetupAjusteVerde').css('width', percMaxVerdeSetupA - percMinVerdeSetupA + '%');
        $('#divProgressSetupAjusteAmarelo').css('width', percMaxAmareloSetupA - percMaxVerdeSetupA + '%');
        $('#divProgressSetupAjusteVermelho').css('width', percMaxAmareloSetupA == 0 ? 0 : 100 - percMaxAmareloSetupA + '%');
        //denine as descricao do intervalo que é a meta de setup e setup ajuste
        $('#spanTempoSegundoSetupAjusteMinVerde').text(GlobalFunctions.secondsToTimeDinamic(target.SetupAjusteMinVerde));
        $('#spanTempoSegundoSetupAjusteMaxVerde').text(GlobalFunctions.secondsToTimeDinamic(target.SetupAjusteMaxVerde));
    }
    this.definirValorSetup = function (tempoDecorridoSetup, setupAjusteMaxAmarelo) {
        //atualiza a posição do icone de seta que indica o tempo de setup decorrido
        spanTempoGastoSetup.text(GlobalFunctions.secondsToTimeDinamic(tempoDecorridoSetup));
        var percentTempGastoSetup = (tempoDecorridoSetup * percenEcalaTotalSetups) / setupAjusteMaxAmarelo;//calcula porcentagem do tempo gasto em setup em relação a meta
        if (percentTempGastoSetup > 98) {
            percentTempGastoSetup = 98;
        }
        if (percentTempGastoSetup >= 50) {
            spanIconeTempoGastoSetup.before(spanTempoGastoSetup.css('margin-right', '4px'));
            var widthSpanTempoGastoSetup = spanTempoGastoSetup.width();
            divPositionIconeSetup.css('width', 'calc(' + percentTempGastoSetup + '% - ' + widthSpanTempoGastoSetup + 'px - 8px - 4px - 7px)');
        }
        else {
            spanIconeTempoGastoSetup.after(spanTempoGastoSetup.css('margin-left', '4px'));
            divPositionIconeSetup.css('width', 'calc(' + percentTempGastoSetup + '% - 4px - 7px)');
        }
    }
    this.definirValorSetupAjuste = function (tempoDecorridoSetupA, setupAjusteMaxAmarelo) {
        //atualiza a posição do icone de seta que indica o tempo de setup ajuste decorrido
        spanTempoGastoSetupAjuste.text(GlobalFunctions.secondsToTimeDinamic(tempoDecorridoSetupA));
        var percentTempGastoSetupAjuste = (tempoDecorridoSetupA * percenEcalaTotalSetups) / setupAjusteMaxAmarelo;//calcula porcentagem do tempo gasto em setup em relação a meta
        if (percentTempGastoSetupAjuste > 98) {
            percentTempGastoSetupAjuste = 98;
        }
        if (percentTempGastoSetupAjuste >= 50) {
            spanIconeTempoGastoSetupAjuste.before(spanTempoGastoSetupAjuste.css('margin-right', '4px'));
            var widthSpanTempoGastoSetupAjuste = spanTempoGastoSetupAjuste.width();
            divPositionIconeSetupAjuste.css('width', 'calc(' + percentTempGastoSetupAjuste + '% - ' + widthSpanTempoGastoSetupAjuste + 'px - 8px - 4px - 7px)');
        }
        else {
            spanIconeTempoGastoSetupAjuste.after(spanTempoGastoSetupAjuste.css('margin-left', '4px'));
            divPositionIconeSetupAjuste.css('width', 'calc(' + percentTempGastoSetupAjuste + '% - 4px - 7px)');
        }
    }  
    this.definirValorPerformace = function (percProjVeloc, metaPerformace) {
        //atualiza grafico indica uma projeção de qual será o resultado ao final da op, de acordo com a velocidade atual.
        var percentVelocmetro = 0
        if (metaPerformace != 0)
            percentVelocmetro = (percProjVeloc * 100) / metaPerformace
        gauge.update({
            value: percentVelocmetro
        }); 
    }  
}