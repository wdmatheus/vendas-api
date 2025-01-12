using FluentAssertions;
using MediatR;
using MockQueryable.NSubstitute;
using NSubstitute;
using Vendas.Api.Features.Vendas;
using Vendas.Api.Features.Vendas.Notifications;
using Vendas.Core.Entidades;
using Vendas.Data;
using Vendas.Tests.Utils;

namespace Vendas.Tests.Api.Features.Vendas;

public class CancelarVendaRequestHandlerTests
{
    private readonly DataContext _dataContext = Substitute.For<DataContext>();
    private readonly IPublisher _publisher = Substitute.For<IPublisher>();
    private readonly CancelarVendaRequestHandler _handler;

    public CancelarVendaRequestHandlerTests() =>
        _handler = new CancelarVendaRequestHandler(_dataContext, _publisher);

    [Fact]
    public async Task DeveCancelarVenda()
    {
        //Arrange
        var venda = EntidadesFakerFactory.BuildVenda();
        List<Venda> vendas = [venda];
        var vendaQueryable = vendas.AsQueryable().BuildMockDbSet();
        _dataContext.Vendas.Returns(vendaQueryable);
        _dataContext.ObterVendaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(vendas[0]);

        //Act
        var result = await _handler.Handle(new CancelarVendaRequest(venda.Id), CancellationToken.None);

        //Assert
        result.IsT0.Should().BeTrue();
        result.AsT0.Status.Should().Be(StatusVenda.Cancelada);

        await _dataContext.Received(1)
            .ObterVendaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _dataContext.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
        await _publisher.Received(1)
            .Publish(Arg.Any<VendaCanceladaMessage>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task DeveRetornarErroQuandoVendaNaoExiste()
    {
        //Arrange
        var vendaQueryable = new List<Venda>().AsQueryable().BuildMockDbSet();
        _dataContext.Vendas.Returns(vendaQueryable);
        _dataContext.ObterVendaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(default(Venda));

        //Act
        var result = await _handler.Handle(new CancelarVendaRequest(Guid.NewGuid()), CancellationToken.None);

        //Assert
        result.IsT1.Should().BeTrue();
        result.AsT1.Erros.Should()
            .ContainEquivalentOf(VendaMensagens.VendaNaoExiste);

        await _dataContext.Received(1)
            .ObterVendaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
