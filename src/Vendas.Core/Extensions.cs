using System.Text.RegularExpressions;

namespace Vendas.Core;

public static partial class Extensions
{
    [GeneratedRegex("[^0-9]", RegexOptions.Compiled)]
    private static partial Regex ApenasNumerosRegex();
    
    public static string ApenasNumeros(this string value) => ApenasNumerosRegex().Replace(value, "");
}
