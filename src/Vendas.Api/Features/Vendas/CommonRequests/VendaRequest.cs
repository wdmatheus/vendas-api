using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;

namespace Vendas.Api.Features.Vendas.CommonRequests;

public record VendaRequest(
    [property: Required] string Numero,
    [property: Required] Guid IdExterno,
    [property: Required] FilialRequest Filial,
    [property: Required] ClienteRequest Cliente,
    [property: Required] DateTimeOffset DataVenda,
    List<ItemVendaRequest> Itens);

public static class VendaRequestValidatorExtensions
{
    public const string NumeroRequeridoErro = "O número da venda é obrigatório.";
    public const string IdExternoInvalidoErro = "O id externo da venda é inválido.";
    public const string FilialRequeridaErro = "A filial da venda é obrigatória.";
    public const string ClienteRequeridoErro = "O cliente da venda é obrigatório.";
    public const string ItensRequeridosErro = "Os itens da venda são obrigatórios.";
    public static string ProdutoRepetidoErro(string codigo) => $"O produto {codigo} está repetido em 2 ou mais itens na venda.";

    public static void RegisterRules<TRequest>(this AbstractValidator<TRequest> validator)
        where TRequest : VendaRequest
    {
        validator.RuleFor(x => x.Numero)
            .NotEmpty()
            .WithMessage(NumeroRequeridoErro);
        
        validator.RuleFor(x => x.IdExterno)
            .NotEmpty()
            .WithMessage(IdExternoInvalidoErro);

        validator.RuleFor(x => x.Filial)
            .NotEmpty()
            .WithMessage(FilialRequeridaErro);

        validator.RuleFor(x => x.Filial)
            .SetValidator(new FilialRequestValidator())
            .When(x => x.Filial is not null);

        validator.RuleFor(x => x.Cliente)
            .NotEmpty()
            .WithMessage(ClienteRequeridoErro);

        validator.RuleFor(x => x.Cliente)
            .SetValidator(new ClienteRequestValidator())
            .When(x => x.Cliente is not null);

        validator.RuleFor(x => x.Itens)
            .SetValidator(new ListaItemVendaRequestValidator<TRequest, ItemVendaRequest>());
    }
}

public class ListaItemVendaRequestValidator<T, TRequest> : PropertyValidator<T, IList<TRequest>> where TRequest : ItemVendaRequest
{
    public override bool IsValid(ValidationContext<T> context, IList<TRequest> itens)
    {
        if (itens is null || itens.Count == 0)
        {
            context.AddFailure(VendaRequestValidatorExtensions.ItensRequeridosErro);
            return true;
        }

        if (!ValidarItemsComProdutosRepetidos(context, itens))
        {
            return true;
        }

        if (!ValidarItemsVendas(context, itens))
        {
            return true;
        }

        return true;
    }

    private static bool ValidarItemsComProdutosRepetidos(ValidationContext<T> context, IList<TRequest> itens)
    {
        var repetidos = itens.Select(x => x.Produto.Codigo)
            .GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .Select(x => VendaRequestValidatorExtensions.ProdutoRepetidoErro(x.Key))
            .ToArray();

        if (repetidos.Length == 0)
        {
            return true;
        }

        foreach (var erro in repetidos)
        {
            context.AddFailure(erro);
        }

        return false;
    }

    private static bool ValidarItemsVendas(ValidationContext<T> context, IList<TRequest> itens)
    {
        var ehValido = true;

        var i = -1;

        var validador = new ItemVendaRequestValidator();

        foreach (var item in itens)
        {
            i++;
            var validacao = validador.Validate(item);
            if (validacao.IsValid)
            {
                continue;
            }
            AddErros(context, validacao.Errors, i);
            ehValido = false;
        }

        return ehValido;
    }

    private static void AddErros(ValidationContext<T> context, List<ValidationFailure> erros, int index)
    {
        foreach (var erro in erros)
        {
            context.AddFailure($"item[{index}]: {erro.ErrorMessage}");
        }
    }

    public override string Name => "ListaItemVendaRequestValidator";
}