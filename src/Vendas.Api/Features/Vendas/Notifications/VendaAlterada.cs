using MediatR;
using Serilog;
using Vendas.Api.Dtos;

namespace Vendas.Api.Features.Vendas.Notifications;

public sealed record VendaAlteradaMessage(VendaDto Venda) : INotification;

public sealed class VendaAlteradaMessageHandler : INotificationHandler<VendaAlteradaMessage>
{
    public Task Handle(VendaAlteradaMessage notification, CancellationToken cancellationToken)
    {
        Log.Information("Venda alterada: {@Venda}", notification.Venda);
        return Task.CompletedTask;
    }
}
