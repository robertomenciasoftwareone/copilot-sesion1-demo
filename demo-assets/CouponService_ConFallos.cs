// DEMO 3: copiar a src/Pedidos.Api/Services/CouponService.cs en la rama feature/cupones y abrir PR.
// Contiene fallos intencionados para que Copilot code review los detecte.
using Pedidos.Api.Data;

namespace Pedidos.Api.Services;

public class CouponService(Database db)
{
    private const string ApiKey = "sk-live-1234567890abcdef";

    public decimal ApplyCoupon(int orderId, string couponCode)
    {
        var conn = db.Open();
        var cmd = conn.CreateCommand();
        // buscar cupon
        cmd.CommandText = $"SELECT discount FROM coupons WHERE code = '{couponCode}'";
        var discount = Convert.ToDecimal(cmd.ExecuteScalar());
        cmd.CommandText = $"SELECT total FROM orders WHERE id = {orderId}";
        var total = Convert.ToDecimal(cmd.ExecuteScalar());
        var newTotal = total - total * discount / 100;
        cmd.CommandText = $"UPDATE orders SET total = {newTotal} WHERE id = {orderId}";
        cmd.ExecuteNonQuery();
        return newTotal;
    }
}
