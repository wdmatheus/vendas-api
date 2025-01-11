using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vendas.Core.Entidades;

namespace Vendas.Data.Configuration;

internal sealed class VendaMap : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.ToTable("vendas");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();
        
        builder.Property(x => x.IdExterno)
            .ValueGeneratedNever();

        builder.Property(x => x.Numero)
            .HasColumnType("varchar");

        builder.ComplexProperty(x => x.Filial, b =>
        {
            b.Property(x => x.Nome)
                .HasColumnType("varchar");
            
            b.Property(x => x.Numero)
                .HasColumnType("varchar");
            
            b.Property(x => x.IdExterno)
                .ValueGeneratedNever();
        });
        
        builder.ComplexProperty(x => x.Cliente, b =>
        {
            b.Property(x => x.Nome)
                .HasColumnType("varchar");

            b.ComplexProperty(x => x.Cnpj, c =>
            {
                c.Property(x => x.Numero)
                    .HasColumnType("varchar(14)");
            });
            
            b.Property(x => x.IdExterno)
                .ValueGeneratedNever();
        });

        builder.Property(x => x.ValorTotalSemDesconto)
            .HasColumnType("numeric(14,2)");
        
        builder.Property(x => x.ValorTotal)
            .HasColumnType("numeric(14,2)");
        
        builder.Property(x => x.Desconto)
            .HasColumnType("numeric(14,2)");

        builder.HasMany(x => x.Itens)
            .WithOne()
            .HasForeignKey("VendaId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.CriadaEm);
    }
}
