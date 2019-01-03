Graficos = new function () {
    //<construtor> 

    //<metodos publicos>
    this.eventos = function () {
        $(document).on('change', '[data-grupo="sltDimensaoGrafico"]', changeSltDimensao);
        $(document).on('change', '[data-grupo="sltSubDimensaoGrafico"]', changesGrafico);
        $(document).on('change', '[data-grupo="sltPeriodoGrafico"]', changesGrafico);
        $(document).on('change', '.datasPeriodo', changesGrafico);
        $(document).on('click', '.aIconTipoGraficoPie', clickAIconTipoGraficoPie);
        $(document).on('click', '.aIconTipoGraficoLine', clickAIconTipoGraficoLine);
        $(document).on('click', '.aIconTipoGraficoBar', clickAIconTipoGraficoBar);
        $(document).on('click', '.aIExcel', clickaIExcel);
    }
    //<eventos>
    function clickaIExcel(e) {
        var indId = $(this).attr('data-ind-id');
        var dimensao = $('#sltDimensaoGrafico_' + indId).val();
        document.getElementById('sltSubDimensaoGrafico_' + indId);
        var e = document.getElementById('sltSubDimensaoGrafico_' + indId);
        var subDimensao = e.value;
        var periodo = $('#sltPeriodoGrafico_' + indId).val();
        var tipo = $('#sltTipoGrafico_' + indId).val();
        var dataIni = $('#dataIniGrafico_' + indId).val();
        var dataFim = $('#dataFimGrafico_' + indId).val();
        document.location.href = '../Consultas/ExecutaConsultaGrafico?indId=' + indId + '&dimensao=' + dimensao + '&periodo=' + periodo + '&subDim='+ subDimensao+'&dataIni=' + dataIni + '&dataFim=' + dataFim+'';
    }

    function clickAIconTipoGraficoPie(e) {
        e.preventDefault();
        var indId = $(this).attr('data-ind-id');
        $('#sltTipoGrafico_' + indId).val('3');
        atualizarGraficos('3', indId)
    }


    function clickAIconTipoGraficoLine(e) {
        e.preventDefault();
        var indId = $(this).attr('data-ind-id');
        $('#sltTipoGrafico_' + indId).val('2');
        atualizarGraficos('2', indId)
    }
    function clickAIconTipoGraficoBar(e) {
        e.preventDefault();
        var indId = $(this).attr('data-ind-id');
        $('#sltTipoGrafico_' + indId).val('1');
        atualizarGraficos('1', indId)
    }
    function atualizarGraficos(tipo, indId) {
        var dimensao = $('#sltDimensaoGrafico_' + indId).val();
        var subDimensao = $('#sltSubDimensaoGrafico_' + indId).val();
        var dataIni = $('#dataIniGrafico_' + indId).val();
        var dataFim = $('#dataFimGrafico_' + indId).val();
        if(tipo == 3){
            ObtemDados(indId, $('#s2id_sltAno').select2('val'), dimensao, '', tipo, dataIni, dataFim);
        }
        else {
            ObtemDados(indId, $('#s2id_sltAno').select2('val'), dimensao, $('#sltPeriodoGrafico_' + indId).val(), tipo, dataIni, dataFim);
        }
    }
    function changeSltTipoGrafico() {
        var tipo = $(this).val();
        var indId = $(this).attr('data-ind-id')
        var dimensao = $('#sltDimensaoGrafico_' + indId).val();
        var subDimensao = $('#sltSubDimensaoGrafico_' + indId).val();
        var dataIni = $('#dataIniGrafico_' + indId).val();
        var dataFim = $('#dataFimGrafico_' + indId).val();
        if (tipo == 3) {
            ObtemDados(indId, $('#s2id_sltAno').select2('val'), dimensao, '', tipo, dataIni, dataFim);
        }
        else {
            ObtemDados(indId, $('#s2id_sltAno').select2('val'), dimensao, $('#sltPeriodoGrafico_' + indId).val(), tipo, dataIni, dataFim);
        }
        
    }//changeSltDimensao
    function changeSltDimensao() {//select dimensao
        debugger
        var indId = $(this).attr('data-ind-id');
        var dimensao = $('#sltDimensaoGrafico_' + indId).val();
        var periodo = $('#sltPeriodoGrafico_' + indId).val();
        var tipo = $('#sltTipoGrafico_' + indId).val();
        var dataIni = $('#dataIniGrafico_' + indId).val();
        var dataFim = $('#dataFimGrafico_' + indId).val();
        //ObtemDados(indId, $('#s2id_sltAno').select2('val'), dimensao, $('#sltPeriodoGrafico_' + indId).val());
        if (tipo == 3) {
            ObtemDados(indId, $('#s2id_sltAno').select2('val'), dimensao, '', tipo, dataIni, dataFim);
        }
        else {
            ObtemDados(indId, $('#s2id_sltAno').select2('val'), dimensao, $('#sltPeriodoGrafico_' + indId).val(), tipo, dataIni, dataFim);
        }
     }


    function changeSltSubDimensao() {//select dimensao
        debugger
        var indId = $(this).attr('data-ind-id');
        var dimensao = $('#sltDimensaoGrafico_' + indId).val();
        document.getElementById('sltSubDimensaoGrafico_' + indId);
        var e = document.getElementById('sltSubDimensaoGrafico_' + indId);
        var subDimensao = e.value;
        var periodo = $('#sltPeriodoGrafico_' + indId).val();
        var tipo = $('#sltTipoGrafico_' + indId).val();
        var dataIni = $('#dataIniGrafico_' + indId).val();
        var dataFim = $('#dataFimGrafico_' + indId).val();
        //ObtemDados(indId, $('#s2id_sltAno').select2('val'), dimensao, $('#sltPeriodoGrafico_' + indId).val());
        if (tipo == 3) {
            ObtemDados(indId, $('#s2id_sltAno').select2('val'), dimensao, '', tipo, dataIni, dataFim,subDimensao);
        }
        else {
            ObtemDados(indId, $('#s2id_sltAno').select2('val'), dimensao, $('#sltPeriodoGrafico_' + indId).val(), tipo, dataIni, dataFim, subDimensao);
        }
    }



    function changeDatas() {//select dimensao
        debugger
        var indId = $(this).attr('data-ind-id');
        var dimensao = $('#sltDimensaoGrafico_' + indId).val();
        document.getElementById('sltSubDimensaoGrafico_' + indId);
        var e = document.getElementById('sltSubDimensaoGrafico_' + indId);
        var subDimensao = e.value;
        var periodo = $('#sltPeriodoGrafico_' + indId).val();
        var tipo = $('#sltTipoGrafico_' + indId).val();
        var dataIni = $('#dataIniGrafico_' + indId).val();
        var dataFim = $('#dataFimGrafico_' + indId).val();
        if (tipo == 3) {
            ObtemDados(indId, $('#s2id_sltAno').select2('val'), dimensao, $('#sltPeriodoGrafico_' + indId).val(), tipo, dataIni, dataFim, subDimensao);
        }
        else {
            ObtemDados(indId, $('#s2id_sltAno').select2('val'), dimensao, $('#sltPeriodoGrafico_' + indId).val(), tipo, dataIni, dataFim, subDimensao);
        }
    }


    function changeStlPeriodo() {
        debugger
        var indId = $(this).attr('data-ind-id');
        var dimensao = $('#sltDimensaoGrafico_' + indId).val();
        document.getElementById('sltSubDimensaoGrafico_' + indId);
        var e = document.getElementById('sltSubDimensaoGrafico_' + indId);
        var subDimensao = e.value;
        var periodo = $('#sltPeriodoGrafico_' + indId).val();
        var tipo = $('#sltTipoGrafico_' + indId).val();
        var dataIni = $('#dataIniGrafico_' + indId).val();
        var dataFim = $('#dataFimGrafico_' + indId).val();
        if (tipo == 3) {
            ObtemDados(indId, $('#s2id_sltAno').select2('val'), dimensao, '', tipo, dataIni, dataFim, subDimensao);
        }
        else {
            ObtemDados(indId, $('#s2id_sltAno').select2('val'), dimensao, $('#sltPeriodoGrafico_' + indId).val(), tipo, dataIni, dataFim, subDimensao);
        }
    }


    function changesGrafico() {
        debugger
        var indId = $(this).attr('data-ind-id');
        var dimensao = $('#sltDimensaoGrafico_' + indId).val();
        document.getElementById('sltSubDimensaoGrafico_' + indId);
        var e = document.getElementById('sltSubDimensaoGrafico_' + indId);
        var subDimensao = e.value;
        var periodo = $('#sltPeriodoGrafico_' + indId).val();
        var tipo = $('#sltTipoGrafico_' + indId).val();
        var dataIni = $('#dataIniGrafico_' + indId).val();
        var dataFim = $('#dataFimGrafico_' + indId).val();
        if (tipo == 3) {
            ObtemDados(indId, $('#s2id_sltAno').select2('val'), dimensao, '', tipo, dataIni, dataFim, subDimensao);
        }
        else {
            ObtemDados(indId, $('#s2id_sltAno').select2('val'), dimensao, $('#sltPeriodoGrafico_' + indId).val(), tipo, dataIni, dataFim, subDimensao);
        }
    }

}