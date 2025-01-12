using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions.Common;
using Vendas.Api.Features.Vendas;
using Vendas.Api.Features.Vendas.CommonRequests;

namespace Vendas.Tests.Utils;

public static class RequestsFakerFactory
{
    public static ClienteRequest BuildClienteRequest() =>
        new Faker<ClienteRequest>()
            .CustomInstantiator(f =>
                new ClienteRequest(
                    f.Company.CompanyName(),
                    f.Company.Cnpj(),
                    f.Random.Guid())
            );

    public static FilialRequest BuiFilialRequest() =>
        new Faker<FilialRequest>()
            .CustomInstantiator(f =>
                new FilialRequest(
                    f.Random.Hash(),
                    f.Company.CompanyName(),
                    f.Random.Guid()
                )
            );

    public static ProdutoRequest BuildProdutoRequest() =>
        new Faker<ProdutoRequest>()
            .CustomInstantiator(f =>
                new ProdutoRequest(
                    f.Random.Hash(),
                    f.Commerce.Ean13(),
                    f.Commerce.ProductName(),
                    f.Commerce.ProductDescription(),
                    f.Random.Guid()
                )
            );

    public static List<ItemVendaRequest> BuildItemVendaRequest(int total = 1) =>
        new Faker<ItemVendaRequest>()
            .CustomInstantiator(f =>
                {
                    var quantidade = f.Random.Number(1, 100);
                    var valorUnitario = f.Random.Decimal(1, 1000);
                    var desconto = f.Random.Decimal(0, quantidade * valorUnitario);

                    return new ItemVendaRequest(
                        quantidade,
                        valorUnitario,
                        desconto,
                        BuildProdutoRequest()
                    );
                }
            ).Generate(total);

    public static CriarVendaRequest BuildCriarVendaRequest(int totalItens) =>
        new Faker<CriarVendaRequest>()
            .CustomInstantiator(f => new CriarVendaRequest(
                    f.Random.Hash(),
                    f.Random.Guid(),
                    BuiFilialRequest(),
                    BuildClienteRequest(),
                    f.Date.Recent().ToDateTimeOffset(),
                    BuildItemVendaRequest(totalItens)
                )
            );
    
    public static EditarVendaRequest BuildEditarVendaRequest(int totalItens) =>
        new Faker<EditarVendaRequest>()
            .CustomInstantiator(f => new EditarVendaRequest(
                    f.Random.Hash(),
                    f.Random.Guid(),
                    BuiFilialRequest(),
                    BuildClienteRequest(),
                    f.Date.Recent().ToDateTimeOffset(),
                    BuildItemVendaRequest(totalItens)
                )
            );
}
