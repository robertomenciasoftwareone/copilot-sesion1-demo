namespace Pedidos.Api.Exceptions;

/// <summary>El pedido solicitado no existe.</summary>
public sealed class OrderNotFoundException(int orderId)
    : Exception($"Pedido {orderId} no encontrado")
{
    public int OrderId { get; } = orderId;
}
