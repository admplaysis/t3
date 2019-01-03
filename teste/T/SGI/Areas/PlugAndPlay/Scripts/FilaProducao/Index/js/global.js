$(document).ready(function () {
    Cadastro.inicio();
    ValidacaoFormulario.inicio();
    ModalAlterarFila.inicio();
    TabelaFilaProducao.iniciar();
    TabelaFilaProducao.carregarTabela();
});
//url padrao para as solicitacoes ajax
var UrlBase = '/PlugAndPlay/FilaProducao/';