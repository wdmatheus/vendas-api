using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Vendas.Api.Dtos;
using Vendas.Api.Features.Vendas.CommonRequests;
using Vendas.Core.Entidades;
using Vendas.Core.Results;

namespace Vendas.Api.Config;

internal static class MapExtensions
{
    internal static Cliente ToCliente(this ClienteRequest request) =>
        new(request.Nome, request.CnpjVo, request.IdExterno);

    internal static Filial ToFilial(this FilialRequest request) =>
        new(request.Numero, request.Nome, request.IdExterno);

    internal static Produto ToProduto(this ProdutoRequest request) =>
        new(request.Codigo, request.Ean, request.Nome, request.Descricao, request.IdExterno);

    internal static ItemVenda ToItemVenda(this ItemVendaRequest request) =>
        new(request.Quantidade, request.ValorUnitario, request.Desconto ?? 0, request.Produto.ToProduto());
    
    internal static List<ItemVenda> ToItensVenda(this List<ItemVendaRequest> requests) =>
        requests.Select(x => x.ToItemVenda()).ToList();

    internal static Venda ToVenda(this VendaRequest request) =>
        new(request.IdExterno,
            request.Numero,
            request.Filial.ToFilial(),
            request.Cliente.ToCliente(),
            request.DataVenda,
            request.Itens.ToItensVenda()
        );
    
    internal static VendaDto ToVendaDto(this Venda venda) =>
        new()
        {
            Id = venda.Id,
            IdExterno = venda.IdExterno,
            Numero = venda.Numero,
            Filial = new FilialDto
            {
                Numero = venda.Filial.Numero,
                Nome = venda.Filial.Nome,
                IdExterno = venda.Filial.IdExterno
            },
            Cliente = new ClienteDto
            {
                Nome = venda.Cliente.Nome,
                Cnpj = venda.Cliente.Cnpj.Numero,
                IdExterno = venda.Cliente.IdExterno
            },
            ValorTotalSemDesconto = venda.ValorTotalSemDesconto,
            ValorTotal = venda.ValorTotal,
            Desconto = venda.Desconto,
            Status = venda.Status,
            Data = venda.Data,
            CriadaEm = venda.CriadaEm,
            AlteradaEm = venda.AlteradaEm,
            Itens = venda.Itens.Select(x => new ItemVendaDto
            {
                Quantidade = x.Quantidade,
                ValorUnitario = x.ValorUnitario,
                ValorTotalSemDesconto = x.ValorTotalSemDesconto,
                ValorTotal = x.ValorTotal,
                Desconto = x.Desconto,
                Produto = new ProdutoDto
                {
                    Codigo = x.Produto.Codigo,
                    Ean = x.Produto.Ean,
                    Nome = x.Produto.Nome,
                    Descricao = x.Produto.Descricao,
                    IdExterno = x.Produto.IdExterno
                }
            }).ToList()
        };
    

    internal static ProblemHttpResult ToBadRequestProblemDetails(this ValidacaoResult validacaoResult)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Bad Request"
        };
        problemDetails.Extensions.TryAdd("errors", validacaoResult.Erros);
        return TypedResults.Problem(problemDetails);
    }
    
    internal static ProblemHttpResult ToUnprocessableEntityProblemDetails(this ValidacaoResult validacaoResult)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "Unprocessable Entity"
        };
        problemDetails.Extensions.TryAdd("errors", validacaoResult.Erros);
        return TypedResults.Problem(problemDetails);
    }

    internal static ProblemHttpResult ToUnprocessableEntityProblemDetails(this List<ValidationFailure> validationFailures)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "Unprocessable Entity"
        };
        problemDetails.Extensions.TryAdd("errors", validationFailures.Select(x => x.ErrorMessage));
        return TypedResults.Problem(problemDetails);
    }
}
