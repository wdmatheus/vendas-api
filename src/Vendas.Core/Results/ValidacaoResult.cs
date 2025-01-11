namespace Vendas.Core.Results;

public sealed class ValidacaoResult
{
    private List<string> _erros = [];
    public IReadOnlyList<string> Erros => _erros.AsReadOnly();
    
    
    public static ValidacaoResult Build(params string[] erros) => new()
    {
        _erros = [..erros]
    };
}