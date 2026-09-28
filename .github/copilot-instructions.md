# Instrucciones del repositorio para GitHub Copilot

## Contexto
API de pedidos en .NET 8 con ASP.NET Core Minimal APIs. Persistencia en SQLite con Microsoft.Data.Sqlite (`Data/Database.cs`).
La lógica de negocio vive en `Services/`; `Program.cs` solo define endpoints y traduce excepciones a códigos HTTP.

## Convenciones
- Nombres de código en inglés; comentarios XML, mensajes de error y textos al usuario en español.
- Nullable reference types activados. Modelos como `record`.
- Errores de dominio como excepciones propias en `Exceptions/`; `Program.cs` las mapea a HTTP (404, 409…).
- SQL siempre con parámetros (`$nombre` + `Parameters.AddWithValue`). Nunca interpolación ni concatenación.
- Conexiones, comandos y readers siempre con `using`.
- Nada de secretos en el código: se leen de configuración o variables de entorno.

## Tests
- xUnit en `tests/Pedidos.Tests`. Endpoints con `PedidosApiFactory` (WebApplicationFactory con BD temporal).
- Toda funcionalidad nueva incluye al menos un test del caso feliz y uno del caso de error.

## Revisión de pull requests
Al revisar, prioriza en este orden: seguridad (inyección SQL, secretos), errores no controlados y recursos sin liberar,
ausencia de tests y, por último, estilo. Explica el riesgo y propone la corrección concreta.
