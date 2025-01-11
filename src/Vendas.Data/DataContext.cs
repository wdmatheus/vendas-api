using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Vendas.Core.Entidades;
using Vendas.Data.Configuration;

namespace Vendas.Data;

public class DataContext : DbContext
{
    [ExcludeFromCodeCoverage]
    public DataContext()
    {
        
    }   

    [ExcludeFromCodeCoverage]
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public virtual DbSet<Venda> Vendas { get; set; } = null!;

    public virtual async ValueTask<Venda?> ObterVendaAsync(Guid id, CancellationToken ct)
    {
        var venda = await Vendas
            .Include(x => x.Itens)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return venda;
    }

    [ExcludeFromCodeCoverage]
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VendaMap).Assembly);
    }
}

[ExcludeFromCodeCoverage]
public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseCustomNpgsql(
        this DbContextOptionsBuilder optionsBuilder,
        string connectionString,
        bool isDevelopment) =>
        optionsBuilder.UseNpgsql(connectionString, options => options.MapEnum<StatusVenda>())
            .EnableSensitiveDataLogging(isDevelopment)
            .UseSnakeCaseNamingConvention();
}

/// <summary>
/// To run migrations on Data project folder
/// </summary>
[ExcludeFromCodeCoverage]
public class DbContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        var builder = new DbContextOptionsBuilder<DataContext>();
        builder.UseCustomNpgsql(config.GetConnectionString("Default")!, true);
        return new DataContext(builder.Options);
    }
}
