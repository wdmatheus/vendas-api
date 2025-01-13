using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vendas.Api.Config;
using Vendas.Api.Dtos;
using Vendas.Api.Features.Vendas;
using Vendas.Data;

namespace Vendas.Api.Endpoints;

internal static class VendaEndpoints
{
    internal static void Map(WebApplication app)
    {
        var group = app.MapGroup("/v1/vendas")
            .WithGroupName("v1")
            .WithTags("vendas");

        group.MapGet("{id:guid}", ObterAsync)
            .Produces<VendaDto>()
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithName("ObterPorId")
            .WithDescription("Obtém uma venda");

        group.MapPost("", CriarAsync)
            .Produces(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .WithName("CriarVenda")
            .WithDescription("Cria uma venda");

        group.MapPut("{id:guid}", EditarAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)
            .WithName("EditarVenda")
            .WithDescription("Edita uma venda");
        
        group.MapPut("{id:guid}/cancelar", CancelarAsync)
            .Produces<VendaDto>()
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("CancelarVenda")
            .WithDescription("Cancela uma venda");
        
        group.MapDelete("{id:guid}", RemoverAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("RemoverVenda")
            .WithDescription("Remove uma venda");
        
        group.MapDelete("{id:guid}/itens/{idItemOuCodProduto}", RemoverItemAsync)
            .Produces<VendaDto>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("RemoverItemVenda")
            .WithDescription("Remove um item da venda");
    }

    private static async ValueTask<IResult> ObterAsync(Guid id, DataContext dataContext, CancellationToken ct)
    {
        var venda = await dataContext.Vendas
            .AsNoTracking()
            .Include(x => x.Itens)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (venda is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(venda.ToVendaDto());
    }

    private static async ValueTask<IResult> CriarAsync(CriarVendaRequest request, ISender sender, CancellationToken ct)
    {
        var validacaoRequest = await new CriarVendaRequestValidator()
            .ValidateAsync(request, ct);

        if (!validacaoRequest.IsValid)
        {
            return validacaoRequest.Errors.ToUnprocessableEntityProblemDetails();
        }

        var response = await sender.Send(request, ct);

        return response.Match<IResult>(
            id => Results.CreatedAtRoute("ObterPorId", new RouteValueDictionary { { "id", id } }),
            validacao => validacao.ToUnprocessableEntityProblemDetails()
        );
    }

    private static async ValueTask<IResult> EditarAsync(Guid id, EditarVendaRequest request, ISender sender, CancellationToken ct)
    {
        var validacaoRequest = await new EditarVendaRequestValidator()
            .ValidateAsync(request, ct);

        if (!validacaoRequest.IsValid)
        {
            return validacaoRequest.Errors.ToUnprocessableEntityProblemDetails();
        }

        request = request with { Id = id };

        var response = await sender.Send(request, ct);

        return response.Match<IResult>(
            _ => TypedResults.NoContent(),
            validacao => validacao.ToUnprocessableEntityProblemDetails()
        );
    }
    
    private static async ValueTask<IResult> CancelarAsync(Guid id, ISender sender, CancellationToken ct)
    {
        var request = new CancelarVendaRequest(id);

        var response = await sender.Send(request, ct);

        return response.Match<IResult>(
            TypedResults.Ok,
            validacao => validacao.ToBadRequestProblemDetails()
        );
    }
    
    private static async ValueTask<IResult> RemoverAsync(Guid id, ISender sender, CancellationToken ct)
    {
        var request = new RemoverVendaRequest(id);

        var response = await sender.Send(request, ct);

        return response.Match<IResult>(
            _ => TypedResults.NoContent(),
            validacao => validacao.ToBadRequestProblemDetails()
        );
    }
    
    private static async ValueTask<IResult> RemoverItemAsync(Guid id, string idItemOuCodProduto, ISender sender, CancellationToken ct)
    {
        var request = new RemoverItemVendaRequest(id, idItemOuCodProduto);

        var response = await sender.Send(request, ct);

        return response.Match<IResult>(
            v => v is null ? TypedResults.NoContent() : TypedResults.Ok(v),
            validacao => validacao.ToBadRequestProblemDetails()
        );
    }
}
