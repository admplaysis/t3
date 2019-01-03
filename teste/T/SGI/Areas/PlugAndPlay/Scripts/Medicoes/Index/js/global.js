var $UrlLocal = '/PlugAndPlay/Medicoes/';
$(document).ready(function () {
    //documento carregado
    FilaProducao.documentRead();
    App.documentReady();
    ConectEsp.documentRead();
    GaugePerformaceProducao.criar();
    _tabelaFeedback.documentRead();
    Global.atualizarPaginaAjax(ServerValues.maquinaId)
    //pega parametros na url para restaurar o estado da pagina no retorno
    //var maquina = (new URL(window.location.href)).searchParams.get('maqId');
    //var top = (new URL(window.location.href)).searchParams.get('p');
    //if (maquina != undefined) {
    //    $('#sltMaquinaFeed').val(maquina);
    //    Global.atualizarPaginaAjax(maquina);
    //}
});

var Global = new function () {
    this.atualizarPaginaAjax = function (maquina) {
        FilaProducao.obterFila(maquina);
        Monitoramento.iniciar(maquina);
        Conexao.obterLinhaTempo(maquina);
    }
}