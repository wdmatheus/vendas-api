using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using Vendas.Api.Config;
using Vendas.Api.Dtos;
using Vendas.Api.Features.Vendas.CommonRequests;
using Vendas.Api.Features.Vendas.Notifications;
using Vendas.Core.Entidades;
using Vendas.Core.Results;
using Vendas.Data;

namespace Vendas.Api.Features.Vendas;

public sealed record EditarVendaRequest(
    string Numero,
    Guid IdExterno,
    FilialRequest Filial,
    ClienteRequest Cliente,
    DateTimeOffset DataVenda,
    List<ItemVendaRequest> Itens
) : VendaRequest(Numero, IdExterno, Filial, Cliente, DataVenda, Itens),
    IRequest<Result<VendaDto, ValidacaoResult>>
{
    [JsonIgnore]
    public Guid Id { get; set; }
}

public sealed class EditarVendaRequestValidator : AbstractValidator<EditarVendaRequest>
{
    public EditarVendaRequestValidator() => this.RegisterRules();
}

public sealed class EditarVendaRequestHandler : IRequestHandler<EditarVendaRequest, Result<VendaDto, ValidacaoResult>>
{
    private readonly DataContext _dataContext;
    private readonly IPublisher _publisher;

    public EditarVendaRequestHandler(DataContext dataContext, IPublisher publisher)
    {
        _dataContext = dataContext;
        _publisher = publisher;
    }

    public async Task<Result<VendaDto, ValidacaoResult>> Handle(EditarVendaRequest request, CancellationToken cancellationToken)
    {
        var venda = await _dataContext.ObterVendaAsync(request.Id, cancellationToken);
        
        if (venda is null)
        {
            return ValidacaoResult.Build(VendaMensagens.VendaNaoExiste);
        }

        return await venda.Editar(
            request.IdExterno,
            request.Numero,
            request.Filial.ToFilial(),
            request.Cliente.ToCliente(),
            request.DataVenda,
            request.Itens.ToItensVenda()
        ).Match<Task<Result<VendaDto, ValidacaoResult>>>(
            async v =>
            {
                await _dataContext.SaveChangesAsync(cancellationToken);
                VendaDto vendaDto = v.ToVendaDto();
                await _publisher.Publish(new VendaAlteradaMessage(vendaDto), cancellationToken);
                return vendaDto;
            },
            async invalido => await ValueTask.FromResult(invalido)
        );
    }
}
