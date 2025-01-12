using MediatR;
using Vendas.Api.Config;
using Vendas.Api.Features.Vendas.Notifications;
using Vendas.Core.Entidades;
using Vendas.Core.Results;
using Vendas.Data;

namespace Vendas.Api.Features.Vendas;

public sealed record RemoverVendaRequest(Guid Id) : IRequest<Result<Guid, ValidacaoResult>>;

public sealed class RemoverVendaRequestHandler : IRequestHandler<RemoverVendaRequest, Result<Guid, ValidacaoResult>>
{
    private readonly DataContext _dataContext;
    private readonly IPublisher _publisher;

    public RemoverVendaRequestHandler(DataContext dataContext, IPublisher publisher)
    {
        _dataContext = dataContext;
        _publisher = publisher;
    }

    public async Task<Result<Guid, ValidacaoResult>> Handle(RemoverVendaRequest request, CancellationToken cancellationToken)
    {
        var venda = await _dataContext.ObterVendaAsync(request.Id, cancellationToken);

        if (venda is null)
        {
            return ValidacaoResult.Build(VendaMensagens.VendaNaoExiste);
        }

        _dataContext.Vendas.Remove(venda);
        await _dataContext.SaveChangesAsync(cancellationToken);
        await _publisher.Publish(new VendaRemovidaMessage(venda.ToVendaDto()), cancellationToken);
        return venda.Id;
    }
}
