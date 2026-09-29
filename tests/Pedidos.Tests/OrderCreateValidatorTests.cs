using Pedidos.Api.Models;
using Pedidos.Api.Validation;

namespace Pedidos.Tests;

public class OrderCreateValidatorTests
{
    [Fact]
    public void Validate_CustomerMissing_ReturnsError()
    {
        var data = new OrderCreate(" ", "Producto", 1, 1m);

        var errors = OrderCreateValidator.Validate(data);

        Assert.Contains(nameof(OrderCreate.Customer), errors.Keys);
    }

    [Fact]
    public void Validate_CustomerOver100Characters_ReturnsError()
    {
        var data = new OrderCreate(new string('A', 101), "Producto", 1, 1m);

        var errors = OrderCreateValidator.Validate(data);

        Assert.Contains(nameof(OrderCreate.Customer), errors.Keys);
    }

    [Fact]
    public void Validate_ProductMissing_ReturnsError()
    {
        var data = new OrderCreate("Cliente", "", 1, 1m);

        var errors = OrderCreateValidator.Validate(data);

        Assert.Contains(nameof(OrderCreate.Product), errors.Keys);
    }

    [Fact]
    public void Validate_QuantityBelowOne_ReturnsError()
    {
        var data = new OrderCreate("Cliente", "Producto", 0, 1m);

        var errors = OrderCreateValidator.Validate(data);

        Assert.Contains(nameof(OrderCreate.Quantity), errors.Keys);
    }

    [Fact]
    public void Validate_QuantityAbove1000_ReturnsError()
    {
        var data = new OrderCreate("Cliente", "Producto", 1001, 1m);

        var errors = OrderCreateValidator.Validate(data);

        Assert.Contains(nameof(OrderCreate.Quantity), errors.Keys);
    }

    [Fact]
    public void Validate_UnitPriceZero_ReturnsError()
    {
        var data = new OrderCreate("Cliente", "Producto", 1, 0m);

        var errors = OrderCreateValidator.Validate(data);

        Assert.Contains(nameof(OrderCreate.UnitPrice), errors.Keys);
    }

    [Fact]
    public void Validate_UnitPriceOverTwoDecimals_ReturnsError()
    {
        var data = new OrderCreate("Cliente", "Producto", 1, 1.234m);

        var errors = OrderCreateValidator.Validate(data);

        Assert.Contains(nameof(OrderCreate.UnitPrice), errors.Keys);
    }

    [Fact]
    public void Validate_UnitPriceWithTrailingThirdDecimal_ReturnsError()
    {
        var data = new OrderCreate("Cliente", "Producto", 1, 1.230m);

        var errors = OrderCreateValidator.Validate(data);

        Assert.Contains(nameof(OrderCreate.UnitPrice), errors.Keys);
    }
}