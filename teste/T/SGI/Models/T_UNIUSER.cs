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
    
    public class T_UNIUSER
    {
        public int USERGRU_ID { get; set; }
        public int UNI_ID { get; set; }
        public int USER_ID { get; set; }
    
        public virtual T_Unidade T_Unidade { get; set; }
        public virtual T_Usuario T_Usuario { get; set; }
    }

    class T_UNIUSER_ResultConfiguration : EntityTypeConfiguration<T_UNIUSER>
    {
        public T_UNIUSER_ResultConfiguration()
        {
            // Configurando propriedades e chaves
            this.HasKey(c => c.USERGRU_ID);

            this.Property(c => c.USERGRU_ID)
                .HasColumnName("USERGRU_ID")
                .IsRequired();

            this.Property(c => c.UNI_ID)
                .HasColumnName("UNI_ID")
                .IsRequired();

            this.HasRequired(c => c.T_Unidade)
                .WithMany(c => c.T_UNIUSER)
                .HasForeignKey(c => c.UNI_ID);

            this.Property(c => c.USER_ID)
                .HasColumnName("USER_ID")
                .IsRequired();

            this.HasRequired(c => c.T_Usuario)
                .WithMany(c => c.T_UNIUSER)
                .HasForeignKey(c => c.USER_ID);

            // Configurando a Tabela
            this.ToTable("T_UNIUSER");
        }
    }
}
