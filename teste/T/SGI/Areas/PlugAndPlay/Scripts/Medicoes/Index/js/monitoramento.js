var Monitoramento = (function () {
    var monitorTempoReal = {
        ajax: null,
        timeout: null,
        iniciar: function (maquinaId) {
            monitorTempoReal.cancelar();
            util.openAnLoad();
            //=================== obtem as metas e monta as cores dos graficos =========
            $.get($UrlLocal + 'ObterTargetProduto', {
                maqId: maquinaId
            }).done(function (result) {
                var gauge = document.gauges.get('canvVelocimetroPerformace');//instancia do velocimetro
                var metaPerformace = 0; 
                if (result.op != null) {//exibe a na seçao de monitoramento
                    $('#ddOrdemProducao').text(result.op.op);
                }
                if (result.target != null) {//configura as definições de layot que precisa da meta
                    console.log(result.target)
                    $('#divAlertPrimeiraProducao').hide();
                    $('#divLegendaVelocimetro').show();
                    var metaTempoSegundoGastoSetup = result.target.tarSetup;
                    var metaTempoSegundoGastoSetupAjuste = result.target.tarSetpuAjuste;
                    //metas 
                    //percentual das cores para a barra de progresso para setup  
                    var percenEcalaTotalSetups = 84;
                    //var percentEscalaMetaSetup = (result.target.tarSetup * percenEcalaTotalSetups) / result.target.setupMaxAmarelo;
                    //var percentEscalaMetaSetupAjuste = (result.target.tarSetpuAjuste * percenEcalaTotalSetups) / result.target.setupAjusteMaxAmarelo;
                    var setupMaxAmarelo = result.target.setupMaxAmarelo;
                    var setupAjusteMaxAmarelo = result.target.setupAjusteMaxAmarelo;
                    var percMinVerdeSetup = (result.target.setupMinVerde * percenEcalaTotalSetups) / setupMaxAmarelo;
                    var percMinVerdeSetupA = (result.target.setupAjusteMinVerde * percenEcalaTotalSetups) / setupAjusteMaxAmarelo;
                    var percMaxVerdeSetup = (result.target.setupMaxVerde * percenEcalaTotalSetups) / setupMaxAmarelo;
                    var percMaxVerdeSetupA = (result.target.setupAjusteMaxVerde * percenEcalaTotalSetups) / setupAjusteMaxAmarelo;
                    var percMaxAmareloSetup = (result.target.setupMaxAmarelo * percenEcalaTotalSetups) / setupMaxAmarelo;
                    var percMaxAmareloSetupA = (result.target.setupAjusteMaxAmarelo * percenEcalaTotalSetups) / setupAjusteMaxAmarelo;
                    //percentual das cores para a barra de progresso somando o setup com setupAjuste
                    var totalPercMinVerdeSetupSetupA = percMinVerdeSetup + percMinVerdeSetupA;
                    var totalPercMaxVerdeSetupSetupA = percMaxVerdeSetup + percMaxVerdeSetupA;
                    var totalPercMaxAmareloSetupSetupA = percMaxAmareloSetup + percMaxAmareloSetupA;
                    //define as porcentagens da barra de progresso do setup ajuste para todas as cores
                    $('#divProgressSetupAjusteAzul').css('width', percMinVerdeSetupA + '%');
                    $('#divProgressSetupAjusteVerde').css('width', percMaxVerdeSetupA - percMinVerdeSetupA + '%');
                    $('#divProgressSetupAjusteAmarelo').css('width', percMaxAmareloSetupA - percMaxVerdeSetupA + '%');
                    $('#divProgressSetupAjusteVermelho').css('width', percMaxAmareloSetupA == 0 ? 0 : 100 - percMaxAmareloSetupA + '%');
                    //define as porcentagens da barra de progresso do setup para todas as cores
                    $('#divProgressSetupAzul').css('width', percMinVerdeSetup + '%');
                    $('#divProgressSetupVerde').css('width', percMaxVerdeSetup - percMinVerdeSetup + '%');
                    $('#divProgressSetupAmarelo').css('width', percMaxAmareloSetup - percMaxVerdeSetup + '%');
                    $('#divProgressSetupVermelho').css('width', percMaxAmareloSetup == 0 ? 0 : 100 - percMaxAmareloSetup + '%');
                    //denine as descricao do intervalo que é a meta de setup e setup ajuste
                    $('#spanTempoSegundoSetupMinVerde').text(GlobalFunctions.secondsToTimeDinamic(result.target.setupMinVerde));
                    $('#spanTempoSegundoSetupMaxVerde').text(GlobalFunctions.secondsToTimeDinamic(result.target.setupMaxVerde));
                    $('#spanTempoSegundoSetupAjusteMinVerde').text(GlobalFunctions.secondsToTimeDinamic(result.target.setupAjusteMinVerde));
                    $('#spanTempoSegundoSetupAjusteMaxVerde').text(GlobalFunctions.secondsToTimeDinamic(result.target.setupAjusteMaxVerde));
                    //define as cores do grafico de velocimetro
                    var minAmarelo = 0, minVerde = 0, maxVerde = 0;
                    if (result.target.tarPerformace > 0) {
                        minAmarelo = (result.target.performaceMinAmarelo * 100) / result.target.tarPerformace;
                        minVerde = (result.target.performaceMinVerde * 100) / result.target.tarPerformace;
                        maxVerde = (result.target.performaceMaxVerde * 100) / result.target.tarPerformace;
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
                        //adicona sempre mais um valor que vai ser o ultimo e maximo da escala
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
                    metaPerformace = result.target.tarPerformace;//meta em peças por segundo
                }
                else {
                    $('#divAlertPrimeiraProducao').show();
                    $('#divLegendaVelocimetro').hide();
                    util.resetInfo({op: false});
                }

                //================ atualiza as informaçoes de 5 em 5 segundos =============================
                var ddTempoDecorrido = $('#ddTempoDecorrido');
                var ddPecasProduzidas = $('#ddQuantidadePecasProduzidas');
                var ddPerfomaceAtingirMeta = $('#velocidadeParaAtingirMeta');
                var ddPeformaceAtual = $('#velocidadeAtual');

                //setup
                var divPositionIconeSetup = $('#divPositionIconeSetup');
                var spanTempoGastoSetup = $('#spanTempoGastoSetup');
                var spanIconeTempoGastoSetup = $('#spanIconeTempoGastoSetup');
                //setupAjuste
                var divPositionIconeSetupAjuste = $('#divPositionIconeSetupAjuste');
                var spanTempoGastoSetupAjuste = $('#spanTempoGastoSetupAjuste');
                var spanIconeTempoGastoSetupAjuste = $('#spanIconeTempoGastoSetupAjuste');

                var ddQuantidadeRestante = $('#ddQuantidadeRestante');
               
                (function obterDados() {
                    monitorTempoReal.ajax = $.ajax({
                        url: $UrlLocal + 'ObterStatusMaquina',
                        type: 'GET',
                        data: { maquinaId: maquinaId },
                        success: function (result) {
                            console.log(result)
                            if (result.dados != null) {
                                //atualiza grafico indica uma projeção de qual será o resultado ao final da op, de acordo com a velocidade atual.
                                var percentVelocmetro = 0
                                if (metaPerformace != 0)
                                    percentVelocmetro = (result.dados.percProjVeloc * 100) / metaPerformace
                                gauge.update({
                                    value: percentVelocmetro
                                }); 
                                //Atualiza informações secundarias no bloco de monitoramento
                                if (result.dados.qtdPecasRestante >= 0)
                                    ddQuantidadeRestante.text(result.dados.qtdPecasRestante)
                                else
                                    ddQuantidadeRestante.text('0')
                                ddTempoDecorrido.text(GlobalFunctions.secondsToTimeDinamic(result.dados.tempoGastoPerformace + result.dados.tempoDecorridoSetup + result.dados.tempoDecorridoSetupA + result.dados.tempoDecorridoPequenasParadas));
                                ddPecasProduzidas.text(result.dados.qtdPecasProduzidas);
                                ddPerfomaceAtingirMeta.text(Number(result.dados.segunPerfNecessaria).toFixed(1) + ' P/S'); 
                                ddPeformaceAtual.text(Number(result.dados.vlcAtualPcSegundo).toFixed(1) + ' P/S');
                                //atualiza a posição do icone de seta que indica o tempo de setup decorrido
                                spanTempoGastoSetup.text(GlobalFunctions.secondsToTimeDinamic(result.dados.tempoDecorridoSetup));
                                var percentTempGastoSetup = (result.dados.tempoDecorridoSetup * percenEcalaTotalSetups) / setupMaxAmarelo;//calcula porcentagem do tempo gasto em setup em relação a meta
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
                                //atualiza a posição do icone de seta que indica o tempo de setup ajuste decorrido
                                
                                spanTempoGastoSetupAjuste.text(GlobalFunctions.secondsToTimeDinamic(result.dados.tempoDecorridoSetupA));
                                var percentTempGastoSetupAjuste = (result.dados.tempoDecorridoSetupA * percenEcalaTotalSetups) / setupAjusteMaxAmarelo;//calcula porcentagem do tempo gasto em setup em relação a meta
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
                        },
                        error: function () {
                            AlertPage.error('Erro na atualização das informações de monitoramento.');
                        },
                        complete: function (xhr, textStatus) {//chama a mesma função após 5 segundos
                            //o codigo abaixo impede que uma nova solicitação seja feita caso o ajax polling tenha sido cancelado
                            //no momento em que a solicitação ja foi feita e esta aguardando a resposta do servidor
                            if (textStatus != "abort")
                                monitorTempoReal.timeout = setTimeout(obterDados, 500);
                        }
                    });
                })()
            }).fail(function () {
                AlertPage.erro('Erro ao iniciar o monitoramento em tempo real');
            }).always(function () { util.closeAnLoad() });
        },
        cancelar: function () {
            if (monitorTempoReal.ajax != null)//cancela a requisição ajax caso ja tenha sido feita quando o ajax polling for cancelado
                monitorTempoReal.ajax.abort();
            if (monitorTempoReal.timeout != null)//remove a execução agendada do ajax polling 
                clearTimeout(monitorTempoReal.timeout);
        }
    }
    var util = {
        resetInfo: function (param) {
            var hide = param;
            if (hide === undefined)
                hide = {};
            $.extend({
                op: true
            }, hide)
            //reseta informacoes 
            if (hide.op) {
                $('#ddOrdemProducao').text('Indefinido');
            }
            $('#ddTempoDecorrido').text('Indefinido');
            $('#ddQuantidadePecasProduzidas').text('Indefinido');
            $('#ddTempoSemFeed').text('Indefinido');
            $('#velocidadeParaAtingirMeta').text('Indefinido');
            //reseta velocimetro
            var gauge = document.gauges.get('canvVelocimetroPerformace');
            gauge.update({ highlights: [{ from: 0, to: 120, color: '#DDD' }], value: 0 });
            //reseta informacoes de setup
            $('#divProgressSetupAzul').css('width', '0%');
            $('#divProgressSetupVerde').css('width', '0%');
            $('#divProgressSetupAmarelo').css('width', '0%');
            $('#divProgressSetupVermelho').css('width', '0%');
            $('#spanSetupMetaTotalValue').text('');
            $('#divSetupMetaTotalPorcent').css('width', '0%');
            $('#divProgressAjusteMetaValue').css('width', '0%');
            $('#divProgressSetupMetaValue').css('width', '0%');
            //reseta informacoes de setup ajuste
            $('#divProgressSetupAzul').css('width', '0%');
            $('#divProgressSetupVerde').css('width', '0%');
            $('#divProgressSetupAmarelo').css('width', '0%');
            $('#divProgressSetupVermelho').css('width', '0%');
            $('#spanTempoGastoSetup').text('');
            $('#divPositionIconeSetup').css('width', '0%');
        },
        openAnLoad: function () {
            $('#dvLoadMonito').show();
        },
        closeAnLoad: function () {
            $('#dvLoadMonito').hide();
        }
    }
    var publico = {
        iniciar: function (maqId) {
            monitorTempoReal.iniciar(maqId)
        },
        cancelar: function () {
            monitorTempoReal.cancelar();
            util.resetInfo();
        },
    }
    return publico;
})();