using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Models
{
    public class T_Auditoria
    {
        public int ID { get; set; }
        public DateTime DATA { get; set; }
        public int ID_USUARIO { get; set; }
        public virtual T_Usuario T_Usuario { get; set; }
        public string ROTINA { get; set; }
        public string HISTORICO { get; set; }
        public string CHAVE { get; set; }
    }

    class T_Auditoria_ResultConfiguration : EntityTypeConfiguration<T_Auditoria>
    {
        public T_Auditoria_ResultConfiguration()
        {
            // Configurando propriedades e chaves
            this.HasKey(c => c.ID);

            this.Property(c => c.DATA)
                .HasColumnName("DATA")
                .IsRequired();

            this.Property(c => c.ID_USUARIO)
                .HasColumnName("ID_USUARIO");

            this.HasRequired(e => e.T_Usuario)
                .WithMany(t => t.T_Auditoria)
                .HasForeignKey(c => c.ID_USUARIO);

            this.Property(c => c.ROTINA)
                .HasColumnName("ROTINA")
                .HasMaxLength(100)
                .IsRequired();

            this.Property(c => c.HISTORICO)
                .HasColumnName("HISTORICO")
                .HasMaxLength(3000)
                .IsOptional();

            this.Property(c => c.CHAVE)
                .HasColumnName("CHAVE")
                .HasMaxLength(100)
                .IsOptional();

            // Configurando a Tabela
            this.ToTable("T_Auditoria");
        }
    }
}