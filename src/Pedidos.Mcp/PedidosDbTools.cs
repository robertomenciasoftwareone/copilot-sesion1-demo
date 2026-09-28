using System.ComponentModel;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using ModelContextProtocol.Server;

/// <summary>Herramientas MCP de solo lectura sobre la base de datos de pedidos (demo).</summary>
[McpServerToolType]
public static class PedidosDbTools
{
    private static string DbPath =>
        Environment.GetEnvironmentVariable("PEDIDOS_DB") ?? Path.GetFullPath("data/pedidos.db");

    private static SqliteConnection OpenReadOnly()
    {
        if (!File.Exists(DbPath))
        {
            throw new InvalidOperationException($"No existe {DbPath}. Ejecuta la API una vez para crearla.");
        }

        var connection = new SqliteConnection($"Data Source={DbPath};Mode=ReadOnly");
        connection.Open();
        return connection;
    }

    [McpServerTool(Name = "list_tables"), Description("Lista las tablas de la base de datos de pedidos.")]
    public static List<string> ListTables()
    {
        using var connection = OpenReadOnly();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite_%'";

        using var reader = command.ExecuteReader();
        var tables = new List<string>();
        while (reader.Read()) tables.Add(reader.GetString(0));
        return tables;
    }

    [McpServerTool(Name = "describe_table"), Description("Devuelve las columnas y tipos de una tabla.")]
    public static List<Dictionary<string, string>> DescribeTable([Description("Nombre de la tabla")] string table)
    {
        if (!ListTables().Contains(table))
        {
            throw new ArgumentException($"Tabla desconocida: {table}");
        }

        using var connection = OpenReadOnly();
        using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({table})"; // nombre validado contra la lista de tablas

        using var reader = command.ExecuteReader();
        var columns = new List<Dictionary<string, string>>();
        while (reader.Read())
        {
            columns.Add(new() { ["column"] = reader.GetString(1), ["type"] = reader.GetString(2) });
        }
        return columns;
    }

    [McpServerTool(Name = "run_query"), Description("Ejecuta una única consulta SELECT de solo lectura (máximo 200 filas).")]
    public static List<Dictionary<string, object?>> RunQuery([Description("Consulta SELECT")] string sql)
    {
        var trimmed = sql.Trim().TrimEnd(';');
        if (!Regex.IsMatch(trimmed, @"^\s*select\b", RegexOptions.IgnoreCase) || trimmed.Contains(';'))
        {
            throw new ArgumentException("Solo se permite una única sentencia SELECT");
        }

        using var connection = OpenReadOnly();
        using var command = connection.CreateCommand();
        command.CommandText = trimmed;

        using var reader = command.ExecuteReader();
        var rows = new List<Dictionary<string, object?>>();
        while (reader.Read() && rows.Count < 200)
        {
            var row = new Dictionary<string, object?>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }
            rows.Add(row);
        }
        return rows;
    }
}
