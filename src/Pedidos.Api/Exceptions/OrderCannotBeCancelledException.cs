using Pedidos.Api.Models;

namespace Pedidos.Api.Exceptions;

/// <summary>El pedido existe, pero su estado no permite cancelarlo.</summary>
public sealed class OrderCannotBeCancelledException(int orderId, OrderStatus status)
    : Exception($"El pedido {orderId} no se puede cancelar porque está en estado {status}.")
{
    public int OrderId { get; } = orderId;
    public OrderStatus Status { get; } = status;
}
