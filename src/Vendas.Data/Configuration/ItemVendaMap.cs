using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vendas.Core.Entidades;

namespace Vendas.Data.Configuration;

internal sealed class ItemVendaMap : IEntityTypeConfiguration<ItemVenda>
{
    public void Configure(EntityTypeBuilder<ItemVenda> builder)
    {
        builder.ToTable("items_venda");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();
        
        builder.Property<Guid>("VendaId")
            .IsRequired();
        
        builder.Property(x => x.Quantidade)
            .HasColumnType("numeric(14,2)");
        
        builder.Property(x => x.ValorUnitario)
            .HasColumnType("numeric(14,2)");
        
        builder.Property(x => x.ValorTotalSemDesconto)
            .HasColumnType("numeric(14,2)");
        
        builder.Property(x => x.ValorTotal)
            .HasColumnType("numeric(14,2)");
        
        builder.Property(x => x.Desconto)
            .HasColumnType("numeric(14,2)");

        builder.ComplexProperty(x => x.Produto, b =>
        {
            b.Property(x => x.Codigo)
                .HasColumnType("varchar");
            
            b.Property(x => x.Ean)
                .HasColumnType("varchar");
            
            b.Property(x => x.Nome)
                .HasColumnType("varchar");
            
            b.Property(x => x.Descricao)
                .HasColumnType("varchar");
            
            b.Property(x => x.IdExterno)
                .ValueGeneratedNever();
        });
    }
}
