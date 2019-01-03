var DataTableCrudConfg = (function () {
    var publico = {
        inicio: function () {
            $('#tabListaDados').DataTable({
                responsive: true,
                    paging: false,
                    searching: false
            });
        }
    }
    return publico;
})();