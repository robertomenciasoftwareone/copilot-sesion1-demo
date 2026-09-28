# Repo demo · Sesión 1 GitHub Copilot (.NET)

API de pedidos mínima en **.NET 8 · ASP.NET Core Minimal APIs · SQLite · xUnit**, más un servidor MCP en C#.

## Puesta en marcha (hacerlo la víspera)
```bash
dotnet restore
dotnet build
dotnet test                                   # 4 tests en verde
dotnet run --project src/Pedidos.Api          # crea data/pedidos.db con 60 pedidos · http://localhost:5xxx/orders
```
> Si tenéis .NET 9 o 10 instalado y no el 8, cambiad `net8.0` por vuestra versión en los tres `.csproj`.
> Tras el primer restore, fijad la versión exacta del paquete `ModelContextProtocol` en `src/Pedidos.Mcp/Pedidos.Mcp.csproj`.

## Qué hay para cada demo
| Demo | Material |
|---|---|
| 1 · Editar y entender | `src/Pedidos.Api/Legacy/PricingCalculator.cs` (código heredado) |
| 2 · Ask → Plan → Agent | Falta el endpoint de cancelación de pedidos |
| 3 · Revisión de PR | `demo-assets/CouponService_ConFallos.cs` + `.github/copilot-instructions.md` |
| 4 · MCP | `.vscode/mcp.json` + `src/Pedidos.Mcp` (SQLite en solo lectura) |
| 5 · Agentes | `.github/agents/*.agent.md` |
| 6 · Arquitectura y docs | Agente `Arquitecto` → `docs/` |

Sube el repo a GitHub (privado) antes de la sesión: la demo 3 y el servidor MCP de GitHub lo necesitan.

