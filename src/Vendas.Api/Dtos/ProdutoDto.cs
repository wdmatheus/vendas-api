namespace Vendas.Api.Dtos;

public sealed class ProdutoDto
{
    public string Codigo { get;  set; } = null!;
    public string Ean { get;  set; } = null!;
    public string Nome { get;  set; } = null!;
    public string Descricao { get;  set; } = null!;
    public Guid IdExterno { get;  set; }
}
