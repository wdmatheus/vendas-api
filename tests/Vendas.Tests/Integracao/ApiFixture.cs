using System.Security.Cryptography;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Vendas.Core.Entidades;
using Vendas.Data;
using Vendas.Tests.Utils;


namespace Vendas.Tests.Integracao;

public sealed class ApiFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _databaseContainer;
    public HttpClient ApiClient { get; private set; } = null!;
    public List<Venda> ListaVendas { get; }

    private readonly int _dbPort = RandomNumberGenerator.GetInt32(5433, 6000);

    private const string DbName = "vendas_omina";
    private const string DbUser = "postgres";
    private const string DbPwd = "123456";

    public ApiFixture()
    {
        ListaVendas = CriarVendas();

        _databaseContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase(DbName)
            .WithUsername(DbUser)
            .WithPassword(DbPwd)
            .WithPortBinding(_dbPort, 5432)
            .WithCleanUp(true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .Build();
    }

    private static List<Venda> CriarVendas()
    {
        var vendas = new List<Venda>(5);

        for (var i = 0; i < 5; i++)
        {
            vendas.Add(EntidadesFakerFactory.BuildVenda(i + 1));
        }

        return vendas;
    }

    public async Task InitializeAsync()
    {
        await _databaseContainer.StartAsync();

        await SeedDatabase();

#pragma warning disable CA2000
        ApiClient = new WebApplicationFactory<Program>()
#pragma warning restore CA2000
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DataContext>));
                    services.Remove(dbContextDescriptor!);

                    var dataContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DataContext));
                    services.Remove(dataContextDescriptor!);

                    var connectionString = _databaseContainer.GetConnectionString();

                    services.AddDbContextPool<DataContext>(optionsBuilder =>
                        optionsBuilder.UseCustomNpgsql(connectionString, true)
                            .UseModel(Vendas.Data.CompiledModels.DataContextModel.Instance)
                    );
                });
            }).CreateClient();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _databaseContainer.DisposeAsync();
    }

    public async Task SeedDatabase()
    {
        var builder = new DbContextOptionsBuilder<DataContext>();
        builder.UseCustomNpgsql(_databaseContainer.GetConnectionString(), true);
        await using var context = new DataContext(builder.Options);

        await context.Database.MigrateAsync();

        await context.Set<Venda>()
            .AddRangeAsync(ListaVendas);

        await context.SaveChangesAsync();
    }
}
