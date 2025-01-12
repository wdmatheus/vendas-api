using System.Net;
using System.Text.Json;
using FluentAssertions;
using Vendas.Core.Entidades;

namespace Vendas.Tests.Integracao.Endpoints;

public class RemoverVendaTests : IClassFixture<ApiFixture>
{
    private readonly ApiFixture _apiFixture;

    public RemoverVendaTests(ApiFixture apiFixture) => _apiFixture = apiFixture;

    [Fact]
    public async Task DeveRemoverVenda()
    {
        // Arrange
        var venda = _apiFixture.ListaVendas[0];
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"v1/vendas/{venda.Id}");

        // Act
        var response = await _apiFixture.ApiClient.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task RemoverVendaNaoExistenteDeveFalhar()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"v1/vendas/{Guid.NewGuid()}");

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
