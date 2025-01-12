using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Vendas.Api.Dtos;

namespace Vendas.Tests.Integracao.Endpoints;

public class ObterVendaTests : IClassFixture<ApiFixture>
{
    private readonly ApiFixture _apiFixture;

    public ObterVendaTests(ApiFixture apiFixture) => _apiFixture = apiFixture;

    [Fact]
    public async Task DeveObterVenda()
    {
        // Arrange
        var venda = _apiFixture.ListaVendas[0];
        using var request = new HttpRequestMessage(HttpMethod.Get, $"v1/vendas/{venda.Id}");

        // Act
        var response = await _apiFixture.ApiClient.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();

        var vendaResponse = await response.Content.ReadFromJsonAsync<VendaDto>(JsonHelper.JsonSerializerOptions);
        vendaResponse!.Numero.Should().BeEquivalentTo(venda.Numero);
    }
    
    [Fact]
    public async Task ObterVendaNaoExistenteDeveRetornarNotFound()
    {
        // Arrange
        
        using var request = new HttpRequestMessage(HttpMethod.Get, $"v1/vendas/{Guid.NewGuid()}");

        // Act
        var response = await _apiFixture.ApiClient.SendAsync(request);

        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);
    }
}
