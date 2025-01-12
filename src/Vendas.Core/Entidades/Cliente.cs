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
    
    public string Nome { get; private set; } = null!;
    
    public Cnpj Cnpj { get; private set; } = null!;
    
    public Guid IdExterno { get; private set; }
}
