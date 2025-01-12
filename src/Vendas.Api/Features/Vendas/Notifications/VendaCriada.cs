using MediatR;
using Serilog;
using Vendas.Api.Dtos;

namespace Vendas.Api.Features.Vendas.Notifications;

public sealed record VendaCriadaMessage(VendaDto Venda) : INotification;

public sealed class VendaCriadaMessageHandler : INotificationHandler<VendaCriadaMessage>
{
    public Task Handle(VendaCriadaMessage notification, CancellationToken cancellationToken)
    {
        Log.Information("Venda criada: {@Venda}", notification.Venda);
        return Task.CompletedTask;
    }
}
