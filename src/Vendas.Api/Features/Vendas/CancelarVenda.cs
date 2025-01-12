using MediatR;
using Vendas.Api.Config;
using Vendas.Api.Dtos;
using Vendas.Api.Features.Vendas.Notifications;
using Vendas.Core.Entidades;
using Vendas.Core.Results;
using Vendas.Data;

namespace Vendas.Api.Features.Vendas;

public sealed record CancelarVendaRequest(Guid Id) : IRequest<Result<VendaDto, ValidacaoResult>>;

public sealed class CancelarVendaRequestHandler : IRequestHandler<CancelarVendaRequest, Result<VendaDto, ValidacaoResult>>
{
    private readonly DataContext _dataContext;
    private readonly IPublisher _publisher;

    public CancelarVendaRequestHandler(DataContext dataContext, IPublisher publisher)
    {
        _dataContext = dataContext;
        _publisher = publisher;
    }

    public async Task<Result<VendaDto, ValidacaoResult>> Handle(CancelarVendaRequest request, CancellationToken cancellationToken)
    {
        var venda = await _dataContext.ObterVendaAsync(request.Id, cancellationToken);

        if (venda is null)
        {
            return ValidacaoResult.Build(VendaMensagens.VendaNaoExiste);
        }

        venda.Cancelar();

        await _dataContext.SaveChangesAsync(cancellationToken);
        
        VendaDto vendaDto = venda.ToVendaDto();
        
        await _publisher.Publish(new VendaCanceladaMessage(vendaDto), cancellationToken);

        return vendaDto;
    }
}
