using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Vendas.Api.Features.Vendas.CommonRequests;

public record ItemVendaRequest(
    [property: Required, Range(1, int.MaxValue)] decimal Quantidade,
    [property: Required, Range(1, int.MaxValue)] decimal ValorUnitario,
    [property: Range(0, int.MaxValue)] decimal? Desconto,
    [property: Required] ProdutoRequest Produto
);

public sealed class ItemVendaRequestValidator : AbstractValidator<ItemVendaRequest>
{
    public const string QuantidadeMinimaErro = "A quantidade do item de venda deve ser maior que zero.";
    public const string ValorUnitarioMinimoErro = "O valor unitário do item de venda deve ser maior que zero.";
    public const string DescontoMinimoErro = "O desconto do item de venda deve ser maior/igual que zero.";
    public const string ProdutoRequeridoErro = "O produto do item de venda é obrigatório.";
    
    public ItemVendaRequestValidator()
    {
        RuleFor(x => x.Quantidade)
            .GreaterThan(0)
            .WithMessage(QuantidadeMinimaErro);

        RuleFor(x => x.ValorUnitario)
            .GreaterThan(0)
            .WithMessage(ValorUnitarioMinimoErro);
        
        RuleFor(x => x.Desconto)
            .GreaterThanOrEqualTo(0)
            .WithMessage(DescontoMinimoErro)
            .When(x => x.Desconto.HasValue);

        RuleFor(x => x.Produto)
            .NotNull()
            .WithMessage(ProdutoRequeridoErro);
        
        RuleFor(x => x.Produto)
            .SetValidator(new ProdutoRequestValidator())
            .When(x => x.Produto is not null);
    }
}

