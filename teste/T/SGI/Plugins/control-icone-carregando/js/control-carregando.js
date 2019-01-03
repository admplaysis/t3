var Carregando = new function () {
    //(1) executa no construtor
    $(document).ready(function () {
        $('body').append([
            $('<div>', { class: 'icone-loading', id: 'divIconeCarregando' }).html([
                $('<div>', { class: 'div-central' }).html([
                    $('<div>', { class: 'icone' }).html([
                        $('<i>', { class: 'fa fa-cog fa-spin fa-3x fa-fw' })
                    ]),
                    $('<div>', { class: 'texto' }).html([
                        $('<p>').text('Carregando...')
                    ])
                ])
            ])
        ]);
    });
    //(2) tela inteira ===>
    this.abrir = function (texto = 'Carregando') {
        texto += '...';
        $('#divIconeCarregando').css('display', 'block').find('p').text(texto)
    }
    this.fechar = function () {
        $('#divIconeCarregando').css('display', 'none')
    }
    //(4) parte da tela ===>
    this.abrirParcial = function (id) {
        var divLoad = $('#' + id + '_infoCarregando');
        if (divLoad.length > 0) {
            divLoad.css('display', 'flex');
        }
        else {
            var secao = $('#' + id);
            var classe = secao.width() > 400 ? 'info-carregando-pc-acima' : 'info-carregando-parcial-meio';
            divLoad = gerarHtml().attr('id', id + '_infoCarregando').addClass(classe);
            secao.prepend(divLoad);
            divLoad.css('display', 'flex');
        }
        var div = gerarHtml();
    }
    this.fecharParcial = function (id) {
        $('#' + id + '_infoCarregando').css('display', 'none');
    }
    //(5) uteis ===>
    function gerarHtml() {
        return div = $('<div>', { class: 'info-carregando-pc' }).html([
            $('<div>', { class: 'info' }).html([
                $('<i>', { class: 'fa fa-circle-o-notch fa-spin' }),
                $('<span>').text('Carregando...')
            ])
        ]);
    }
};