using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Vendas.Core.Entidades;
using Vendas.Data;
using Vendas.Tests.Utils;

namespace Vendas.Tests.Data;

public class DataContextTests
{
    private readonly DataContext _dataContext = Substitute.For<DataContext>();

    [Fact]
    public async Task DeveEncontrarVenda()
    {
        // Arrange
        var vendas = new List<Venda> { EntidadesFakerFactory.BuildVenda() };
        var idVenda = vendas[0].Id;
        var vendasDbSet = vendas.AsQueryable().BuildMockDbSet();
        _dataContext.Vendas.Returns(vendasDbSet);
        _dataContext.ObterVendaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(vendas[0]);

        // Act
        var venda = await _dataContext.ObterVendaAsync(idVenda, CancellationToken.None);

        // Assert
        venda.Should().NotBeNull();
    }

    [Fact]
    public async Task NaoDeveEncontrarVenda()
    {
        // Arrange
        var vendas = new List<Venda> { EntidadesFakerFactory.BuildVenda() };
        var idVenda = Guid.NewGuid();
        var vendasDbSet = vendas.AsQueryable().BuildMockDbSet();
        _dataContext.Vendas.Returns(vendasDbSet);
        _dataContext.ObterVendaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(default(Venda));

        // Act
        var venda = await _dataContext.ObterVendaAsync(idVenda, CancellationToken.None);

        // Assert
        venda.Should().BeNull();
    }
}
