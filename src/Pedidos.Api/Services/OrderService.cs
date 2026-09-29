using Microsoft.Data.Sqlite;
using Pedidos.Api.Data;
using Pedidos.Api.Exceptions;
using Pedidos.Api.Legacy;
using Pedidos.Api.Models;

namespace Pedidos.Api.Services;

public sealed class OrderService(Database db)
{
    public IReadOnlyList<Order> ListOrders()
    {
        using var connection = db.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM orders ORDER BY id";

        using var reader = command.ExecuteReader();
        var orders = new List<Order>();
        while (reader.Read())
        {
            orders.Add(Map(reader));
        }
        return orders;
    }

    public Order GetOrder(int id)
    {
        using var connection = db.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM orders WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            throw new OrderNotFoundException(id);
        }
        return Map(reader);
    }

    public Order CreateOrder(OrderCreate data)
    {
        var total = PricingCalculator.Calc(data.UnitPrice, data.Quantity, 1, data.Customer);

        using var connection = db.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO orders (customer, product, quantity, unit_price, status, total)
            VALUES ($customer, $product, $quantity, $price, $status, $total);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("$customer", data.Customer);
        command.Parameters.AddWithValue("$product", data.Product);
        command.Parameters.AddWithValue("$quantity", data.Quantity);
        command.Parameters.AddWithValue("$price", data.UnitPrice);
        command.Parameters.AddWithValue("$status", OrderStatus.Pending.ToString());
        command.Parameters.AddWithValue("$total", total);

        var newId = Convert.ToInt32(command.ExecuteScalar());
        return GetOrder(newId);
    }

    public Order CancelOrder(int id, string reason)
    {
        using var connection = db.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE orders
            SET status = $cancelled, cancel_reason = $reason
            WHERE id = $id AND status IN ($pending, $paid)
            """;
        command.Parameters.AddWithValue("$cancelled", OrderStatus.Cancelled.ToString());
        command.Parameters.AddWithValue("$reason", reason);
        command.Parameters.AddWithValue("$id", id);
        command.Parameters.AddWithValue("$pending", OrderStatus.Pending.ToString());
        command.Parameters.AddWithValue("$paid", OrderStatus.Paid.ToString());

        if (command.ExecuteNonQuery() == 0)
        {
            var order = GetOrder(id);
            throw new OrderCannotBeCancelledException(id, order.Status);
        }

        return GetOrder(id);
    }

    private static Order Map(SqliteDataReader r) => new(
        r.GetInt32(r.GetOrdinal("id")),
        r.GetString(r.GetOrdinal("customer")),
        r.GetString(r.GetOrdinal("product")),
        r.GetInt32(r.GetOrdinal("quantity")),
        r.GetDecimal(r.GetOrdinal("unit_price")),
        Enum.Parse<OrderStatus>(r.GetString(r.GetOrdinal("status")), ignoreCase: true),
        r.GetDecimal(r.GetOrdinal("total")),
        r.IsDBNull(r.GetOrdinal("cancel_reason")) ? null : r.GetString(r.GetOrdinal("cancel_reason")));
}
