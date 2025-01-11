using FluentAssertions;
using Vendas.Core.Entidades;
using Vendas.Tests.Utils;

namespace Vendas.Tests.Core.Entidades;

public class VendaTests
{
    [Fact]
    public void VendaValidaDeveSerCriada()
    {
        // Arrange
        var venda = EntidadesFakerFactory.BuildVenda();

        // Act
        var ehValida = venda.EhValida();

        // Assert
        ehValida.IsT0.Should().BeTrue();
        ehValida.AsT0.Status.Should().Be(StatusVenda.Criada);
    }

    [Fact]
    public void VendaDeveFalharValidacaoSemItens()
    {
        // Arrange
        var venda = new Venda(
            Guid.NewGuid(),
            "12345",
            EntidadesFakerFactory.BuildFilial(),
            EntidadesFakerFactory.BuildCliente(),
            DateTimeOffset.UtcNow,
            []);

        var mensagemErroEsperada = VendaMensagens.VendaSemItemsErro;

        // Act
        var resultado = venda.EhValida();

        // Assert
        resultado.IsT1.Should().BeTrue();
        resultado.AsT1.Erros.Should().ContainEquivalentOf(mensagemErroEsperada);
    }

    [Fact]
    public void VendaNaoDeveSerCriadaComItemInvalido()
    {
        // Arrange
        var vendaInvalida = new ItemVenda(1, 1, 10, EntidadesFakerFactory.BuildProduto());
        var items = EntidadesFakerFactory.BuildItensVenda(1)
            .Concat([vendaInvalida])
            .ToList();
        var venda = EntidadesFakerFactory.BuildVenda(items);

        var mensagemErroEsperada = VendaMensagens.ValorTotalItemNegativoErro(vendaInvalida.Produto.Codigo);

        // Act
        var ehValida = venda.EhValida();

        // Assert
        ehValida.IsT1.Should().BeTrue();
        ehValida.AsT1.Erros.Should().ContainEquivalentOf(mensagemErroEsperada);
    }

    [Fact]
    public void VendaDeveSerCancelada()
    {
        // Arrange
        var venda = EntidadesFakerFactory.BuildVenda();

        // Act
        venda.Cancelar();

        // Assert
        venda.Status.Should().Be(StatusVenda.Cancelada);
    }

    [Fact]
    public void VendaNaoDeveSerEditadaSeEstiverCancelada()
    {
        // Arrange
        var venda = EntidadesFakerFactory.BuildVenda();
        venda.Cancelar();

        var novoIdExterno = Guid.NewGuid();
        var novoNumero = "12345";
        var novaFilial = EntidadesFakerFactory.BuildFilial();
        var novoCliente = EntidadesFakerFactory.BuildCliente();
        var novaData = DateTimeOffset.UtcNow;
        var novosItens = EntidadesFakerFactory.BuildItensVenda(2);

        var mensagemErroEsperada = VendaMensagens.VendaCanceladaEdicaoErro;

        // Act
        var resultado = venda.Editar(novoIdExterno, novoNumero, novaFilial, novoCliente, novaData, novosItens);

        // Assert
        resultado.IsT1.Should().BeTrue();
        resultado.AsT1.Erros.Should().ContainEquivalentOf(mensagemErroEsperada);
    }

    [Fact]
    public void VendaNaoDeveSerEditadaComItensDeValorInvalido()
    {
        // Arrange
        var venda = EntidadesFakerFactory.BuildVenda();

        var novoIdExterno = Guid.NewGuid();
        var novoNumero = "12345";
        var novaFilial = EntidadesFakerFactory.BuildFilial();
        var novoCliente = EntidadesFakerFactory.BuildCliente();
        var novaData = DateTimeOffset.UtcNow;
        var itensInvalidos = EntidadesFakerFactory.BuildItensVenda(1)
            .Concat([
                new ItemVenda(1, 1, 10, EntidadesFakerFactory.BuildProduto())
            ])
            .ToList();

        var mensagemErroEsperada = VendaMensagens.ValorTotalItemNegativoErro(itensInvalidos[1].Produto.Codigo);

        // Act
        var resultado = venda.Editar(novoIdExterno, novoNumero, novaFilial, novoCliente, novaData, itensInvalidos);

        // Assert
        resultado.IsT1.Should().BeTrue();
        resultado.AsT1.Erros.Should().ContainEquivalentOf(mensagemErroEsperada);
    }

    [Fact]
    public void VendaDeveSerEditadaComSucesso()
    {
        // Arrange
        var venda = EntidadesFakerFactory.BuildVenda();

        var novoIdExterno = Guid.NewGuid();
        var novoNumero = "12345";
        var novaFilial = EntidadesFakerFactory.BuildFilial();
        var novoCliente = EntidadesFakerFactory.BuildCliente();
        var novaData = DateTimeOffset.UtcNow;
        var novosItens = EntidadesFakerFactory.BuildItensVenda(2);

        // Act
        var resultado = venda.Editar(novoIdExterno, novoNumero, novaFilial, novoCliente, novaData, novosItens);

        // Assert
        resultado.IsT0.Should().BeTrue();
        resultado.AsT0.IdExterno.Should().Be(novoIdExterno);
        resultado.AsT0.Numero.Should().Be(novoNumero);
        resultado.AsT0.Filial.Should().Be(novaFilial);
        resultado.AsT0.Cliente.Should().Be(novoCliente);
        resultado.AsT0.Data.Should().Be(novaData);
        resultado.AsT0.Itens.Should().BeEquivalentTo(novosItens);
    }

    [Fact]
    public void VendaNaoDeveRemoverItemQueNaoExiste()
    {
        // Arrange
        var venda = EntidadesFakerFactory.BuildVenda();
        var idItemOuCodProdutoInexistente = "ItemInexistente";

        var mensagemErroEsperada = VendaMensagens.ItemVendaNaoExiste(idItemOuCodProdutoInexistente);

        // Act
        var resultado = venda.RemoverItem(idItemOuCodProdutoInexistente);

        // Assert
        resultado.IsT1.Should().BeTrue();
        resultado.AsT1.Erros.Should().ContainEquivalentOf(mensagemErroEsperada);
    }

    [Fact]
    public void VendaDeveRemoverItemComSucesso()
    {
        // Arrange
        var venda = EntidadesFakerFactory.BuildVenda();
        var item = venda.Itens[0];
        var idItemOuCodProduto = item.Id.ToString();

        // Act
        var resultado = venda.RemoverItem(idItemOuCodProduto);

        // Assert
        resultado.IsT0.Should().BeTrue();
        venda.Itens.Should().NotContain(item);
    }
}
