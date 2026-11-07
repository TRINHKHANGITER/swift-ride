using MatchingPricing_Service.Api.Middleware;
using MatchingPricing_Service.Application.Services;
using MatchingPricing_Service.Application.Services.Impl;
using MatchingPricing_Service.Application.Services.Util;
using MatchingPricing_Service.Infrastructure;
using MatchingPricing_Service.Infrastructure.Persistence;
using MatchingPricing_Service.Infrastructure.Persistence.Impl;
using Microsoft.Extensions.Options;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionHandler>();

builder.Services
    .AddOptions<MongoDBSetting>()
    .BindConfiguration("MongoDB")
    .Validate(
        x => !string.IsNullOrWhiteSpace(
            x.ConnectionString),
        "MongoDB ConnectionString không được để trống.")
    .Validate(
        x => !string.IsNullOrWhiteSpace(
            x.DatabaseName),
        "MongoDB DatabaseName không được để trống.")
    .ValidateOnStart();

builder.Services
    .AddOptions<MatchingSetting>()
    .BindConfiguration("Matching")
    .Validate(
        setting => double.IsFinite(setting.MaxDistanceMeters) && setting.MaxDistanceMeters > 0,
        "Matching MaxDistanceMeters phải lớn hơn 0.")
    .ValidateOnStart();

builder.Services.AddSingleton<IMongoClient>(
    serviceProvider =>
    {
        var settings = serviceProvider
            .GetRequiredService<
                IOptions<MongoDBSetting>>()
            .Value;

        return new MongoClient(
            settings.ConnectionString);
    });

builder.Services.AddSingleton<MongoDBContext>();
builder.Services.AddSingleton<MongoDBInitializer>();

// Using at controller 
builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<IMatchingService, MatchingDriver>();

// Repsitory service
builder.Services.AddSingleton<IDriverLocationRepository, DriverLocationRepository>();

// Other service
builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddHttpClient<IOSRMService, OSRMService>(client =>
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd("SwiftRide/1.0");
});


var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

await app.Services
    .GetRequiredService<MongoDBInitializer>()
    .InitializeAsync();

app.Run();
