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
    public async Task CancelOrder_PendingOrder_ReturnsCancelledOrderWithReason()
    {
        using var factory = new PedidosApiFactory();
        var client = factory.CreateClient();
        var createResponse = await client.PostAsJsonAsync("/orders", new
        {
            customer = "ACME",
            product = "Formación",
            quantity = 1,
            unitPrice = 100
        });
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var orderId = createdOrder.GetProperty("id").GetInt32();

        var response = await client.PostAsJsonAsync($"/orders/{orderId}/cancel", new { reason = "Cliente desiste" });
        var cancelledOrder = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Cancelled", cancelledOrder.GetProperty("status").GetString());
        Assert.Equal("Cliente desiste", cancelledOrder.GetProperty("cancelReason").GetString());
    }

    [Fact]
    public async Task CancelOrder_UnknownId_Returns404()
    {
        using var factory = new PedidosApiFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/orders/99999/cancel", new { reason = "Cliente desiste" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("Shipped")]
    [InlineData("Cancelled")]
    public async Task CancelOrder_NoncancellableStatus_Returns409(string status)
    {
        using var factory = new PedidosApiFactory();
        var client = factory.CreateClient();
        var orders = await client.GetFromJsonAsync<JsonElement>("/orders");
        var orderId = orders.EnumerateArray()
            .First(order => order.GetProperty("status").GetString() == status)
            .GetProperty("id")
            .GetInt32();

        var response = await client.PostAsJsonAsync($"/orders/{orderId}/cancel", new { reason = "Cliente desiste" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task CancelOrder_EmptyReason_Returns400()
    {
        using var factory = new PedidosApiFactory();
        var client = factory.CreateClient();
        var createResponse = await client.PostAsJsonAsync("/orders", new
        {
            customer = "ACME",
            product = "Formación",
            quantity = 1,
            unitPrice = 100
        });
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var orderId = createdOrder.GetProperty("id").GetInt32();

        var response = await client.PostAsJsonAsync($"/orders/{orderId}/cancel", new { reason = "  " });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public void Calc_SmallQuantity_OnlyAddsVat()
    {
        var result = PricingCalculator.Calc(10m, 5, 1);

        Assert.Equal(60.50m, result);
    }
}
