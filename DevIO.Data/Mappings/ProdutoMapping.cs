﻿using DevIO.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevIO.Data.Mappings
{
    class ProdutoMapping : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Imagem)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Valor)
                .IsRequired();

            builder.Property(p => p.Descricao)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(p => p.Ativo)
                .IsRequired();

            builder.ToTable("Produtos");
        }
    }
}
