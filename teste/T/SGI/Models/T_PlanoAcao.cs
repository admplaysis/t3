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
    
    public class T_PlanoAcao
    {
        public int PLA_ID { get; set; }
        public string PLA_DESCRICAO { get; set; }
        public Nullable<int> MET_ID { get; set; }
        public string PLA_STATUS { get; set; }
        public Nullable<System.DateTime> PLA_DATA { get; set; }
        public string PLA_METAPERIODO { get; set; }
        public string PLA_VLRPERIODO { get; set; }
        public string PLA_METACULADO { get; set; }
        public string PLA_VLRACUMULADO { get; set; }
        public string PLA_REFERENCIA { get; set; }
        public int USER_ID { get; set; }
    
        public virtual T_Metas T_Metas { get; set; }
        public virtual T_Usuario T_Usuario { get; set; }
    }

    class T_PlanoAcao_ResultConfiguration : EntityTypeConfiguration<T_PlanoAcao>
    {
        public T_PlanoAcao_ResultConfiguration()
        {
            // Configurando propriedades e chaves
            this.HasKey(c => c.PLA_ID);

            this.Property(c => c.PLA_ID)
                .HasColumnName("PLA_ID")
                .IsRequired();

            this.Property(c => c.PLA_DESCRICAO)
                .HasColumnName("PLA_DESCRICAO")
                .HasMaxLength(3000)
                .IsRequired();

            this.Property(c => c.MET_ID)
               .HasColumnName("MET_ID")
               .IsOptional();

            this.HasOptional(c => c.T_Metas)
                .WithMany(c => c.T_PlanoAcao)
                .HasForeignKey(c => c.MET_ID);

            this.Property(c => c.PLA_STATUS)
              .HasColumnName("PLA_STATUS")
              .HasMaxLength(1)
              .IsOptional();

            this.Property(c => c.PLA_DATA)
              .HasColumnName("PLA_DATA")
              .IsOptional();

            this.Property(c => c.PLA_METAPERIODO)
              .HasColumnName("PLA_METAPERIODO")
              .HasMaxLength(50)
              .IsOptional();

            this.Property(c => c.PLA_VLRPERIODO)
              .HasColumnName("PLA_VLRPERIODO")
              .HasMaxLength(50)
              .IsOptional();

            this.Property(c => c.PLA_METACULADO)
              .HasColumnName("PLA_METACULADO")
              .HasMaxLength(50)
              .IsOptional();

            this.Property(c => c.PLA_VLRACUMULADO)
              .HasColumnName("PLA_VLRACUMULADO")
              .HasMaxLength(50)
              .IsOptional();

            this.Property(c => c.PLA_REFERENCIA)
              .HasColumnName("PLA_REFERENCIA")
              .HasMaxLength(50)
              .IsOptional();

            this.Property(c => c.USER_ID)
              .HasColumnName("USER_ID")
              .IsRequired();

            this.HasRequired(c => c.T_Usuario)
                .WithMany(c => c.T_PlanoAcao)
                .HasForeignKey(c => c.USER_ID);
                    
            // Configurando a Tabela
            this.ToTable("T_PlanoAcao");
        }
    }
}