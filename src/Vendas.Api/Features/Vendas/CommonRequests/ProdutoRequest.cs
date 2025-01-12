using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Vendas.Api.Features.Vendas.CommonRequests;

public sealed record ProdutoRequest(
    [property: Required] string Codigo,
    [property: Required] string Ean,
    [property: Required] string Nome,
    [property: Required] string Descricao,
    [property: Required] Guid IdExterno
);

public sealed class ProdutoRequestValidator : AbstractValidator<ProdutoRequest>
{
    public const string CodigoRequeridoErro = "O código do produto é obrigatório.";
    public const string EanRequeridoErro = "O ean do produto é obrigatório.";
    public const string NomeRequeridoErro = "O nome do produto é obrigatório.";
    public const string DescricaoRequeridoErro = "A descrção do produto é obrigatório.";
    public const string IdExternoInvalidoErro = "O id externo do produto é inválido.";
    
    public ProdutoRequestValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty()
            .WithMessage(CodigoRequeridoErro);

        RuleFor(x => x.Ean)
            .NotEmpty()
            .WithMessage(EanRequeridoErro);

        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage(NomeRequeridoErro);

        RuleFor(x => x.Descricao)
            .NotEmpty()
            .WithMessage(DescricaoRequeridoErro);
        
        RuleFor(x => x.IdExterno)
            .NotEmpty()
            .WithMessage(IdExternoInvalidoErro);
    }
}