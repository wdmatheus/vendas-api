namespace Vendas.Api.Dtos;

public sealed class FilialDto
{
    public string Numero { get;  set; } = null!;
    public string Nome { get;  set; } = null!;
    public Guid IdExterno { get;  set; }
}