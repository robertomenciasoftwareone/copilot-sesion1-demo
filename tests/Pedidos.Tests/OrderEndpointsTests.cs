using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Pedidos.Api.Legacy;

namespace Pedidos.Tests;

public class OrderEndpointsTests
{
    [Fact]
    public async Task ListOrders_SeededDatabase_Returns60Orders()
    {
        using var factory = new PedidosApiFactory();
        var client = factory.CreateClient();

        var json = await client.GetFromJsonAsync<JsonElement>("/orders");

        Assert.Equal(60, json.GetArrayLength());
    }

    [Fact]
    public async Task GetOrder_UnknownId_Returns404()
    {
        using var factory = new PedidosApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/orders/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_ValidData_CalculatesTotalWithVat()
    {
        using var factory = new PedidosApiFactory();
        var client = factory.CreateClient();
        var payload = new { customer = "ACME", product = "Formación", quantity = 2, unitPrice = 100 };

        var response = await client.PostAsJsonAsync("/orders", payload);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(242m, json.GetProperty("total").GetDecimal());
    }

    [Fact]
    public void Calc_SmallQuantity_OnlyAddsVat()
    {
        var result = PricingCalculator.Calc(10m, 5, 1);

        Assert.Equal(60.50m, result);
    }
}
