using FluentAssertions;
using Vendas.Api.Features.Vendas.CommonRequests;
using Vendas.Tests.Utils;

namespace Vendas.Tests.Api.Features.Vendas.CommonRequests;

public class ClienteRequestValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void DeveRetornarErroQuandoNomeNaoEhInformado(string nome)
    {
        //Arrange
        var clienteRequest = RequestsFakerFactory.BuildClienteRequest() with { Nome = nome };

        //Act
        var result = new ClienteRequestValidator().Validate(clienteRequest);

        //Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(ClienteRequestValidator.NomeRequeridoErro);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void DeveRetornarErroQuandoCnpjNaoEhInformado(string cnpj)
    {
        //Arrange
        var clienteRequest = RequestsFakerFactory.BuildClienteRequest() with { Cnpj = cnpj };

        //Act
        var result = new ClienteRequestValidator().Validate(clienteRequest);

        //Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(ClienteRequestValidator.CnpjRequeridoErro);
    }

    [Fact]
    public void DeveRetornarErroQuandoCnpjEhInvalido()
    {
        //Arrange
        var clienteRequest = RequestsFakerFactory.BuildClienteRequest() with { Cnpj = "12345678901234" };

        //Act
        var result = new ClienteRequestValidator().Validate(clienteRequest);

        //Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(ClienteRequestValidator.CnpjInvalidoErro);
    }

    [Fact]
    public void DeveRetornarErroQuandoIdExternoNaoEhInformado()
    {
        //Arrange
        var clienteRequest = RequestsFakerFactory.BuildClienteRequest() with { IdExterno = Guid.Empty };

        //Act
        var result = new ClienteRequestValidator().Validate(clienteRequest);

        //Assert
        result.IsValid
            .Should()
            .BeFalse();

        result.Errors
            .Select(x => x.ErrorMessage)
            .Should()
            .ContainEquivalentOf(ClienteRequestValidator.IdExternoInvalidoErro);
    }

    [Fact]
    public void DeveRetornarSucessoQuandoTodosOsCamposSaoInformados()
    {
        //Arrange
        var clienteRequest = RequestsFakerFactory.BuildClienteRequest();

        //Act
        var result = new ClienteRequestValidator().Validate(clienteRequest);

        //Assert
        result.IsValid
            .Should()
            .BeTrue();
    }
}
