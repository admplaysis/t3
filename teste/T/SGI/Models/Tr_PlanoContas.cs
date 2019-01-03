using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Models
{
    public class Tr_PlanoContas
    {
        public Tr_PlanoContas()
        {
            this.Tr_Movimentos = new HashSet<Tr_Movimentos>();
            this.Tr_Visoes = new HashSet<Tr_Visoes>();
        }

        public int PLA_ID { get; set; }

        [Required(ErrorMessage = "Digite o código")]
        public string PLA_CODIGO { get; set; }

        [Required(ErrorMessage = "Digite a descrição")]
        public string PLA_DESCRICAO { get; set; }

        [Required(ErrorMessage = "Selecione o tipo da conta")]
        public int PLA_TIPO { get; set; }
        public string PLA_NATUREZA { get; set; }
        public virtual ICollection<Tr_Movimentos> Tr_Movimentos { get; set; }
        public virtual ICollection<Tr_Visoes> Tr_Visoes { get; set; }
    }

    class Tr_PlanoContas_ResultConfiguration : EntityTypeConfiguration<Tr_PlanoContas>
    {
        public Tr_PlanoContas_ResultConfiguration()
        {
            // Configurando propriedades e chaves
            this.HasKey(c => c.PLA_ID);

            this.Property(c => c.PLA_ID)
                .HasColumnName("PLA_ID")
                .IsRequired();

            this.Property(c => c.PLA_CODIGO)
                .HasColumnName("PLA_CODIGO")
                .HasMaxLength(30)
                .IsRequired();

            this.Property(c => c.PLA_DESCRICAO)
               .HasColumnName("PLA_DESCRICAO")
               .HasMaxLength(150)
               .IsRequired();

            this.Property(c => c.PLA_TIPO)
                .HasColumnName("PLA_TIPO")
                .IsRequired();
            this.Property(c => c.PLA_NATUREZA)
               .HasColumnName("PLA_NATUREZA").HasColumnType("nchar").HasMaxLength(1)
               .IsOptional();

            // Configurando a Tabela
            this.ToTable("Tr_PlanoContas");
        }
    }
}