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

## Estado actual

La solución contiene el esqueleto de proyectos, referencias entre capas y puntos de composición de dependencias. Los endpoints de negocio, EF Core, RabbitMQ, Docker y las pruebas se incorporarán en los siguientes incrementos.
