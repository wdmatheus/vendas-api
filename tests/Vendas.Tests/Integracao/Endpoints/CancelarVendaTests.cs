using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Vendas.Api.Dtos;
using Vendas.Core.Entidades;

namespace Vendas.Tests.Integracao.Endpoints;

public class CancelarVendaTests : IClassFixture<ApiFixture>
{
    private readonly ApiFixture _apiFixture;

    public CancelarVendaTests(ApiFixture apiFixture) => _apiFixture = apiFixture;

    [Fact]
    public async Task DeveCancelarVenda()
    {
        // Arrange
        var venda = _apiFixture.ListaVendas[0];
        using var request = new HttpRequestMessage(HttpMethod.Put, $"v1/vendas/{venda.Id}/cancelar");

        // Act
        var response = await _apiFixture.ApiClient.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();

        var vendaResponse = await response.Content.ReadFromJsonAsync<VendaDto>(JsonHelper.JsonSerializerOptions);
        vendaResponse!.Status.Should().Be(StatusVenda.Cancelada);
    }
    
    [Fact]
    public async Task CancelarVendaNaoExistenteDeveFalhar()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Put, $"v1/vendas/{Guid.NewGuid()}/cancelar");

        // Act
        var response = await _apiFixture.ApiClient.SendAsync(request);
        
        var vendaResponse = await response.Content.ReadAsStringAsync();
        
        var erros = JsonDocument.Parse(vendaResponse).RootElement.GetProperty("errors")
            .EnumerateArray()
            .Select(x => x.GetString())
            .ToArray();
        
        // Assert
        response.StatusCode.Should()
            .Be(HttpStatusCode.BadRequest);
        erros.Should().ContainEquivalentOf(VendaMensagens.VendaNaoExiste);
    }
}
