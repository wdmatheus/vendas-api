namespace Vendas.Core.Entidades;

public sealed class ItemVenda
{
    public ItemVenda()
    {
    }
    
    public ItemVenda(decimal quantidade, decimal valorUnitario, decimal desconto, Produto produto)
    {
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
        Desconto = desconto;
        Produto = produto;
        CalcularValorTotal();
    }
    
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public decimal Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public decimal ValorTotalSemDesconto { get; private set; }
    public decimal ValorTotal { get; private set; }
    public decimal Desconto { get; private set; }
    public Produto Produto { get; private set; } = null!;
    
    private void CalcularValorTotal()
    {
        ValorTotalSemDesconto = Quantidade * ValorUnitario;
        ValorTotal = ValorTotalSemDesconto - Desconto;
    }
}
