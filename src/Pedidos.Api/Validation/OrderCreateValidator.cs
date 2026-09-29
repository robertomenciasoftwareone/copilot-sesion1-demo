using Pedidos.Api.Models;

namespace Pedidos.Api.Validation;

public static class OrderCreateValidator
{
    public static Dictionary<string, string[]> Validate(OrderCreate data)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(data.Customer))
        {
            errors[nameof(OrderCreate.Customer)] = ["El cliente es obligatorio."];
        }
        else if (data.Customer.Length > 100)
        {
            errors[nameof(OrderCreate.Customer)] = ["El cliente no puede superar los 100 caracteres."];
        }

        if (string.IsNullOrWhiteSpace(data.Product))
        {
            errors[nameof(OrderCreate.Product)] = ["El producto es obligatorio."];
        }

        if (data.Quantity is < 1 or > 1000)
        {
            errors[nameof(OrderCreate.Quantity)] = ["La cantidad debe estar entre 1 y 1000."];
        }

        if (data.UnitPrice <= 0)
        {
            errors[nameof(OrderCreate.UnitPrice)] = ["El precio unitario debe ser mayor que cero."];
        }
        else if (((decimal.GetBits(data.UnitPrice)[3] >> 16) & 0xFF) > 2)
        {
            errors[nameof(OrderCreate.UnitPrice)] = ["El precio unitario debe tener como máximo 2 decimales."];
        }

        return errors;
    }
}