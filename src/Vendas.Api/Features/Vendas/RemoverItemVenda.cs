using MediatR;
using Vendas.Api.Config;
using Vendas.Api.Dtos;
using Vendas.Api.Features.Vendas.Notifications;
using Vendas.Core.Entidades;
using Vendas.Core.Results;
using Vendas.Data;

namespace Vendas.Api.Features.Vendas;

public sealed record RemoverItemVendaRequest(Guid Id, string IdItemOuCodProduto) : IRequest<Result<VendaDto?, ValidacaoResult>>;

public sealed class RemoverItemVendaRequestHandler : IRequestHandler<RemoverItemVendaRequest, Result<VendaDto?, ValidacaoResult>>
{
    private readonly DataContext _dataContext;
    private readonly IPublisher _publisher;

    public RemoverItemVendaRequestHandler(DataContext dataContext, IPublisher publisher)
    {
        _dataContext = dataContext;
        _publisher = publisher;
    }

    public async Task<Result<VendaDto?, ValidacaoResult>> Handle(RemoverItemVendaRequest request, CancellationToken cancellationToken)
    {
        var venda = await _dataContext.ObterVendaAsync(request.Id, cancellationToken);

        if (venda is null)
        {
            return ValidacaoResult.Build(VendaMensagens.VendaNaoExiste);
        }

        return await venda.RemoverItem(request.IdItemOuCodProduto)
            .Match<Task<Result<VendaDto?, ValidacaoResult>>>(
                async v => await RemoverOuAtualizarVenda(v, cancellationToken),
                async validacao => await ValueTask.FromResult(validacao)
            );
    }
    
    private async ValueTask<VendaDto?> RemoverOuAtualizarVenda(Venda venda, CancellationToken cancellationToken)
    {
        if (venda.Itens.Count == 0)
        {
            _dataContext.Vendas.Remove(venda);
            await _dataContext.SaveChangesAsync(cancellationToken);
            await _publisher.Publish(new VendaRemovidaMessage(venda.ToVendaDto()), cancellationToken);
            return null;
        }

        VendaDto vendaDto = venda.ToVendaDto();
        await _dataContext.SaveChangesAsync(cancellationToken);
        await _publisher.Publish(new ItemRemovidoMessage(vendaDto), cancellationToken);
        return vendaDto;
    }
}
