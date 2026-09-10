# Microservicios Devsu

Solución base para la prueba técnica, desarrollada con .NET 10 y Clean Architecture.

## Servicios

- **Clientes**: gestiona Persona, Cliente y publica sus cambios de integración.
- **Cuentas**: gestiona Cuenta y Movimiento.

Cada servicio conserva su propia base de datos y se integra asíncronamente mediante eventos de RabbitMQ.

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

## Comunicación asíncrona: Clientes → Cuentas

El microservicio Clientes publica los eventos `ClienteSincronizado` y `ClienteEliminado`. Cuentas los consume mediante RabbitMQ y conserva únicamente una proyección local (`cuentas.ClientesIntegracion`) con identificador, nombre, estado y fecha de actualización. Por tanto, Cuentas no consulta la base de datos de Clientes ni usa comunicación HTTP síncrona para crear una cuenta.

Antes de activarla en una base existente, ejecuta [004_proyeccion_clientes_integracion.sql](database/actualizaciones/004_proyeccion_clientes_integracion.sql). Luego inicia RabbitMQ:

```powershell
docker compose -f docker-compose.mensajeria.yml up -d
```

En ambos `appsettings.json`, cambia `Mensajeria:Activa` a `true`, inicia primero Cuentas y después Clientes. Finalmente llama a `POST /api/clientes/sincronizar` una vez para publicar los clientes que existían antes de activar la mensajería. A partir de entonces, cada alta, modificación, cambio de estado o eliminación se sincroniza de forma asíncrona.

Mientras `Mensajeria:Activa` sea `false` (valor predeterminado), Clientes mantiene su ejecución local sin RabbitMQ. En ese modo no se sincronizan clientes y Cuentas rechazará nuevas cuentas porque no puede validar de forma distribuida su propietario.

## Reporte de estado de cuenta

El reporte se consulta en Clientes con el siguiente formato, donde `cliente` es el identificador GUID y `fecha` es un rango inclusivo:

```text
GET /api/reportes?cliente={clienteId}&fecha=yyyy-MM-dd%20HH:mm:ss,yyyy-MM-dd%20HH:mm:ss
```

Cuentas publica una instantánea de la cuenta y de todos sus movimientos después de cada creación o modificación. Clientes la consume en las tablas de proyección `CuentasReporte` y `MovimientosReporte`; por ello el reporte no realiza peticiones HTTP a Cuentas ni accede a `DevsuCuentas`.

Para cargar cuentas existentes al activar esta funcionalidad, ejecuta `POST /api/cuentas/sincronizar` y espera unos segundos antes de consultar el reporte. Requiere haber ejecutado [005_proyeccion_reportes.sql](database/actualizaciones/005_proyeccion_reportes.sql).

## Postman

Importa [Devsu.Clientes.postman_collection.json](postman/Devsu.Clientes.postman_collection.json) en Postman y ejecuta las solicitudes en el orden mostrado. La colección usa `https://{{servidor}}:{{puerto}}`; ajusta las variables `servidor` y `puerto` según el perfil de inicio de Visual Studio.

La colección [Devsu.Cuentas.postman_collection.json](postman/Devsu.Cuentas.postman_collection.json) valida cuentas y movimientos. Con la mensajería activa, primero crea o sincroniza el cliente y espera unos segundos antes de usar su `clienteId` para crear una cuenta.

La colección [Devsu.Reportes.postman_collection.json](postman/Devsu.Reportes.postman_collection.json) valida el endpoint de estado de cuenta.
