using FluentAssertions;
using Vendas.Api.Features.Vendas.CommonRequests;
using Vendas.Tests.Utils;

namespace Vendas.Tests.Api.Features.Vendas.CommonRequests;

public class ProdutoRequestValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void DeveRetornarErroQuandoCodigoNaoEhInformado(string codigo)
    {
        // Arrange
        var produtoRequest = RequestsFakerFactory.BuildProdutoRequest() with { Codigo = codigo };

        // Act
        var result = new ProdutoRequestValidator().Validate(produtoRequest);

        // Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(ProdutoRequestValidator.CodigoRequeridoErro);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void DeveRetornarErroQuandoEanNaoEhInformado(string ean)
    {
        // Arrange
        var produtoRequest = RequestsFakerFactory.BuildProdutoRequest() with { Ean = ean };

        // Act
        var result = new ProdutoRequestValidator().Validate(produtoRequest);

        // Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(ProdutoRequestValidator.EanRequeridoErro);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void DeveRetornarErroQuandoNomeNaoEhInformado(string nome)
    {
        // Arrange
        var produtoRequest = RequestsFakerFactory.BuildProdutoRequest() with { Nome = nome };

        // Act
        var result = new ProdutoRequestValidator().Validate(produtoRequest);

        // Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(ProdutoRequestValidator.NomeRequeridoErro);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void DeveRetornarErroQuandoDescricaoNaoEhInformado(string descricao)
    {
        // Arrange
        var produtoRequest = RequestsFakerFactory.BuildProdutoRequest() with { Descricao = descricao };

        // Act
        var result = new ProdutoRequestValidator().Validate(produtoRequest);

        // Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(ProdutoRequestValidator.DescricaoRequeridoErro);
    }

    [Fact]
    public void DeveRetornarErroQuandoIdExternoNaoEhInformado()
    {
        // Arrange
        var produtoRequest = RequestsFakerFactory.BuildProdutoRequest() with { IdExterno = Guid.Empty };

        // Act
        var result = new ProdutoRequestValidator().Validate(produtoRequest);

        // Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(ProdutoRequestValidator.IdExternoInvalidoErro);
    }

    [Fact]
    public void DeveRetornarSucessoQuandoTodosOsCamposSaoInformados()
    {
        // Arrange
        var produtoRequest = RequestsFakerFactory.BuildProdutoRequest();

        // Act
        var result = new ProdutoRequestValidator().Validate(produtoRequest);

        // Assert
        result.IsValid
            .Should()
            .BeTrue();
    }
}
