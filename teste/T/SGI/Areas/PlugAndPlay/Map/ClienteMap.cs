using SGI.Areas.PlugAndPlay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Map
{
    public class ClienteMap:EntityTypeConfiguration<Cliente>
    {
        public ClienteMap()
        {
            ToTable("T_CLIENTES");
            Property(x => x.Id).HasColumnName("CLI_ID").HasMaxLength(50);
            Property(x => x.Nome).HasColumnName("CLI_NOME").HasMaxLength(100).IsRequired();
            Property(x => x.Fone).HasColumnName("CLI_FONE").HasMaxLength(11).IsRequired();
            Property(x => x.Observacao).HasColumnName("CLI_OBS").HasMaxLength(500).IsOptional();
            Property(x => x.Endereco).HasColumnName("CLI_ENDERECO").HasMaxLength(100).IsRequired();
            Property(x => x.CpfCnpj).HasColumnName("CLI_CPF_CNPJ").HasMaxLength(11).IsRequired();
            Property(x => x.TempoParaEntrega).HasColumnName("CLI_TEMPO_PARA_ENTREGA").IsRequired();
            HasKey(x => x.Id);
        }
    }
}