namespace Pedidos.Api.Models;

public enum OrderStatus
{
    Pending,
    Paid,
    Shipped,
    Cancelled
}

public sealed record Order(
    int Id,
    string Customer,
    string Product,
    int Quantity,
    decimal UnitPrice,
    OrderStatus Status,
    decimal Total,
    string? CancelReason);

public sealed record OrderCreate(string Customer, string Product, int Quantity, decimal UnitPrice);
