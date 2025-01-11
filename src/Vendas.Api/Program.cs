using Vendas.Api.Config;
using Vendas.Api.Endpoints;

var builder = WebApplication.CreateSlimBuilder(args);

builder.ConfigureServices()
    .ConfigureServer()
    .ConfigureDatabase()
    .ConfigureProblemsDetails()
    .ConfigureLogger();

var app = builder.Build();

app.UseResponseCompression();
app.UseExceptionHandler();
app.MapOpenApi();
app.UseStatusCodePages();
app.UseDeveloperExceptionPage();
DocsEndpoints.Map(app);

app.Run();

public partial class Program
{
    
}
