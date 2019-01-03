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
    
    public class T_Negocio
    {
        public T_Negocio()
        {
            this.T_Indicadores = new HashSet<T_Indicadores>();
        }
    
        public int NEG_ID { get; set; }
        public string NEG_DESCRICAO { get; set; }
    
        public virtual ICollection<T_Indicadores> T_Indicadores { get; set; }
    }

    class T_Negocio_ResultConfiguration : EntityTypeConfiguration<T_Negocio>
    {
        public T_Negocio_ResultConfiguration()
        {
            // Configurando propriedades e chaves
            this.HasKey(c => c.NEG_ID);

            this.Property(c => c.NEG_ID)
                .HasColumnName("NEG_ID")
                .IsRequired();

            this.Property(c => c.NEG_DESCRICAO)
                .HasColumnName("NEG_DESCRICAO")
                .HasMaxLength(80)
                .IsRequired();

            // Configurando a Tabela
            this.ToTable("T_Negocio");
        }
    }
}
