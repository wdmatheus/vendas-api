using Vendas.Core.Results;

namespace Vendas.Core.Entidades;

public sealed class Venda
{
    public Venda()
    {
    }

    public Venda(Guid idExterno, string numero, Filial filial, Cliente cliente, DateTimeOffset data, List<ItemVenda> itens)
    {
        IdExterno = idExterno;
        Numero = numero;
        Filial = filial;
        Cliente = cliente;
        Status = StatusVenda.Criada;
        Data = data;
        CriadaEm = DateTimeOffset.UtcNow;
        _itens = itens;
    }
    
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid IdExterno { get; private set; }
    public string Numero { get; private set; } = null!;
    public Filial Filial { get; private set; } = null!;
    public Cliente Cliente { get; private set; } = null!;
    public decimal ValorTotalSemDesconto { get; private set; }
    public decimal ValorTotal { get; private set; }
    public decimal Desconto { get; private set; }
    public StatusVenda Status { get; private set; }
    public DateTimeOffset Data { get; private set; }
    public DateTimeOffset CriadaEm { get; private set; }
    public DateTimeOffset? AlteradaEm { get; private set; }
    
    private List<ItemVenda> _itens = [];
    public IReadOnlyList<ItemVenda> Itens => _itens.AsReadOnly();

    public Result<Venda, ValidacaoResult> Editar(Guid idExterno, string numero, Filial filial, Cliente cliente, DateTimeOffset data,
        List<ItemVenda> itens)
    {
        if (Status is StatusVenda.Cancelada)
        {
            return ValidacaoResult.Build(VendaMensagens.VendaCanceladaEdicaoErro);
        }

        IdExterno = idExterno;
        Numero = numero;
        Filial = filial;
        Cliente = cliente;
        Data = data;
        AlteradaEm = DateTimeOffset.UtcNow;
        EditarItens(itens);
        return EhValida();
    }

    private void EditarItens(List<ItemVenda> itens)
    {
        _itens.Clear();
        _itens = itens;
    }

    public Result<Venda, ValidacaoResult> EhValida() =>
        ItemsSaoValidos().Match<Result<Venda, ValidacaoResult>>(
            _ =>
            {
                AtualizarValorTotal();
                return this;
            },
            invalido => invalido
        );

    private void AtualizarValorTotal()
    {
        ValorTotalSemDesconto = _itens.Sum(i => i.ValorTotalSemDesconto);

        ValorTotal = _itens.Sum(i => i.ValorTotal);

        Desconto = _itens.Sum(i => i.Desconto);
    }


    private Result<bool, ValidacaoResult> ItemsSaoValidos()
    {
        if (_itens.Count == 0)
        {
            return ValidacaoResult.Build(VendaMensagens.VendaSemItemsErro);
        }

        var itensInvalidos = _itens.Where(x => x.ValorTotal < 0)
            .ToArray();

        if (itensInvalidos.Length > 0)
        {
            return ValidacaoResult.Build(
                itensInvalidos.Select(i => VendaMensagens.ValorTotalItemNegativoErro(i.Produto.Codigo))
                    .ToArray()
            );
        }

        return true;
    }

    public void Cancelar()
    {
        AlteradaEm = Status is not StatusVenda.Cancelada ? DateTimeOffset.UtcNow : AlteradaEm;
        Status = StatusVenda.Cancelada;
    }

    public Result<Venda, ValidacaoResult> RemoverItem(string idItemOuCodProduto)
    {
        var item = _itens.FirstOrDefault(x => 
            x.Id.ToString() == idItemOuCodProduto || x.Produto.Codigo == idItemOuCodProduto);
        
        if (item is null)
        {
            return ValidacaoResult.Build(VendaMensagens.ItemVendaNaoExiste(idItemOuCodProduto));
        }
        
        _itens.Remove(item);
        AtualizarValorTotal();
        AlteradaEm = DateTimeOffset.UtcNow;
        return this;
    }
    
}
