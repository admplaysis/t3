using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Models
{
    public class Tr_Movimentos
    {
        public int MOV_ID { get; set; }
        public string MOV_DATA { get; set; }
        public decimal MOV_VALOR { get; set; }
        public int MOV_PLAID { get; set; }
        public virtual Tr_PlanoContas Tr_PlanoContas { get; set; }
        public int MOV_UNID { get; set; }
        public virtual Tr_Unidade Tr_Unidade { get; set; }
    }

    class Tr_Movimentos_ResultConfiguration : EntityTypeConfiguration<Tr_Movimentos>
    {
        public Tr_Movimentos_ResultConfiguration()
        {
            // Configurando propriedades e chaves
            this.HasKey(c => c.MOV_ID);

            this.Property(c => c.MOV_ID)
                .HasColumnName("MOV_ID")
                .IsRequired();

            this.Property(c => c.MOV_DATA)
                .HasColumnName("MOV_DATA")
                .HasMaxLength(8)
                .IsRequired();

            this.Property(c => c.MOV_VALOR)
                .HasColumnName("MOV_VALOR")
                .HasPrecision(18,2)
                .IsRequired();

            this.Property(c => c.MOV_PLAID)
                .HasColumnName("MOV_PLAID");

            this.HasRequired(c => c.Tr_PlanoContas)
                .WithMany(t => t.Tr_Movimentos)
                .HasForeignKey(c => c.MOV_PLAID);

            // Configurando a Tabela
            this.ToTable("Tr_Movimentos");
        }
    }
}