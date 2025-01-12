using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Vendas.Api.Features.Vendas.CommonRequests;

public sealed record FilialRequest(
    [property: Required] string Numero,
    [property: Required] string Nome,
    [property: Required] Guid IdExterno
);

public sealed class FilialRequestValidator : AbstractValidator<FilialRequest>
{
    public const string NumeroRequeridoErro = "O número da filial é obrigatório.";
    public const string NomeRequeridoErro = "O nome da filial é obrigatório.";
    public const string IdExternoInvalidoErro = "O id externo da filial é inválido.";
    
    public FilialRequestValidator()
    {
        RuleFor(x => x.Numero)
            .NotEmpty()
            .WithMessage(NumeroRequeridoErro);

        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage(NomeRequeridoErro);
        
        RuleFor(x => x.IdExterno)
            .NotEmpty()
            .WithMessage(IdExternoInvalidoErro);
    }
}
