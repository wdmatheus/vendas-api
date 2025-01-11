using Vendas.Api.Config;

var builder = WebApplication.CreateSlimBuilder(args);

builder.ConfigureServices()
    .ConfigureServer()
    .ConfigureProblemsDetails()
    .ConfigureLogger();

var app = builder.Build();

app.UseResponseCompression();
app.UseExceptionHandler();
app.MapOpenApi();
app.UseStatusCodePages();
app.UseDeveloperExceptionPage();

app.MapGet("/", () => "Hello World!");

app.Run();

public partial class Program
{
    
}
