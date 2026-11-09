using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payment_Service.Infrastructure;
using Payment_Service.Infrastructure.Persistence;
using Payment_Service.Infrastructure.Persistence.Impl;
using Payment_Service.Infrastructure.RabbitMQ.Consumer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Database
builder.Services.AddDbContext<PaymentDBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<RideCompletedConsumer>();

    // Send message to rabbitMQ configuration
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"]!);
            h.Password(builder.Configuration["RabbitMq:Password"]!);
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// if (builder.Configuration.GetValue<bool>("Database:ApplyMigrations"))
// {
//     await using var scope = app.Services.CreateAsyncScope();
//     await scope.ServiceProvider
//         .GetRequiredService<PaymentDBContext>()
//         .Database
//         .MigrateAsync();
// }

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
