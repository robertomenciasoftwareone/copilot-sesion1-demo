using System.Text.Json.Serialization;
using Pedidos.Api.Data;
using Pedidos.Api.Exceptions;
using Pedidos.Api.Models;
using Pedidos.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var relative = config["Pedidos:DbPath"] ?? "../../data/pedidos.db";
    return new Database(Path.GetFullPath(Path.Combine(env.ContentRootPath, relative)));
});
builder.Services.AddScoped<OrderService>();

var app = builder.Build();
app.Services.GetRequiredService<Database>().EnsureCreated();

app.MapGet("/orders", (OrderService orders) => orders.ListOrders());

app.MapGet("/orders/{id:int}", (int id, OrderService orders) =>
{
    try
    {
        return Results.Ok(orders.GetOrder(id));
    }
    catch (OrderNotFoundException ex)
    {
        return Results.NotFound(new { detail = ex.Message });
    }
});

app.MapPost("/orders", (OrderCreate data, OrderService orders) =>
{
    var errors = data.Validate();
    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var order = orders.CreateOrder(data);
    return Results.Created($"/orders/{order.Id}", order);
});

app.Run();

public partial class Program { }
