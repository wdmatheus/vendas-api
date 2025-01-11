using System.Text.Json.Serialization;
using Vendas.Core.ValueObjects;

namespace Vendas.Core.Entidades;

public sealed class Cliente
{
    public Cliente()
    {
        
    }
    
    public Cliente(string nome, Cnpj cnpj, Guid idExterno)
    {
        Nome = nome;
        Cnpj = cnpj;
        IdExterno = idExterno;
    }
    
    [JsonInclude]
    public string Nome { get; private set; } = null!;
    
    [JsonInclude]
    public Cnpj Cnpj { get; private set; } = null!;
    
    [JsonInclude]
    public Guid IdExterno { get; private set; }
}
