using FluentValidation;
using MediatR;
using Vendas.Api.Config;
using Vendas.Api.Features.Vendas.CommonRequests;
using Vendas.Api.Features.Vendas.Notifications;
using Vendas.Core.Results;
using Vendas.Data;

namespace Vendas.Api.Features.Vendas;

public sealed record CriarVendaRequest(
    string Numero,
    Guid IdExterno,
    FilialRequest Filial,
    ClienteRequest Cliente,
    DateTimeOffset DataVenda,
    List<ItemVendaRequest> Itens
) : VendaRequest(Numero, IdExterno, Filial, Cliente, DataVenda, Itens),
    IRequest<Result<Guid, ValidacaoResult>>;

public sealed class CriarVendaRequestValidator : AbstractValidator<CriarVendaRequest>
{
    public CriarVendaRequestValidator() => this.RegisterRules();
}

public sealed class CriarVendaRequestHandler : IRequestHandler<CriarVendaRequest, Result<Guid, ValidacaoResult>>
{
    private readonly DataContext _dataContext;
    private readonly IPublisher _publisher;

    public CriarVendaRequestHandler(DataContext dataContext, IPublisher publisher)
    {
        _dataContext = dataContext;
        _publisher = publisher;
    }

    public async Task<Result<Guid, ValidacaoResult>> Handle(CriarVendaRequest request, CancellationToken cancellationToken)
    {
        var venda = request.ToVenda();

        return await venda.EhValida()
            .Match<Task<Result<Guid, ValidacaoResult>>>(
                async _ =>
                {
                    await _dataContext.Vendas.AddAsync(venda, cancellationToken);
                    await _dataContext.SaveChangesAsync(cancellationToken);
                    await _publisher.Publish(new VendaCriadaMessage(venda.ToVendaDto()), cancellationToken);
                    return venda.Id;
                },
                async invalido => await ValueTask.FromResult(invalido)
            );
    }
}
