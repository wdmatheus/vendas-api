namespace Vendas.Api.Dtos;

public sealed class ItemVendaDto
{
    public decimal Quantidade { get;  set; }
    public decimal ValorUnitario { get;  set; }
    public decimal ValorTotalSemDesconto { get;  set; }
    public decimal ValorTotal { get;  set; }
    public decimal Desconto { get;  set; }
    public ProdutoDto Produto { get;  set; } = null!;
}