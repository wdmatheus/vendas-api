using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions.Common;
using Vendas.Core.Entidades;

namespace Vendas.Tests.Utils;

public static class EntidadesFakerFactory
{
    public static Faker Faker => new ();

    public static Venda BuildVenda() =>
        new(
            Faker.Random.Guid(),
            Faker.Random.Hash(10),
            BuildFilial(),
            BuildCliente(),
            Faker.Date.Recent().ToDateTimeOffset(),
            BuildItensVenda(3)
        );
    
    public static Venda BuildVenda(int totalItens) =>
        new(
            Faker.Random.Guid(),
            Faker.Random.Hash(10),
            BuildFilial(),
            BuildCliente(),
            Faker.Date.Recent().ToDateTimeOffset(),
            BuildItensVenda(totalItens)
        );

    public static Venda BuildVenda(List<ItemVenda> itemVendas) =>
        new(
            Faker.Random.Guid(),
            Faker.Random.Hash(10),
            BuildFilial(),
            BuildCliente(),
            Faker.Date.Recent().ToDateTimeOffset(),
            itemVendas
        );

    public static List<ItemVenda> BuildItensVenda(int total)
    {
        var items = new List<ItemVenda>();
        for (var i = 0; i < total; i++)
        {
            var quantidade = Faker.Random.Int(1, 10);
            var valorUnitario = Faker.Random.Decimal(1, 100);
            var desconto = Faker.Random.Decimal(0, valorUnitario * quantidade);
            items.Add(new ItemVenda(
                    quantidade,
                    valorUnitario,
                    desconto,
                    BuildProduto()
                )
            );
        }

        return items;
    }

    public static Produto BuildProduto() =>
        new Faker<Produto>()
            .RuleFor(x => x.Codigo, f => f.Random.Hash(10))
            .RuleFor(x => x.Ean, f => f.Commerce.Ean13())
            .RuleFor(x => x.Nome, f => f.Commerce.Product())
            .RuleFor(x => x.Descricao, f => f.Commerce.ProductDescription())
            .RuleFor(x => x.IdExterno, f => f.Random.Guid())
            .Generate();

    public static Cliente BuildCliente() =>
        new Faker<Cliente>()
            .RuleFor(x => x.Nome, f => f.Company.CompanyName())
            .RuleFor(x => x.Cnpj, f => f.Company.Cnpj())
            .RuleFor(x => x.Nome, f => f.Commerce.Product())
            .RuleFor(x => x.IdExterno, f => f.Random.Guid())
            .Generate();

    public static Filial BuildFilial() => new Faker<Filial>()
        .RuleFor(x => x.Nome, f => f.Company.CompanyName())
        .RuleFor(x => x.Numero, f => f.Random.Hash(10))
        .RuleFor(x => x.IdExterno, f => f.Random.Guid())
        .Generate();
}
