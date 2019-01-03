var AlertPage = new function () {
    //<property>
    var timeOut = null; 
    //<construtor>
    $(document).ready(function () {
        $('body').click(function () {
            if (timeOut != null)
                clearTimeout(timeOut);
            ocultar(false);
        });
    });
    this.erro = function (msg, timeHide = true) {
        this.mostrar("erro", msg, timeHide);
    }
    this.mostrar = function (tipo, msg, ocultarAutomatico = true) {
        if (timeOut != null) 
            clearTimeout(timeOut);
        var alert = gerarDivAlert(tipo, msg);
        $('body').append(alert); 
        alert.slideToggle();
        if (ocultarAutomatico) {
            timeOut = setTimeout(ocultar, 10000, true);
        }
    }
    this.ocultar = function () {

    }
    //<uteis>
    function gerarDivAlert(tipo, msg) {
        classTipoIcone = 'fa fa-check-circle';
        classTipoAlert = 'alert-success';
        if (tipo == 'erro') {
            classTipoIcone = 'fa fa-exclamation-circle';
            classTipoAlert = 'alert-danger';
        }
        else if (tipo == 'sucesso') {
            classTipoIcone = 'fa fa-check-circle';
            classTipoAlert = 'alert-success';
        }
        return $('<div>', { id: 'principalAlertPage', class: 'alert ' + classTipoAlert + ' alert-dismissible fade in' }).html([
            $('<div>', { class: 'lateral'}).html([
                $('<i>', { class: classTipoIcone })
            ]),
            $('<div>', { class: 'texto' }).text(msg),
            $('<div>', { class: 'lateral text-rigth' }).html([
                $('<button>', { type:'button', class:'close', 'data-dismiss':'alert'}).text('×')
            ]),
        ])
    }
    function ocultar(slide) {
        var alert = $('#principalAlertPage');
        if (slide) {
            alert.slideToggle(1000, function () {
                $(this).remove();
            });
        }
        else {
            alert.remove();
        }
    }
}