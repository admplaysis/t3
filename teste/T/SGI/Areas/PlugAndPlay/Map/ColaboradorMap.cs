using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class ColaboradorMap : EntityTypeConfiguration<Colaborador>
    {
        public ColaboradorMap()
        {
            ToTable("T_COLABORADOR");
            Property(x => x.Cpf).HasColumnName("COL_CPF").HasMaxLength(14);
            Property(x => x.Nome).HasColumnName("COL_NOME").HasMaxLength(100).IsRequired();
            Property(x => x.DataNascimento).HasColumnName("COL_NASCIMENTO").IsRequired();
            Property(x => x.Email).HasColumnName("COL_EMAIL").HasMaxLength(150).IsRequired();
            Property(x => x.Matricula).HasColumnName("COL_MATRICULA").HasMaxLength(10).IsRequired();
            Property(x => x.TurmaId).HasColumnName("TURM_id").HasMaxLength(10).IsRequired();

            HasKey(x => x.Cpf);
            HasRequired(x => x.Turma).WithMany(x => x.Colaboradores).HasForeignKey(x => x.TurmaId);
        }
    }
}