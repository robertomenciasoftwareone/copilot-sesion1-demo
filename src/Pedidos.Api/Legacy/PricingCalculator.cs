#nullable enable
namespace Pedidos.Api.Legacy;

/// <summary>
/// Calcula precios aplicando las reglas de descuentos e IVA del módulo heredado.
/// </summary>
public static class PricingCalculator
{
    private const decimal VatRate = 1.21m;
    private const decimal QuantityDiscountRate = 0.05m;
    private const decimal WholesaleDiscountRate = 0.9m;
    private const decimal VipDiscountRate = 0.95m;
    private const int FirstQuantityDiscountThreshold = 10;
    private const int SecondQuantityDiscountThreshold = 50;

    /// <summary>
    /// Calcula el importe total de un pedido, incluidos los descuentos aplicables y el IVA.
    /// </summary>
    /// <param name="unitPrice">Precio unitario del producto.</param>
    /// <param name="quantity">Cantidad de unidades.</param>
    /// <param name="pricingType">Tipo de cálculo de precio.</param>
    /// <param name="customerCode">Código del cliente, si está disponible.</param>
    /// <param name="discount">Descuento fijo que se resta del subtotal.</param>
    /// <returns>Importe total redondeado a dos decimales.</returns>
    public static decimal Calc(decimal unitPrice, int quantity, int pricingType, string? customerCode = null, decimal discount = 0)
    {
        decimal subtotal;
        if (pricingType == 1)
        {
            subtotal = unitPrice * quantity;
            if (quantity > FirstQuantityDiscountThreshold)
                subtotal -= subtotal * QuantityDiscountRate;
            if (quantity > SecondQuantityDiscountThreshold)
                subtotal -= subtotal * QuantityDiscountRate;
        }
        else if (pricingType == 2)
        {
            subtotal = unitPrice * quantity * WholesaleDiscountRate;
            if (customerCode != null && customerCode.StartsWith("VIP"))
                subtotal *= VipDiscountRate;
        }
        else
        {
            subtotal = unitPrice * quantity;
        }

        if (discount > 0)
            subtotal -= discount;
        if (subtotal < 0)
            subtotal = 0;

        subtotal *= VatRate;
        return Math.Round(subtotal, 2);
    }
}
