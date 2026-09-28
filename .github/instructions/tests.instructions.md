---
applyTo: "tests/**/*.cs"
---
- Nombra los tests como `Metodo_Escenario_ResultadoEsperado`.
- Patrón Arrange / Act / Assert, separado por líneas en blanco.
- Cada test crea su propia `PedidosApiFactory`: nunca compartas base de datos entre tests.
