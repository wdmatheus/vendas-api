namespace Vendas.Api.Endpoints;

internal static class DocsEndpoints
{
    internal static void Map(WebApplication app)
    {
        app.MapGet("/docs", 
                async context => await context.Response.WriteAsync(Html))
            .ExcludeFromDescription()
            .AllowAnonymous();
            
        app.MapGet("/docs/index.html", 
            async context => await context.Response.WriteAsync(Html))
            .ExcludeFromDescription()
            .AllowAnonymous();
    }
    
    private static string Html =>
        """
        <html>
        <head>
            <meta charset="utf-8">
            <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
            <title>V1 Docs</title>            
            <script src="https://unpkg.com/@stoplight/elements/web-components.min.js"></script>            
            <link rel="stylesheet" href="https://unpkg.com/@stoplight/elements/styles.min.css">
            <style>
              body {
                display: flex;
                flex-direction: column;
                height: 100vh;
              }
              main {
                flex: 1 0 0;
                overflow: hidden;
              }              
            </style>
        </head>
        <body>
        <!-- Begin page content -->
        <main role="main">
           <elements-api
              apiDescriptionUrl="../openapi/v1.json"
              router="hash"
              logo=""
              hideSchemas=true/>
        </main>
        </body>
        </html>
        """;
}
