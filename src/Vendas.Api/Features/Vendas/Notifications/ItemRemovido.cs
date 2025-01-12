using MediatR;
using Serilog;
using Vendas.Api.Dtos;

namespace Vendas.Api.Features.Vendas.Notifications;

public sealed record ItemRemovidoMessage(VendaDto Venda) : INotification;

public sealed class ItemRemovidoMessageHandler : INotificationHandler<ItemRemovidoMessage>
{
    public Task Handle(ItemRemovidoMessage notification, CancellationToken cancellationToken)
    {
        Log.Information("Item removido: {@Venda}", notification.Venda);
        return Task.CompletedTask;
    }
}
