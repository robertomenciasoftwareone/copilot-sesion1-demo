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

public sealed record OrderCreate(string Customer, string Product, int Quantity, decimal UnitPrice)
{
    public Dictionary<string, string[]> Validate()
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(Customer)) errors[nameof(Customer)] = ["El cliente es obligatorio."];
        if (string.IsNullOrWhiteSpace(Product)) errors[nameof(Product)] = ["El producto es obligatorio."];
        if (Quantity <= 0) errors[nameof(Quantity)] = ["La cantidad debe ser mayor que cero."];
        if (UnitPrice <= 0) errors[nameof(UnitPrice)] = ["El precio unitario debe ser mayor que cero."];
        return errors;
    }
}

public sealed record OrderCancellation(string? Reason)
{
    public Dictionary<string, string[]> Validate()
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(Reason)) errors[nameof(Reason)] = ["El motivo de cancelación es obligatorio."];
        return errors;
    }
}
