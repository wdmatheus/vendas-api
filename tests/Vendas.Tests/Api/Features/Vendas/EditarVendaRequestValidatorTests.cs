using FluentAssertions;
using Vendas.Api.Features.Vendas;
using Vendas.Api.Features.Vendas.CommonRequests;
using Vendas.Tests.Utils;

namespace Vendas.Tests.Api.Features.Vendas;

public class EditarVendaRequestValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void DeveRetornarErroQuandoNumeroNaoEhInformado(string numero)
    {
        // Arrange
        var editarVendaRequest = RequestsFakerFactory.BuildEditarVendaRequest(1) with { Numero = numero };

        // Act
        var result = new EditarVendaRequestValidator().Validate(editarVendaRequest);

        // Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(VendaRequestValidatorExtensions.NumeroRequeridoErro);
    }

    [Fact]
    public void DeveRetornarErroQuandoIdExternoNaoEhInformado()
    {
        // Arrange
        var editarVendaRequest = RequestsFakerFactory.BuildEditarVendaRequest(1) with { IdExterno = Guid.Empty };

        // Act
        var result = new EditarVendaRequestValidator().Validate(editarVendaRequest);

        // Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(VendaRequestValidatorExtensions.IdExternoInvalidoErro);
    }

    [Fact]
    public void DeveRetornarErroQuandoFilialNaoEhInformada()
    {
        // Arrange
        var editarVendaRequest = RequestsFakerFactory.BuildEditarVendaRequest(1) with { Filial = null! };

        // Act
        var result = new EditarVendaRequestValidator().Validate(editarVendaRequest);

        // Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(VendaRequestValidatorExtensions.FilialRequeridaErro);
    }

    [Fact]
    public void DeveRetornarErroQuandoClienteNaoEhInformado()
    {
        // Arrange
        var editarVendaRequest = RequestsFakerFactory.BuildEditarVendaRequest(1) with { Cliente = null! };

        // Act
        var result = new EditarVendaRequestValidator().Validate(editarVendaRequest);

        // Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(VendaRequestValidatorExtensions.ClienteRequeridoErro);
    }

    [Fact]
    public void DeveRetornarSucessoQuandoTodosOsCamposSaoInformados()
    {
        // Arrange
        var editarVendaRequest = RequestsFakerFactory.BuildEditarVendaRequest(1);

        // Act
        var result = new EditarVendaRequestValidator().Validate(editarVendaRequest);

        // Assert
        result.IsValid
            .Should()
            .BeTrue();
    }
}
