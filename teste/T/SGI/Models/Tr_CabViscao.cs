using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Models
{
    public class Tr_CabViscao
    {
        public Tr_CabViscao()
        {
            this.Tr_Visoes = new HashSet<Tr_Visoes>();
        }

        public int CAB_ID { get; set; }

        [Required(ErrorMessage="Obrigatório informar a descrição")]
        public string CAB_DESC { get; set; }

        public int CAB_STATUS { get; set; }
        public virtual ICollection<Tr_Visoes> Tr_Visoes { get; set; }
        public int ID_USUARIO { get; set; }
        public virtual T_Usuario Usuario { get; set; }
    }

    class TR_CABVISAO_ResultConfiguration : EntityTypeConfiguration<Tr_CabViscao>
    {
        public TR_CABVISAO_ResultConfiguration()
        {
            // Configurando propriedades e chaves
            this.HasKey(c => c.CAB_ID);

            this.Property(c => c.CAB_ID)
                .HasColumnName("CAB_ID")
                .IsRequired();

            this.Property(c => c.CAB_DESC)
                .HasColumnName("CAB_DESC")
                .HasMaxLength(100)
                .IsRequired();

            this.Property(c => c.CAB_STATUS)
                .HasColumnName("CAB_STATUS")
                .IsRequired();

            this.Property(c => c.ID_USUARIO)
                .HasColumnName("ID_USUARIO");

            this.HasRequired(c => c.Usuario)
                .WithMany(c => c.Tr_CabViscao)
                .HasForeignKey(x => x.ID_USUARIO);

            // Configurando a Tabela
            this.ToTable("TR_CABVISAO");
        }
    }
}