using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetryWithAspireDashboard;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CoffeeDbContext>(options =>
{
    options.UseInMemoryDatabase("CoffeeShopDB");
});

builder.Services.AddOpenTelemetry()
.ConfigureResource(resource => resource.AddService("CoffeeShop"))
.WithMetrics(metrics =>
{
    metrics.AddAspNetCoreInstrumentation();
    metrics.AddHttpClientInstrumentation();
    metrics.AddOtlpExporter();
})
.WithTracing(tracing =>
{
    tracing.AddAspNetCoreInstrumentation();
    tracing.AddHttpClientInstrumentation();
    tracing.AddAspNetCoreInstrumentation();
    tracing.AddOtlpExporter();
});

builder.Logging.AddOpenTelemetry(logging => logging.AddOtlpExporter());

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapPost("/Coffee", (CoffeeType coffeeType, CoffeeDbContext dbContext, ILogger<Program> logger) =>
{
    if (!Enum.IsDefined(coffeeType))
    {
        logger.LogWarning("Not a proper coffee type {coffeetype}", coffeeType);
        return Results.BadRequest();
    }
    var entry = dbContext.Sales.Add(new Sale
    {
        CoffeeType = coffeeType,
        CreatedDate = DateTime.Now,
    });
    dbContext.SaveChanges();

    logger.LogWarning("Sale saved successfully {@Sale}", entry.Entity);

    return Results.Ok(entry.Entity.Id);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

