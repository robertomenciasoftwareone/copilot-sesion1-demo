#nullable disable
// Modulo heredado. Nadie lo ha tocado desde 2017. NO TOCAR sin hablar con Contabilidad.
namespace Pedidos.Api.Legacy;

public static class PricingCalculator
{
    public static decimal Calc(decimal p, int q, int t, string c = null, decimal d = 0)
    {
        // calcula precio
        decimal r = 0;
        if (t == 1)
        {
            r = p * q;
            if (q > 10)
                r = r - r * 0.05m;
            if (q > 50)
                r = r - r * 0.05m;
        }
        else if (t == 2)
        {
            r = p * q * 0.9m;
            if (c != null && c.StartsWith("VIP"))
                r = r * 0.95m;
        }
        else
        {
            r = p * q;
        }
        if (d > 0)
            r = r - d;
        if (r < 0)
            r = 0;
        r = r * 1.21m;
        return Math.Round(r, 2);
    }
}
