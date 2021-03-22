using DevIO.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevIO.Data.Mappings
{
    class FornecedorMapping : IEntityTypeConfiguration<Fornecedor>
    {
        public void Configure(EntityTypeBuilder<Fornecedor> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.Documento)
                .IsRequired()
                .HasColumnType("varchar");

            builder.Property(f => f.Ativo)
                .IsRequired();

            builder.HasOne(f => f.Endereco)
                .WithOne(e => e.Fornecedor);

            builder.HasMany(f => f.Produtos)
                .WithOne(p => p.Fornecedor)
                .HasForeignKey(p => p.FornecedorId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.ToTable("Fornecedores");
        }
    }
}
