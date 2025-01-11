namespace Vendas.Core.Entidades;

public sealed class Filial
{
    public Filial()
    {
        
    }
    
    public Filial(string numero, string nome, Guid idExterno)
    {
        Numero = numero;
        Nome = nome;
        IdExterno = idExterno;
    }
    
    public string Numero { get; private set; } = null!;
    public string Nome { get; private set; } = null!;
    public Guid IdExterno { get; private set; }
}
