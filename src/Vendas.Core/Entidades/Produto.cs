namespace Vendas.Core.Entidades;

public sealed class Produto
{
    public Produto()
    {
        
    }
    
    public Produto(string codigo, string ean, string nome, string descricao, Guid idExterno) 
    {
        Codigo = codigo;
        Ean = ean;
        Nome = nome;
        Descricao = descricao;
        IdExterno = idExterno;
    }
    
    public string Codigo { get; private set; } = null!;
    public string Ean { get; private set; } = null!;
    public string Nome { get; private set; } = null!;
    public string Descricao { get; private set; } = null!;
    public Guid IdExterno { get; private set; } 
}
