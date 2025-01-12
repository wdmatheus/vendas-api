using FluentAssertions;
using Vendas.Api.Features.Vendas.CommonRequests;
using Vendas.Tests.Utils;

namespace Vendas.Tests.Api.Features.Vendas.CommonRequests;

public class FilialRequestValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void DeveRetornarErroQuandoNumeroNaoEhInformado(string numero)
    {
        // Arrange
        var filialRequest = RequestsFakerFactory.BuiFilialRequest() with { Numero = numero };

        // Act
        var result = new FilialRequestValidator().Validate(filialRequest);

        // Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(FilialRequestValidator.NumeroRequeridoErro);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void DeveRetornarErroQuandoNomeNaoEhInformado(string nome)
    {
        // Arrange
        var filialRequest = RequestsFakerFactory.BuiFilialRequest() with { Nome = nome };

        // Act
        var result = new FilialRequestValidator().Validate(filialRequest);

        // Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(FilialRequestValidator.NomeRequeridoErro);
    }

    [Fact]
    public void DeveRetornarErroQuandoIdExternoNaoEhInformado()
    {
        // Arrange
        var filialRequest = RequestsFakerFactory.BuiFilialRequest() with { IdExterno = Guid.Empty };

        // Act
        var result = new FilialRequestValidator().Validate(filialRequest);

        // Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(FilialRequestValidator.IdExternoInvalidoErro);
    }

    [Fact]
    public void DeveRetornarSucessoQuandoTodosOsCamposSaoInformados()
    {
        // Arrange
        var filialRequest = RequestsFakerFactory.BuiFilialRequest();

        // Act
        var result = new FilialRequestValidator().Validate(filialRequest);

        // Assert
        result.IsValid
            .Should()
            .BeTrue();
    }
}
