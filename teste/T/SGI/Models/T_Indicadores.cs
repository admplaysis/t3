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
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;

    public class T_Indicadores
    {
        public T_Indicadores()
        {
            this.T_Grupo_Indicador = new HashSet<T_Grupo_Indicador>();
            this.T_Indicadores_Departamentos = new HashSet<T_Indicadores_Departamentos>();
            this.T_Metas = new HashSet<T_Metas>();
            this.T_Favoritos = new HashSet<T_Favoritos>();
            Dimensoes = new HashSet<Dimensao>();
        }

        public int IND_ID { get; set; }
        public string IND_DESCRICAO { get; set; }
        public Nullable<int> NEG_ID { get; set; }
        public string DESC_CALCULO { get; set; }
        public Nullable<int> IND_TIPOCOMPARADOR { get; set; }
        public Nullable<int> IND_GRAFICO { get; set; }
        public Nullable<int> DIM_ID { get; set; }
        public string PER_ID { get; set; }
        public string IND_CONEXAO { get; set; }
        public Nullable<DateTime> IND_DTCRIACAO { get; set; }
        public string RESPOSAVELIND { get; set; }
        public string RESPOSAVELCARGA { get; set; }
        public string PROCEXTRACAO { get; set; }

        public virtual ICollection<T_Grupo_Indicador> T_Grupo_Indicador { get; set; }
        public virtual ICollection<T_Indicadores_Departamentos> T_Indicadores_Departamentos { get; set; }
        public virtual ICollection<T_Metas> T_Metas { get; set; }
        public virtual T_Negocio T_Negocio { get; set; }
        public virtual ICollection<T_Favoritos> T_Favoritos { get; set; }
        [NotMapped]
        public virtual ICollection<Dimensao> Dimensoes { get; set; }
    }

    class T_Indicadores_ResultConfiguration : EntityTypeConfiguration<T_Indicadores>
    {
        public T_Indicadores_ResultConfiguration()
        {
            // Configurando propriedades e chaves
            this.HasKey(c => c.IND_ID);

            this.Property(c => c.IND_ID)
                .HasColumnName("IND_ID")
                .IsRequired();

            this.Property(c => c.IND_DESCRICAO)
                .HasColumnName("IND_DESCRICAO")
                .HasMaxLength(100)
                .IsRequired();

            this.Property(c => c.NEG_ID)
                .HasColumnName("NEG_ID")
                .IsRequired();

            this.HasRequired(c => c.T_Negocio)
                .WithMany(c => c.T_Indicadores)
                .HasForeignKey(x => x.NEG_ID);

            
            this.Property(c => c.DESC_CALCULO)
                .HasColumnName("DESC_CALCULO")
                .HasMaxLength(3000)
                .IsOptional();

            this.Property(c => c.IND_TIPOCOMPARADOR)
                .HasColumnName("IND_TIPOCOMPARADOR")
                .IsRequired();

            this.Property(c => c.IND_GRAFICO)
                .HasColumnName("IND_GRAFICO")
                .IsOptional();

            this.Property(c => c.DIM_ID)
                .HasColumnName("DIM_ID")
                .IsOptional();

            this.Property(c => c.PER_ID)
                .HasColumnName("PER_ID")
                .HasMaxLength(3)
                .IsOptional();
            
            this.Property(c => c.IND_CONEXAO)
                .HasColumnName("IND_CONEXAO")
                .HasMaxLength(100)
                .IsOptional();

            this.Property(c => c.IND_DTCRIACAO)
                .HasColumnName("IND_DTCRIACAO")
                .IsOptional();

            this.Property(c => c.RESPOSAVELIND)
               .HasColumnName("RESPOSAVELIND")
               .HasMaxLength(100)
               .IsOptional();

            this.Property(c => c.RESPOSAVELCARGA)
               .HasColumnName("RESPOSAVELCARGA")
               .HasMaxLength(100)
               .IsOptional();

            this.Property(c => c.PROCEXTRACAO)
               .HasColumnName("PROCEXTRACAO")
               .HasMaxLength(100)
               .IsOptional();

            // Configurando a Tabela
            this.ToTable("T_Indicadores");
        }
    }
}