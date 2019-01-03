using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Models
{
    public class Tr_Unidade
    {
        public Tr_Unidade()
        {
            this.Tr_Movimentos = new HashSet<Tr_Movimentos>();
        }

        public int UNI_ID { get; set; }
        public string UNI_DESCRICAO { get; set; }
        public virtual IEnumerable<Tr_Movimentos> Tr_Movimentos { get; set; }
    }
    class Tr_Unidade_ResultConfiguration : EntityTypeConfiguration<Tr_Unidade>
    {
        public Tr_Unidade_ResultConfiguration()
        {
            // Configurando propriedades e chaves
            this.HasKey(c => c.UNI_ID);

            this.Property(c => c.UNI_ID)
                .HasColumnName("UNI_ID")
                .IsRequired();

            this.Property(c => c.UNI_DESCRICAO)
                .HasColumnName("UNI_DESCRICAO")
                .HasMaxLength(100)
                .IsRequired();

            // Configurando a Tabela
            this.ToTable("Tr_Unidade");
        }
    }
}