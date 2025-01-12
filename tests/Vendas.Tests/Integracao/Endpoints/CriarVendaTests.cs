using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Vendas.Core.Entidades;
using Vendas.Tests.Utils;

namespace Vendas.Tests.Integracao.Endpoints;

public sealed class CriarVendaTests : IClassFixture<ApiFixture>
{
    private readonly ApiFixture _apiFixture;

    public CriarVendaTests(ApiFixture apiFixture) => _apiFixture = apiFixture;

    [Fact]
    public async Task DeveCriarVenda()
    {
        //Arrange
        var payload = RequestsFakerFactory.BuildCriarVendaRequest(1);
        
        using var request = new HttpRequestMessage(HttpMethod.Post, $"v1/vendas");
        request.Content = new StringContent(
            JsonSerializer.Serialize(payload, JsonHelper.JsonSerializerOptions),
            Encoding.UTF8,
            MediaTypeNames.Application.Json
        );

        //Act
        var response = await _apiFixture.ApiClient.SendAsync(request);

        //Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Fact]
    public async Task DeveRetornarErroQuandoHaItensInvalidos()
    {
        //Arrange
        var payload = RequestsFakerFactory.BuildCriarVendaRequest(1);
        payload.Itens[0] = payload.Itens[0] with
        {
            Quantidade = 1,
            ValorUnitario = 1,
            Desconto = 10
        };
        
        using var request = new HttpRequestMessage(HttpMethod.Post, $"v1/vendas");
        request.Content = new StringContent(
            JsonSerializer.Serialize(payload, JsonHelper.JsonSerializerOptions),
            Encoding.UTF8,
            MediaTypeNames.Application.Json
        );
        
        var erroEsperado = VendaMensagens.ValorTotalItemNegativoErro(payload.Itens[0].Produto.Codigo);

        //Act
        var response = await _apiFixture.ApiClient.SendAsync(request);
        var vendaResponse = await response.Content.ReadAsStringAsync();
        var erros = JsonDocument.Parse(vendaResponse).RootElement.GetProperty("errors")
            .EnumerateArray()
            .Select(x => x.GetString())
            .ToArray();

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        erros.Should().ContainEquivalentOf(erroEsperado);
    }
    
    [Fact]
    public async Task DeveRetornarErroQuandoPayloadEhInvalido()
    {
        //Arrange
        var payload = RequestsFakerFactory.BuildCriarVendaRequest(1) with
        {
            Cliente = null!,
            Filial = null!,
            Numero = null!,
            IdExterno = Guid.NewGuid()
        };
        
        using var request = new HttpRequestMessage(HttpMethod.Post, $"v1/vendas");
        request.Content = new StringContent(
            JsonSerializer.Serialize(payload, JsonHelper.JsonSerializerOptions),
            Encoding.UTF8,
            MediaTypeNames.Application.Json
        );

        //Act
        var response = await _apiFixture.ApiClient.SendAsync(request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
}
