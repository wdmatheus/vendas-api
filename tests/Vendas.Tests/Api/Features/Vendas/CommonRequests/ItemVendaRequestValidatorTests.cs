using FluentAssertions;
using Vendas.Api.Features.Vendas.CommonRequests;
using Vendas.Tests.Utils;

namespace Vendas.Tests.Api.Features.Vendas.CommonRequests;

public class ItemVendaRequestValidatorTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DeveRetornarErroQuandoQuantidadeEhInvalida(decimal quantidade)
    {
        // Arrange
        var itemVendaRequest = RequestsFakerFactory.BuildItemVendaRequest()[0] with { Quantidade = quantidade };

        // Act
        var result = new ItemVendaRequestValidator().Validate(itemVendaRequest);

        // Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(ItemVendaRequestValidator.QuantidadeMinimaErro);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DeveRetornarErroQuandoValorUnitarioEhInvalido(decimal valorUnitario)
    {
        // Arrange
        var itemVendaRequest = RequestsFakerFactory.BuildItemVendaRequest()[0] with { ValorUnitario = valorUnitario };

        // Act
        var result = new ItemVendaRequestValidator().Validate(itemVendaRequest);

        // Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(ItemVendaRequestValidator.ValorUnitarioMinimoErro);
    }

    [Fact]
    public void DeveRetornarErroQuandoDescontoEhInvalido()
    {
        // Arrange
        var itemVendaRequest = RequestsFakerFactory.BuildItemVendaRequest()[0] with { Desconto = -1 };

        // Act
        var result = new ItemVendaRequestValidator().Validate(itemVendaRequest);

        // Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(ItemVendaRequestValidator.DescontoMinimoErro);
    }

    [Fact]
    public void DeveRetornarErroQuandoProdutoNaoEhInformado()
    {
        // Arrange
        var itemVendaRequest = RequestsFakerFactory.BuildItemVendaRequest()[0] with { Produto = null! };

        // Act
        var result = new ItemVendaRequestValidator().Validate(itemVendaRequest);

        // Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(ItemVendaRequestValidator.ProdutoRequeridoErro);
    }

    [Fact]
    public void DeveRetornarSucessoQuandoTodosOsCamposSaoInformados()
    {
        // Arrange
        var itemVendaRequest = RequestsFakerFactory.BuildItemVendaRequest()[0] ;

        // Act
        var result = new ItemVendaRequestValidator().Validate(itemVendaRequest);

        // Assert
        result.IsValid
            .Should()
            .BeTrue();
    }
}
