using System.Text.Json.Serialization;
using Vendas.Core;

namespace Vendas.Api.Dtos;

public sealed class ClienteDto
{
    public string Nome { get;  set; } = null!;
    
    [JsonIgnore]
    public string Cnpj { get;  set; } = null!;
    
    [JsonPropertyName("cnpj")]
    public string Formatado =>
        Convert.ToUInt64(Cnpj.ApenasNumeros()).ToString(@"00\.000\.000\/0000\-00");
    public Guid IdExterno { get;  set; }
}
