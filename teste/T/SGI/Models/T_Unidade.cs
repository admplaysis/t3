//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SGI.Models
{
    using System;
    using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
    
    public class T_Unidade
    {
        public T_Unidade()
        {
            this.T_Medicoes = new HashSet<T_Medicoes>();
            this.T_UNIUSER = new HashSet<T_UNIUSER>();
        }
    
        public int UNI_ID { get; set; }
        public string DESCRICAO { get; set; }
        public string UN { get; set; }
    
        public virtual ICollection<T_Medicoes> T_Medicoes { get; set; }
        public virtual ICollection<T_UNIUSER> T_UNIUSER { get; set; }
    }

    class T_Unidade_ResultConfiguration : EntityTypeConfiguration<T_Unidade>
    {
        public T_Unidade_ResultConfiguration()
        {
            // Configurando propriedades e chaves
            this.HasKey(c => c.UNI_ID);

            this.Property(c => c.UNI_ID)
                .HasColumnName("UNI_ID")
                .IsRequired();

            this.Property(c => c.DESCRICAO)
                .HasColumnName("DEESCRICAO")
                .HasMaxLength(100)
                .IsRequired();

            this.Property(c => c.UN)
              .HasColumnName("UN")
              .HasMaxLength(20)
              .IsRequired();

            // Configurando a Tabela
            this.ToTable("T_Unidade");
        }
    }
}
