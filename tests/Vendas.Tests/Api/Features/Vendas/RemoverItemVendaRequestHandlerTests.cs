using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using NSubstitute;
using Vendas.Api.Features.Vendas;
using Vendas.Api.Features.Vendas.Notifications;
using Vendas.Core.Entidades;
using Vendas.Data;
using Vendas.Tests.Utils;

namespace Vendas.Tests.Api.Features.Vendas;

public class RemoverItemVendaRequestHandlerTests
{
    private readonly DataContext _dataContext = Substitute.For<DataContext>();
    private readonly IPublisher _publisher = Substitute.For<IPublisher>();
    private readonly RemoverItemVendaRequestHandler _handler;

    public RemoverItemVendaRequestHandlerTests() =>
        _handler = new RemoverItemVendaRequestHandler(_dataContext, _publisher);

    [Fact]
    public async Task DeveRemoverVendaComUmItemAoRemoverItem()
    {
        //Arrange
        var venda = EntidadesFakerFactory.BuildVenda(1);
        var idItemOuCodProduto = venda.Itens[0].Id.ToString();
        DbSet<Venda> vendaQueryable = new[]{venda}.AsQueryable().BuildMockDbSet();
        _dataContext.Vendas.Returns(vendaQueryable);
        _dataContext.ObterVendaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(venda);

        //Act
        var result = await _handler.Handle(
            new RemoverItemVendaRequest(Guid.NewGuid(), idItemOuCodProduto), CancellationToken.None);

        //Assert
        result.IsT0.Should().BeTrue();
        result.AsT0.Should()
            .BeNull();

        await _dataContext.Received(1)
            .ObterVendaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        _dataContext.Received(1).Vendas.Remove(Arg.Any<Venda>());
        await _dataContext.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
        await _publisher.Received(0)
            .Publish(Arg.Any<ItemRemovidoMessage>(), Arg.Any<CancellationToken>());
        await _publisher.Received(1)
            .Publish(Arg.Any<VendaRemovidaMessage>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task DeveRemoverItemVenda()
    {
        //Arrange
        var venda = EntidadesFakerFactory.BuildVenda(2);
        var idItemOuCodProduto = venda.Itens[0].Id.ToString();
        DbSet<Venda> vendaQueryable = new[]{venda}.AsQueryable().BuildMockDbSet();
        _dataContext.Vendas.Returns(vendaQueryable);
        _dataContext.ObterVendaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(venda);

        //Act
        var result = await _handler.Handle(
            new RemoverItemVendaRequest(Guid.NewGuid(), idItemOuCodProduto), CancellationToken.None);

        //Assert
        result.IsT0.Should().BeTrue();
        result.AsT0.Should()
            .NotBeNull();
        result.AsT0!.Itens.Should()
            .HaveCount(1);

        await _dataContext.Received(1)
            .ObterVendaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        _dataContext.Received(0).Vendas.Remove(Arg.Any<Venda>());
        await _dataContext.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
        await _publisher.Received(1)
            .Publish(Arg.Any<ItemRemovidoMessage>(), Arg.Any<CancellationToken>());
        await _publisher.Received(0)
            .Publish(Arg.Any<VendaRemovidaMessage>(), Arg.Any<CancellationToken>());
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
        var result = await _handler.Handle(
            new RemoverItemVendaRequest(Guid.NewGuid(), "123"), CancellationToken.None);

        //Assert
        result.IsT1.Should().BeTrue();
        result.AsT1.Erros.Should()
            .ContainEquivalentOf(VendaMensagens.VendaNaoExiste);

        await _dataContext.Received(1)
            .ObterVendaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task DeveRetornarErroQuandoItemVendaNaoExiste()
    {
        //Arrange
        var idItemOuCodProduto = Guid.NewGuid().ToString();
        var mensagemErroEsperada = VendaMensagens.ItemVendaNaoExiste(idItemOuCodProduto);
        var venda = EntidadesFakerFactory.BuildVenda();
        DbSet<Venda> vendaQueryable = new[]{venda}.AsQueryable().BuildMockDbSet();
        _dataContext.Vendas.Returns(vendaQueryable);
        _dataContext.ObterVendaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(venda);

        //Act
        var result = await _handler.Handle(
            new RemoverItemVendaRequest(Guid.NewGuid(), idItemOuCodProduto), CancellationToken.None);

        //Assert
        result.IsT1.Should().BeTrue();
        result.AsT1.Erros.Should()
            .ContainEquivalentOf(mensagemErroEsperada);

        await _dataContext.Received(1)
            .ObterVendaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
