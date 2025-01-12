using MediatR;
using Serilog;
using Vendas.Api.Dtos;

namespace Vendas.Api.Features.Vendas.Notifications;

public sealed record VendaCanceladaMessage(VendaDto Venda) : INotification;

public sealed class VendaCanceladaMessageHandler : INotificationHandler<VendaCanceladaMessage>
{
    public Task Handle(VendaCanceladaMessage notification, CancellationToken cancellationToken)
    {
        Log.Information("Venda cancelada: {@Venda}", notification.Venda);
        return Task.CompletedTask;
    }
}
