using MediatR;
using Serilog;
using Vendas.Api.Dtos;

namespace Vendas.Api.Features.Vendas.Notifications;

public sealed record VendaRemovidaMessage(VendaDto Venda) : INotification;

public sealed class VendaRemovidaMessageHandler : INotificationHandler<VendaRemovidaMessage>
{
    public Task Handle(VendaRemovidaMessage notification, CancellationToken cancellationToken)
    {
        Log.Information("Venda removida: {@Venda}", notification.Venda);
        return Task.CompletedTask;
    }
}
