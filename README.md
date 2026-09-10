# Microservicios Devsu

Solución base para la prueba técnica, desarrollada con .NET 10 y Clean Architecture.

## Servicios

- **Clientes**: gestiona Persona, Cliente y la consulta de reportes.
- **Cuentas**: gestiona Cuenta y Movimiento.

Cada servicio conserva su propia base de datos y se integrará asíncronamente mediante eventos de RabbitMQ.

## Inyección de dependencias

La configuración de dependencias se concentra en métodos de extensión por capa:

- `AgregarServiciosAplicacionClientes` y `AgregarServiciosAplicacionCuentas`.
- `AgregarServiciosInfraestructuraClientes` y `AgregarServiciosInfraestructuraCuentas`.

Esto mantiene las APIs libres de dependencias concretas. La infraestructura implementará interfaces definidas por el dominio o la aplicación; las APIs solo compondrán dichas capas.

## Estructura

```text
src/
  Clientes/      # API, Aplicación, Dominio e Infraestructura
  Cuentas/       # API, Aplicación, Dominio e Infraestructura
  Construccion/  # Contratos de integración y observabilidad compartida
tests/           # Pruebas de dominio e integración
database/        # BaseDatos.sql
postman/         # Colección de validación
```

## Base de datos local

La solución usa SQL Server LocalDB por defecto. Ejecuta el script `BaseDatos.sql` desde SQL Server Management Studio para crear el esquema inicial. No se usan migraciones ni creación automática del esquema: cualquier cambio futuro de base de datos se entregará como un script SQL incremental y también se incorporará a `BaseDatos.sql`.

Si ya creaste las bases antes de las validaciones de Clientes, ejecuta también [001_validaciones_clientes.sql](database/actualizaciones/001_validaciones_clientes.sql).

También puedes ejecutar [BaseDatos.sql](database/BaseDatos.sql) desde SQL Server Management Studio. Si usas otra instancia, actualiza las cadenas `Clientes` y `Cuentas` en los archivos `appsettings.json` de las APIs.

## Estado actual

La solución incluye modelos de dominio, contextos EF Core, script inicial de SQL Server y CRUD de Clientes. RabbitMQ, Cuentas, Movimientos, reportes, Docker y las pruebas se incorporarán en los siguientes incrementos.

## Postman

Importa [Devsu.Clientes.postman_collection.json](postman/Devsu.Clientes.postman_collection.json) en Postman y ejecuta las solicitudes en el orden mostrado. La colección usa `https://{{servidor}}:{{puerto}}`; ajusta las variables `servidor` y `puerto` según el perfil de inicio de Visual Studio.

La colección [Devsu.Cuentas.postman_collection.json](postman/Devsu.Cuentas.postman_collection.json) valida cuentas y movimientos. Antes de ejecutarla, asigna a `clienteId` el identificador de un cliente existente.
