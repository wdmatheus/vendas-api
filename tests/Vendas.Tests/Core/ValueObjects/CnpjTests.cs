using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Vendas.Core.ValueObjects;

namespace Vendas.Tests.Core.ValueObjects;

public class CnpjTests
{
    public static ICollection<object[]> Cnpjs =>
        Enumerable.Range(1, 10)
            .Select(x => new object[] { new Faker().Company.Cnpj() })
            .ToList();

    [Theory]
    [MemberData(nameof(Cnpjs))]
    public void CnpjValidoDeveRetornarTrue(string numeroCnpj)
    {
        // Arrange
        var cnpj = new Cnpj(numeroCnpj);

        // Act
        var ehValido = cnpj.EhValido();

        // Assert
        ehValido.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("00000000000000")]
    [InlineData("00.000.000/0000-00")]
    [InlineData("12.345.678/9012-34")]
    public static void CnpjInvalidoDeveRetornarFalse(string numeroCnpj)
    {
        // Arrange
        var cnpj = new Cnpj(numeroCnpj);

        // Act
        var ehValido = cnpj.EhValido();

        // Assert
        ehValido.Should().BeFalse();
    }
}
