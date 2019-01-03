using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class ViewClpMedicoesMap : EntityTypeConfiguration<ViewClpMedicoes>
    {
        public ViewClpMedicoesMap()
        {
            ToTable("V_CLP_MEDICOES");
            Property(x => x.MaquinaId).HasColumnName("MAQUINA_ID");
            Property(x => x.DataIni).HasColumnName("DATA_INI");
            Property(x => x.DataFim).HasColumnName("DATA_FIM");
            Property(x => x.Quantidade).HasColumnName("QTD");
            Property(x => x.Grupo).HasColumnName("GRUPO");
            Property(x => x.TurnoId).HasColumnName("URN_ID");
            Property(x => x.TurmaId).HasColumnName("URM_ID");
            Property(x => x.FeedBackId).HasColumnName("FEE_ID");
            Property(x => x.FeedbackObs).HasColumnName("FEE_OBSERVACOES");
            Property(x => x.OcoId).HasColumnName("OCO_ID");
            Property(x => x.FeedBackIdMov).HasColumnName("FEE_ID_MOV");
            HasKey(x => new { x.MaquinaId, x.Grupo});
        }
    }
}