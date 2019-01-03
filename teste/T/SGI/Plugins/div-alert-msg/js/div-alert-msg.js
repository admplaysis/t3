var DivAlertMsg = (function () {
    //(1) - tipos de alertas ===
    // 1.1 - info
    function gerarAlertInfo(msg) {
        return $('<div>', { class: 'div-alert-msg' }).text(msg);
    }
    // 1.2 - erro
    function gerarAlertErro(id, msg, autoRemTemp = false) {
        $('#' + id).html($('<div>', { class: 'alert alert-danger' }).text(msg));
        if (autoRemTemp) {
            setTimeout(removerAlerta, 5000, id)
        }
    }
    // 1.3 - atencao
    function gerarAlertAtencao(id, msg, autoRemTemp = false) {
        $('#' + id).html($('<div>', { class: 'alert alert-warning' }).text(msg));
        if (autoRemTemp) {
            setTimeout(removerAlerta, 5000, id)
        }
    }
    //(2) - Utilidades ===
    function removerAlerta(id) {
        var div = $('#' + id);
        div.find('.alert').fadeToggle('slow', function () {
            $('#' + id).empty();
        })
    }
    // ============= retorna as funcoes publicas =========
    return {
        info: function (msg) {
           gerarAlertInfo(msg);
        },
        erro: function (id, msg, autoRemTemp = false) {
            gerarAlertErro(id, msg, autoRemTemp);
        },
        atencao: function (id, msg, autoRemTemp = false) {
            gerarAlertAtencao(id, msg, autoRemTemp)
        },
        remover: function (id) {
            removerAlerta(id)
        }
    };
})();