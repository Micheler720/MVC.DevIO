using DevIO.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevIO.Data.Mappings
{
    public class EnderecoMapping : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Logradouro)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Bairro)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Cep)
                .IsRequired()
                .HasMaxLength(8);

            builder.Property(e => e.Cidade)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Complemento)
                .IsRequired()
                .HasMaxLength(100);

            builder.ToTable("Enderecos");
        }
    }
}
