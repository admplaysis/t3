var ValidacaoFormulario = (function () {
    var publico = {
        inicio: function () {
            $('#formPedido').validate({
                rules: {
                    txtPedido: {
                        required: true
                    }
                }
            });
        },
        configurar(form) {
            var teste = $('#formNovoItem');
            form.validate({
                rules: {
                    OrdemProducaoId: {
                        required: true
                    },
                    'Roteiro.MaquinaId': {
                        required: true
                    },
                    'Roteiro.ProdutoId': {
                        required: true
                    },
                    DataInicioPrevista: {
                        required: true
                    },
                    DataInicioPrevista: {
                        required: true
                    },
                    DataFimPrevista: {
                        required: true
                    },
                    DataFimMaxima: {
                        required: true
                    },
                    'Roteiro.SequenciaTransformacao': {
                        required: true,
                        digits: true,
                    },
                    SequencaRepeticao: {
                        required: true,
                        digits: true,
                    },
                    QuantidadePrevista: {
                        required: true,
                        digits: true,
                    },
                    'Roteiro.PecasPorPulso': {
                        required: true,
                        digits: true,
                    }
                }
            });
        },
        validar: function (form) {
            return $('#' + form).valid();
        }
    }
    return publico;
})();