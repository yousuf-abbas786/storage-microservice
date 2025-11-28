using Asp.Versioning.ApiExplorer;
using Serilog;
using StorageService.API.Configs;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting Storage Service API");

    builder.Services.SetupVersioning();
    builder.Services.SetupSwagger();
    builder.Services.SetupOptions(builder.Configuration);
    builder.Services.SetupServices();
    builder.Services.SetupProviders(builder.Configuration);
    builder.Services.SetupAuthentication(builder.Configuration);
    builder.Services.SetupMiddleware();

    var app = builder.Build();

    await app.SetupDatabaseAsync();

    app.UseExceptionHandler();

    app.UseAuthentication();
    app.UseAuthorization();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            
            foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    $"Storage Service API {description.GroupName.ToUpperInvariant()}");
            }
        });
    }

    app.MapEndpoints();

    Log.Information("Storage Service API started successfully");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
