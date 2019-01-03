var _jqComponente = (function () {
    var publico = {
        select: function (listaItens, id = "", idPropName = "id", descPropName = "descricao") {
            var select = $('#' + id);
            if (select.length == 0)
                select = $('<select>', { id: id });
            else
                $('#' + id + ' option:not([value=""])').remove();

            listaItens.forEach(function (i) {
                select.append($('<option>', { value: i[idPropName] }).text(i[descPropName]));
            });
            select.prop('disabled', false);
            return select;
        }
    }
    return publico;
})();