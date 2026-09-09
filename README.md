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

La solución usa SQL Server LocalDB por defecto. Ejecuta el script `BaseDatos.sql` desde SQL Server Management Studio para crear el esquema inicial. Si tu instancia local está disponible y deseas que EF Core cree las bases al arrancar, cambia `BaseDatos:InicializarAlArrancar` a `true` en ambos `appsettings.json`. Más adelante se reemplazará `EnsureCreated` por migraciones de EF Core durante el despliegue Docker.

También puedes ejecutar [BaseDatos.sql](database/BaseDatos.sql) desde SQL Server Management Studio. Si usas otra instancia, actualiza las cadenas `Clientes` y `Cuentas` en los archivos `appsettings.json` de las APIs.

## Estado actual

La solución incluye modelos de dominio, contextos EF Core y el script inicial de SQL Server. Los endpoints de negocio, repositorios, RabbitMQ, Docker y las pruebas se incorporarán en los siguientes incrementos.
