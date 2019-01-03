
$(function () {
    //criar linha do tempo baseado em 24horas
    criaLinhaDoTempo(24);
var escalaAtual = $('.alterarLinhaDoTempo').val();


    $('#btnAlterar').click(function () {

        $('#btnAdd').attr('disabled', 'disabled');
    });
   
    $('.alterarLinhaDoTempo').change(function () {
        var select = $('.alterarLinhaDoTempo option:selected').text();
        //pega o valor do parametro para criar a linha do tempo
        select = select.substr(0, 2);
        //elimina a linha do tempo ja existente para criar outra linha do tempo
        resetaLinhaDoTempo(select);
        criaLinhaDoTempo(select);

    });

    $("#btnAdd").click(function () {
        var tempoEmMinutos = geraNumeroAleatorio(20, 40) ;
        //cada 2min equivale a 1 pixel
        var tamanho = Math.ceil(tempoEmMinutos / 2);
        var cor = geraCor();

        var select = $('.alterarLinhaDoTempo option:selected').text();
        select = select.substr(0, 2);

        var html = '<div class="ped-fila" id="maqId' + tempoEmMinutos + '" style="background-color: ' + cor + '; width:' + tamanho + 'px;  display:block" > ' + tempoEmMinutos + 'Min</div>';

        //insere o codigo html da varialvel, dentro da div '#sortableM1'
        $('#sortableM1').append(html);
        $('#sortableM2').append(html);

         

    });


    /*ESSA FUNCAO PEGA O ELEMENTO 'SORTABLEM1' E FAZ ELE FICAR DO TIPO SORTABLE(Jquery Ui)*/
    $("#sortableM1").sortable();
    $("#sortableM1").disableSelection();
    
    $("#sortableM2").sortable();
    $("#sortableM2").disableSelection();

    //funcao teste que apos soltar um objeto faz uma listagem no log com o id da div
    //$("#sortableM1").bind('mouseup after',function () {
    $("#sortableM1").mouseup(function () {
        /*  apos soltar o elemento que foi arrastado espera 1s para ordenar a fila
            (.sortable()) e mostra no console o codigo de todas as divs
        */
        var TEMPO_DE_ESPERA = 1000; //tempo em segundos

        window.setTimeout(function () {
         var filhos = $('#sortableM1').children().length;

                for (var i = 0; i < filhos; i++) {
           
                    var idElemento = $('#sortableM1').children().eq(i).attr("id");
            
                    console.log(idElemento);

                }

        }, TEMPO_DE_ESPERA);
       
       
    });

    function calculaEscala(select, tamanhoAtual) {
        var novoTamanho;
        /* .val() vai retornar o valor que esta selecionado no momento*/
        var novaEscala = $('.alterarLinhaDoTempo option:selected').val();

        if (escalaAtual == 1) {

            if (novaEscala == 2)
                novoTamanho = Math.ceil(tamanhoAtual / 2);
            else {
                if (novaEscala == 3)
                    novoTamanho = Math.ceil(tamanhoAtual / 3);
            }
        }

        if (escalaAtual == 2) {

            if (novaEscala == 3)
                novoTamanho = tamanhoAtual * (48 / 72);
            else {
                if (novaEscala == 1)
                    novoTamanho = tamanhoAtual * 2;
            }
        }

        if (escalaAtual == 3) {

            if (novaEscala == 2)
                novoTamanho = tamanhoAtual * (72 / 48);
            else {
                if (novaEscala == 1)
                    novoTamanho = tamanhoAtual * 3;
            }
        }
        return Math.ceil(novoTamanho);
    }

    function resetaLinhaDoTempo(select) {

        var elementoaSerRemovido = $('#contemLinhaDoTempo').children();
        elementoaSerRemovido.remove();

        $('#contemLinhaDoTempo').append('<ul style="list-style-type:none; width:1970px;" id="contemLinhaDoTempo"> </ul>');

        var filhos = $('#sortableM1').children().length;

        for (var i = 0; i < filhos; i++) {

            var tamanhoAtualDoFilho = $('#sortableM1').children().eq(i).width();
            var tamanhoAjustado = calculaEscala(select, tamanhoAtualDoFilho);
            $('#sortableM1').children().eq(i).width(tamanhoAjustado);

        }
        escalaAtual = $('.alterarLinhaDoTempo option:selected').val();


    }

    function criaLinhaDoTempo(parametro) {


        var hora = 0;
        var min = 30;

        //incremento de hora e incremento de minutos
        var incH = 1;
        var incM = 30;

        if (parametro == 24) {
            incH = 1;
            incM = 30;
        }
        else {
            if (parametro == 48) {
                incH = 1;
                incM = 60;
                min = 60;
            }
            else {
                hora = 1;
                min = 30;

                incH = 1;
                incM = 90;
            }
        }


        for (var i = 0; i < 48; i++ , min += incM) {
            var htmlHora = "<li class='liFilaDeProducao'>";
            if (min >= 60) {
                while (min - 60 >= 0) {
                    min -= 60;
                    hora += incH;
                }

            }
            if (hora >= 24) {
                hora = 0;
            }

            if (hora == 0) {
                htmlHora = htmlHora + "00";
            }
            else {
                htmlHora = htmlHora + hora.toString();
            }

            if (min == 0) {
                htmlHora = htmlHora + ":00";
            }
            else {
                htmlHora = htmlHora + ":" + min.toString();
            }
            htmlHora = htmlHora + '</li>';
            if (htmlHora.search('00:00') > 0)
                htmlHora = htmlHora.replace("<li class='liFilaDeProducao'>", "<li class='liFilaDeProducao' style='background-color: #FBA90A;'>");

            $('#contemLinhaDoTempo').append(htmlHora);



        }


    }

    /*Funcoes usadas apenas para testes*/
    function calculaTamanhoDiv(tempoEmMinutos) {

        var tamanho = tempoEmMinutos / 2;

        return tamanho;
    }

    function geraNumeroAleatorio(min, max) {
        return Math.ceil(Math.random() * (max - min) - min);
    }
    
    function geraCor() {

        var red = Math.ceil(Math.random() * (15 - 0) - 0);
        var green = Math.ceil(Math.random() * (15 - 0) - 0);
        var blue = Math.ceil(Math.random() * (15 - 0) - 0);

        if (red >= 10)
            red = 'A';

        if (green >= 10)
            green = 'A';

        if (blue >= 10)
            blue = 'A';



        var retorno = '#' + red + '' + green + '' + blue + '';

        return retorno;
    }
});