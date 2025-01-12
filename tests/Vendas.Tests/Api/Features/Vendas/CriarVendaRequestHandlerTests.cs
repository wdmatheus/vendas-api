using FluentAssertions;
using MediatR;
using NSubstitute;
using Vendas.Api.Features.Vendas;
using Vendas.Api.Features.Vendas.Notifications;
using Vendas.Core.Entidades;
using Vendas.Data;
using Vendas.Tests.Utils;

namespace Vendas.Tests.Api.Features.Vendas;

public class CriarVendaRequestHandlerTests
{
    private readonly DataContext _dataContext = Substitute.For<DataContext>();
    private readonly IPublisher _publisher = Substitute.For<IPublisher>();
    private readonly CriarVendaRequestHandler _handler;
    
    public CriarVendaRequestHandlerTests() =>
        _handler = new CriarVendaRequestHandler(_dataContext, _publisher);
        
    [Fact]
    public async Task DeveCriarVenda()
    {
        //Arrange
        var request = RequestsFakerFactory.BuildCriarVendaRequest(2);

        //Act
        var result = await _handler.Handle(request, CancellationToken.None);

        //Assert
        result.IsT0.Should()
            .BeTrue();
        result.AsT0.Should()
            .NotBeEmpty();
        
        await _dataContext.Received(1).Vendas
            .AddAsync(Arg.Any<Venda>(), Arg.Any<CancellationToken>());
        await _dataContext.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
        await _publisher.Received(1)
            .Publish(Arg.Any<VendaCriadaMessage>(), Arg.Any<CancellationToken>());
    }
    

    [Fact]
    public async Task NaoDeveCriarVendaSemItens()
    {
        //Arrange
        var request = RequestsFakerFactory.BuildCriarVendaRequest(0) ;

        //Act
        var result = await _handler.Handle(request, CancellationToken.None);

        //Assert
        result.IsT1.Should()
            .BeTrue();
        result.AsT1.Erros.Should()
            .ContainEquivalentOf(VendaMensagens.VendaSemItemsErro);
    }
    
    [Fact]
    public async Task NaoDeveCriarVendaComItensInvalidos()
    {
        //Arrange
        var request = RequestsFakerFactory.BuildCriarVendaRequest(2);
        request.Itens[0] = request.Itens[0] with
        {
            ValorUnitario = 1,
            Quantidade = 1,
            Desconto = 2
        };
        var mensagemErroEsperada = VendaMensagens.ValorTotalItemNegativoErro(request.Itens[0].Produto.Codigo);
        

        //Act
        var result = await _handler.Handle(request, CancellationToken.None);

        //Assert
        result.IsT1.Should()
            .BeTrue();
        result.AsT1.Erros.Should()
            .ContainEquivalentOf(mensagemErroEsperada);
    }
}
