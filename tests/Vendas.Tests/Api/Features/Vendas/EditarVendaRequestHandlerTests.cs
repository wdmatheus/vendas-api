using FluentAssertions;
using MediatR;
using MockQueryable.NSubstitute;
using NSubstitute;
using Vendas.Api.Config;
using Vendas.Api.Features.Vendas;
using Vendas.Api.Features.Vendas.Notifications;
using Vendas.Core.Entidades;
using Vendas.Data;
using Vendas.Tests.Utils;

namespace Vendas.Tests.Api.Features.Vendas;

public class EditarVendaRequestHandlerTests
{
    private readonly DataContext _dataContext = Substitute.For<DataContext>();
    private readonly IPublisher _publisher = Substitute.For<IPublisher>();
    private readonly EditarVendaRequestHandler _handler;

    public EditarVendaRequestHandlerTests() =>
        _handler = new EditarVendaRequestHandler(_dataContext, _publisher);
        
    [Fact]
    public async Task DeveEditarVenda()
    {
        //Arrange
        var request = RequestsFakerFactory.BuildEditarVendaRequest(1);
        
        var venda = new Venda(
            request.IdExterno,
            request.Numero,
            request.Filial.ToFilial(),
            request.Cliente.ToCliente(),
            request.DataVenda,
            request.Itens.ToItensVenda()
        );

        request.Id = venda.Id;

        var vendasQueryable = new[] { venda }.AsQueryable().BuildMockDbSet();
        _dataContext.Vendas.Returns(vendasQueryable);
        _dataContext.ObterVendaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(venda);

        //Act
        var result = await _handler.Handle(request, CancellationToken.None);

        //Assert
        result.IsT0.Should()
            .BeTrue();
        result.AsT0.Should()
            .NotBeNull();

        await _dataContext.Received(1)
            .ObterVendaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _dataContext.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
        await _publisher.Received(1)
            .Publish(Arg.Any<VendaAlteradaMessage>(), Arg.Any<CancellationToken>());
    }
    

    [Fact]
    public async Task NaoDeveEditarVendaCancelada()
    {
        //Arrange
        var request = RequestsFakerFactory.BuildEditarVendaRequest(1);
        
        var venda = new Venda(
            request.IdExterno,
            request.Numero,
            request.Filial.ToFilial(),
            request.Cliente.ToCliente(),
            request.DataVenda,
            request.Itens.ToItensVenda()
        );
        venda.Cancelar();

        request.Id = venda.Id;

        var vendasQueryable = new[] { venda }.AsQueryable().BuildMockDbSet();
        _dataContext.Vendas.Returns(vendasQueryable);
        _dataContext.ObterVendaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(venda);

        //Act
        var result = await _handler.Handle(request, CancellationToken.None);

        //Assert
        result.IsT1.Should()
            .BeTrue();
        result.AsT1.Erros.Should()
            .ContainEquivalentOf(VendaMensagens.VendaCanceladaEdicaoErro);

        await _dataContext.Received(1)
            .ObterVendaAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
