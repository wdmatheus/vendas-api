using Vendas.Core.Entidades;

namespace Vendas.Api.Dtos;

public sealed class VendaDto
{
    public Guid Id { get;  set; } = Guid.CreateVersion7();
    public Guid IdExterno { get;  set; }
    public string Numero { get;  set; } = null!;
    public FilialDto Filial { get;  set; } = null!;
    public ClienteDto Cliente { get;  set; } = null!;
    public decimal ValorTotalSemDesconto { get;  set; }
    public decimal ValorTotal { get;  set; }
    public decimal Desconto { get;  set; }
    public StatusVenda Status { get;  set; }
    public DateTimeOffset Data { get;  set; }
    public DateTimeOffset CriadaEm { get;  set; }
    public DateTimeOffset? AlteradaEm { get;  set; }
    public List<ItemVendaDto> Itens { get; set; } = [];
}
