using FluentValidation;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.SystemConsole.Themes;
using Vendas.Data;

namespace Vendas.Api.Config;

internal static class WebApplicationBuilderExtensions
{
    internal static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<Program>()
        );
        
        return builder;
    }
    
    internal static WebApplicationBuilder ConfigureServer(this WebApplicationBuilder builder)
    {
        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes;
        });

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        });

        builder.Services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info.Title = "Vendas API V1";
                document.Info.Contact = new OpenApiContact
                {
                    Name = "OminaGht",
                    Email = "contato@omihaght.com.br"
                };
                return Task.CompletedTask;
            });
        });

        return builder;
    }
    
    internal static WebApplicationBuilder ConfigureProblemsDetails(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                if (activity is not null)
                {
                    context.ProblemDetails.Extensions.TryAdd("traceId", activity.Id);
                }

                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
            };
        });

        builder.Services.AddExceptionHandler<ApiExceptionHandler>();

        return builder;
    }
    
    internal static WebApplicationBuilder ConfigureDatabase(this WebApplicationBuilder builder)
    {
        var connection = builder.Configuration.GetConnectionString("Default")!;

        builder.Services.AddDbContextPool<DataContext>(optionsBuilder =>
            optionsBuilder.UseCustomNpgsql(connection, builder.Environment.IsDevelopment())
                .UseModel(Data.CompiledModels.DataContextModel.Instance)
        );

        if (builder.Environment.IsDevelopment())
        {
            var context = builder.Services.BuildServiceProvider().GetRequiredService<DataContext>();
            context.Database.Migrate();
        }

        return builder;
    }

    internal static WebApplicationBuilder ConfigureLogger(this WebApplicationBuilder builder)
    {
        const string template = "[{Timestamp:yyyy-MM-dd HH:mm:ss.ffffff} {Level:u15}] {Message:lj}{NewLine}{Exception}";

        var loggerConfiguration = new LoggerConfiguration();

        Log.Logger = loggerConfiguration.Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithCorrelationId()
            .WriteTo.Async(wt =>
                wt.Console(outputTemplate: template, theme: AnsiConsoleTheme.Code)
            )
            .CreateLogger();

        builder.Host.UseSerilog();
        
        builder.Services.AddLogging(logBuilder =>
        {
            logBuilder.ClearProviders();
            logBuilder.AddSerilog(logger: Log.Logger, dispose: true);
        });

        return builder;
    }
}
