namespace Vendas.Core.ValueObjects;

public sealed class Cnpj
{
    private static readonly int[] Multiplicador1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] Multiplicador2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

    private Cnpj()
    {
    }

    public Cnpj(string numero) => Numero = numero.ApenasNumeros();
    
    public string Numero { get; private set; } = null!;

    public bool EhValido()
    {
        if (Numero.Length != 14)
        {
            return false;
        }

        var digitosSaoIguais = true;
        var ultimoDigito = Numero[0];
        var totalDigito1 = 0;
        var totalDigito2 = 0;

        for (var i = 0; i < 12; i++)
        {
            var digit = Numero[i] - '0';
            if (Numero[i] != ultimoDigito)
            {
                digitosSaoIguais = false;
            }

            totalDigito1 += digit * Multiplicador1[i];
            totalDigito2 += digit * Multiplicador2[i];
        }

        if (digitosSaoIguais)
        {
            return false;
        }

        var dv1 = totalDigito1 % 11;
        dv1 = dv1 < 2 ? 0 : 11 - dv1;
        if (Numero[12] - '0' != dv1)
        {
            return false;
        }

        totalDigito2 += dv1 * Multiplicador2[12];
        var dv2 = totalDigito2 % 11;
        dv2 = dv2 < 2 ? 0 : 11 - dv2;
        return Numero[13] - '0' == dv2;
    }

    public static implicit operator Cnpj(string numero) => new(numero);
    public static explicit operator string(Cnpj cnpj) => cnpj.Numero;
    public override string ToString() => Numero;
}
