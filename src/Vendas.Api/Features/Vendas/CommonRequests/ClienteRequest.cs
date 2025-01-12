using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FluentValidation;
using Vendas.Core.ValueObjects;

namespace Vendas.Api.Features.Vendas.CommonRequests;

public sealed record ClienteRequest(
    [property: Required] string Nome,
    [property: Required] string Cnpj,
    [property: Required] Guid IdExterno
)
{
    [JsonIgnore]
    public Cnpj CnpjVo => Cnpj;
}

public sealed class ClienteRequestValidator : AbstractValidator<ClienteRequest>
{
    public const string NomeRequeridoErro = "O nome do cliente é obrigatório.";
    public const string CnpjRequeridoErro = "O CNPJ do cliente é obrigatório.";
    public const string CnpjInvalidoErro = "O CNPJ do cliente é inválido.";
    public const string IdExternoInvalidoErro = "O id externo do cliente é inválido.";
    
    public ClienteRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage(NomeRequeridoErro);

        RuleFor(x => x.Cnpj)
            .NotEmpty()
            .WithMessage(CnpjRequeridoErro);
        
        RuleFor(x => x.Cnpj)
            .Must((request, _) => request.CnpjVo.EhValido())
            .When(x => !string.IsNullOrWhiteSpace(x.Cnpj))
            .WithMessage(CnpjInvalidoErro);
        
        RuleFor(x => x.IdExterno)
            .NotEmpty()
            .WithMessage(IdExternoInvalidoErro);
    }
}
