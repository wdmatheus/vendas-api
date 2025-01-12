using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Vendas.Api.Dtos;

namespace Vendas.Tests.Integracao.Endpoints;

public class RemoverItemVendaTests : IClassFixture<ApiFixture>
{
    private readonly ApiFixture _apiFixture;
    

    public RemoverItemVendaTests(ApiFixture apiFixture) => _apiFixture = apiFixture;

    [Fact]
    public async Task DeveRemoverItemVenda()
    {
        // Arrange
        var venda = _apiFixture.ListaVendas[^1];
        var itemVenda = venda.Itens[0];
        var totalItensEsperadoAposRemoverItem = venda.Itens.Count - 1;
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"v1/vendas/{venda.Id}/itens/{itemVenda.Id}");

        // Act
        var response = await _apiFixture.ApiClient.SendAsync(request);
        var vendaResponse = await response.Content.ReadFromJsonAsync<VendaDto>(JsonHelper.JsonSerializerOptions);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        vendaResponse!.Itens.Should().HaveCount(totalItensEsperadoAposRemoverItem);
    }
    
    [Fact]
    public async Task DeveRemoverItemVendaERemoverVenda()
    {
        // Arrange
        var venda = _apiFixture.ListaVendas[0];
        var itemVenda = venda.Itens[0];
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"v1/vendas/{venda.Id}/itens/{itemVenda.Id}");

        // Act
        var response = await _apiFixture.ApiClient.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
