namespace Vendas.Core.Entidades;

public readonly struct VendaMensagens
{
    public const string VendaNaoExiste = "A venda não existe.";
    public const string VendaSemItemsErro = "A venda deve conter ao menos um item com staus NaoCancelado.";
    public const string VendaCanceladaEdicaoErro = "A venda foi cancelada e não pode ser editada";

    public static string ItemVendaNaoExiste(string idItemOuCodProduto) =>
        $"Não existe item de venda com id/cód. produto {idItemOuCodProduto}.";

    public static string ValorTotalItemNegativoErro(string codigoProduto) =>
        $"O valor total do item com produto código {codigoProduto} não pode ser negativo.";
}
