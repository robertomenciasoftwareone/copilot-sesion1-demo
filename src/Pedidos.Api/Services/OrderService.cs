using Microsoft.Data.Sqlite;
using Pedidos.Api.Data;
using Pedidos.Api.Exceptions;
using Pedidos.Api.Legacy;
using Pedidos.Api.Models;

namespace Pedidos.Api.Services;

public sealed class OrderService(Database db)
{
    /// <summary>
    /// Obtiene todos los pedidos ordenados por identificador.
    /// </summary>
    /// <returns>La lista de pedidos almacenados.</returns>
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

    /// <summary>
    /// Obtiene un pedido por su identificador.
    /// </summary>
    /// <param name="id">Identificador del pedido.</param>
    /// <returns>El pedido solicitado.</returns>
    /// <exception cref="OrderNotFoundException">Si no existe un pedido con el identificador indicado.</exception>
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

    /// <summary>
    /// Calcula el total y crea un pedido con estado pendiente.
    /// </summary>
    /// <param name="data">Datos del pedido que se desea crear.</param>
    /// <returns>El pedido creado, incluido su identificador y total calculado.</returns>
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
