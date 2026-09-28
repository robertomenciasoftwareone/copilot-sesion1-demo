using Microsoft.Data.Sqlite;

namespace Pedidos.Api.Data;

/// <summary>Acceso a la base de datos SQLite de pedidos y creación de datos de ejemplo.</summary>
public sealed class Database(string filePath)
{
    public string FilePath { get; } = filePath;

    public SqliteConnection Open()
    {
        var connection = new SqliteConnection($"Data Source={FilePath}");
        connection.Open();
        return connection;
    }

    public void EnsureCreated()
    {
        if (File.Exists(FilePath)) return;

        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        using var connection = Open();
        using var transaction = connection.BeginTransaction();

        using (var create = connection.CreateCommand())
        {
            create.CommandText = """
                CREATE TABLE orders (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    customer TEXT NOT NULL,
                    product TEXT NOT NULL,
                    quantity INTEGER NOT NULL,
                    unit_price REAL NOT NULL,
                    status TEXT NOT NULL,
                    total REAL NOT NULL,
                    cancel_reason TEXT
                );
                CREATE TABLE coupons (code TEXT PRIMARY KEY, discount REAL NOT NULL);
                INSERT INTO coupons VALUES ('BIENVENIDA10', 10), ('VIP20', 20);
                """;
            create.ExecuteNonQuery();
        }

        string[] customers = ["ACME", "VIP-Iberia", "Norte SL", "VIP-Levante", "Delta Retail", "Sur Logística"];
        string[] products = ["Licencia M365", "Soporte Premium", "Azure Credits", "Consultoría", "Formación"];
        decimal[] prices = [12.5m, 49.9m, 120m, 300m];
        var random = new Random(42);

        for (var i = 0; i < 60; i++)
        {
            var quantity = random.Next(1, 81);
            var price = prices[random.Next(prices.Length)];
            var roll = random.Next(12);
            var status = roll < 3 ? "Pending" : roll < 7 ? "Paid" : roll < 11 ? "Shipped" : "Cancelled";

            using var insert = connection.CreateCommand();
            insert.CommandText = """
                INSERT INTO orders (customer, product, quantity, unit_price, status, total, cancel_reason)
                VALUES ($customer, $product, $quantity, $price, $status, $total, $reason)
                """;
            insert.Parameters.AddWithValue("$customer", customers[random.Next(customers.Length)]);
            insert.Parameters.AddWithValue("$product", products[random.Next(products.Length)]);
            insert.Parameters.AddWithValue("$quantity", quantity);
            insert.Parameters.AddWithValue("$price", price);
            insert.Parameters.AddWithValue("$status", status);
            insert.Parameters.AddWithValue("$total", Math.Round(price * quantity * 1.21m, 2));
            insert.Parameters.AddWithValue("$reason", status == "Cancelled" ? (object)"Cliente desiste" : DBNull.Value);
            insert.ExecuteNonQuery();
        }

        transaction.Commit();
    }
}
