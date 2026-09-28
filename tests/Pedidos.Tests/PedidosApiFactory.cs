using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Pedidos.Tests;

/// <summary>Levanta la API con una base de datos temporal y propia para cada test.</summary>
public sealed class PedidosApiFactory : WebApplicationFactory<Program>
{
    public string DbPath { get; } = Path.Combine(Path.GetTempPath(), $"pedidos-test-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.UseSetting("Pedidos:DbPath", DbPath);
}
