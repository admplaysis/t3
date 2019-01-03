using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Models
{
    public class Tr_Visoes
    {
        public int VIS_ID { get; set; }

        [Required(ErrorMessage = "Selecione a conta")]
        public int VIS_PLANID { get; set; }
        public virtual Tr_PlanoContas Tr_PlanoContas { get; set; }
        public string VIS_FORMULA { get; set; }
        
        [Required]
        public int CAB_ID { get; set; }
        public virtual Tr_CabViscao TR_CABVISAO { get; set; }
    }

    class Tr_Visoes_ResultConfiguration : EntityTypeConfiguration<Tr_Visoes>
    {
        public Tr_Visoes_ResultConfiguration()
        {
            // Configurando propriedades e chaves
            this.HasKey(c => c.VIS_ID);

            this.Property(c => c.VIS_ID)
                .HasColumnName("VIS_ID")
                .IsRequired();

            this.Property(c => c.VIS_FORMULA)
                .HasColumnName("VIS_FORMULA")
                .HasMaxLength(1000)
                .IsOptional();

            this.Property(c => c.VIS_PLANID)
                .HasColumnName("VIS_PLANID")
                .IsRequired();

            this.HasRequired(c => c.Tr_PlanoContas)
                .WithMany(t => t.Tr_Visoes)
                .HasForeignKey(c => c.VIS_PLANID);

            this.Property(c => c.CAB_ID)
                .HasColumnName("CAB_ID")
                .IsRequired();

            this.HasRequired(c => c.TR_CABVISAO)
                .WithMany(t => t.Tr_Visoes)
                .HasForeignKey(c => c.CAB_ID);

            // Configurando a Tabela
            this.ToTable("Tr_Visoes");
        }
    }
}