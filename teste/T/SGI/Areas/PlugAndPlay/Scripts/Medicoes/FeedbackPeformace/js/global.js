$(document).ready(function () {
    SalvarFeedback.init();
    ValidarCampos.init();
    GaugePerformaceProducao.criar();
   
    BarraCores = new BarraCoresDesempenhoMaquinas();
    BarraCores.ConfCoresVelocimentroPerformace(ServerValues.target);
    BarraCores.ConfCoresBarraSetup(ServerValues.target);
    BarraCores.ConfCoresBarraSetupAjuste(ServerValues.target);

    BarraCores.definirValorSetup(ServerValues.target.RealizadoTempoSetup, ServerValues.target.SetupMaxAmarelo);
    BarraCores.definirValorSetupAjuste(ServerValues.target.RealizadoTempoSetupAjuste, ServerValues.target.SetupAjusteMaxAmarelo);
    BarraCores.definirValorPerformace(ServerValues.target.RealizadoPerformace, ServerValues.target.ProximaMetaPerformace);

});
var Global = new function () {

}