using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Models
{
    public class T_Favoritos
    {
        public int IDFAVORITO { get; set; }
        public int ID_USUARIO { get; set; }
        public virtual T_Usuario T_Usuario { get; set; }
        public int ID_INDICADOR { get; set; }
        public virtual T_Indicadores T_Indicadores { get; set; }
    }

    class T_Favoritos_ResultConfiguration : EntityTypeConfiguration<T_Favoritos>
    {
        public T_Favoritos_ResultConfiguration()
        {
            // Configurando propriedades e chaves
            this.HasKey(c => c.IDFAVORITO);

            this.Property(c => c.ID_USUARIO)
                .HasColumnName("ID_USUARIO")
                .IsRequired();

            this.HasRequired(e => e.T_Usuario)
                .WithMany(t => t.T_Favoritos)
                .HasForeignKey(c => c.ID_USUARIO);

            this.Property(c => c.ID_INDICADOR)
                .HasColumnName("ID_INDICADOR")
                .IsRequired();

            this.HasRequired(e => e.T_Indicadores)
                .WithMany(t => t.T_Favoritos)
                .HasForeignKey(c => c.ID_INDICADOR);

            // Configurando a Tabela
            this.ToTable("T_Favoritos");
        }
    }
}