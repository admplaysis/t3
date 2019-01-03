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

    public class T_Indicadores_Departamentos
    {
        public int DEP_ID { get; set; }
        public int IND_ID { get; set; }
        public int INDDEP_ID { get; set; }

        public virtual T_Departamentos T_Departamentos { get; set; }
        public virtual T_Indicadores T_Indicadores { get; set; }
    }

    class T_Indicadores_Departamentos_ResultConfiguration : EntityTypeConfiguration<T_Indicadores_Departamentos>
    {
        public T_Indicadores_Departamentos_ResultConfiguration()
        {
            // Configurando propriedades e chaves
            this.HasKey(c => c.INDDEP_ID);

            this.Property(c => c.INDDEP_ID)
                .HasColumnName("INDDEP_ID")
                .IsRequired();

            this.Property(c => c.DEP_ID)
                .HasColumnName("DEP_ID")
                .IsRequired();

            this.HasRequired(c => c.T_Departamentos)
                .WithMany(c => c.T_Indicadores_Departamentos)
                .HasForeignKey(x => x.DEP_ID);

            this.Property(c => c.IND_ID)
                .HasColumnName("IND_ID")
                .IsRequired();

            this.HasRequired(c => c.T_Indicadores)
                .WithMany(c => c.T_Indicadores_Departamentos)
                .HasForeignKey(x => x.IND_ID);

            // Configurando a Tabela
            this.ToTable("T_Indicadores_Departamentos");
        }
    }
}