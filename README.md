# Microservicios Devsu

Solución base para la prueba técnica, desarrollada con .NET 10 y Clean Architecture.

## Servicios

- **Clientes**: gestiona Persona, Cliente y publica sus cambios de integración.
- **Cuentas**: gestiona Cuenta y Movimiento.

Cada servicio conserva su propia base de datos y se integra asíncronamente mediante eventos de RabbitMQ.

La comunicación utiliza el patrón Outbox/Inbox: la modificación del dominio y el evento pendiente se guardan en la misma transacción; un proceso en segundo plano publica los pendientes. Cada consumidor registra el identificador del evento antes de confirmar su efecto, por lo que una reentrega no duplica proyecciones ni movimientos.

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

La solución usa SQL Server LocalDB por defecto. Ejecuta [BaseDatos.sql](database/BaseDatos.sql) una sola vez, sobre una instancia nueva, desde SQL Server Management Studio para crear el esquema completo. No se usan migraciones ni creación automática del esquema; el archivo contiene todas las tablas, restricciones y proyecciones requeridas.

La tabla `cuentas.Cuentas` incluye la columna `VersionFila` de SQL Server (`rowversion`) para concurrencia optimista. Si dos operaciones modifican el mismo saldo simultáneamente, una se confirma y la otra recibe HTTP 409 con un mensaje para consultar el saldo e intentar de nuevo.

También puedes ejecutar [BaseDatos.sql](database/BaseDatos.sql) desde SQL Server Management Studio. Si usas otra instancia, actualiza las cadenas `Clientes` y `Cuentas` en los archivos `appsettings.json` de las APIs.

## Comunicación asíncrona: Clientes → Cuentas

El microservicio Clientes publica los eventos `ClienteSincronizado` y `ClienteEliminado`. Cuentas los consume mediante RabbitMQ y conserva únicamente una proyección local (`cuentas.ClientesIntegracion`) con identificador, nombre, estado y fecha de actualización. Por tanto, Cuentas no consulta la base de datos de Clientes ni usa comunicación HTTP síncrona para crear una cuenta.

Para probarla localmente, inicia RabbitMQ:

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

Para cargar cuentas existentes al activar esta funcionalidad, ejecuta `POST /api/cuentas/sincronizar` y espera unos segundos antes de consultar el reporte.

## Postman

Importa [Devsu.Clientes.postman_collection.json](postman/Devsu.Clientes.postman_collection.json) en Postman y ejecuta las solicitudes en el orden mostrado. Las colecciones se entregan configuradas para Docker con `http://{{servidor}}:{{puerto}}`; ajusta protocolo, `servidor` y `puerto` si las ejecutas desde Visual Studio.

La colección [Devsu.Cuentas.postman_collection.json](postman/Devsu.Cuentas.postman_collection.json) valida cuentas y movimientos. Con la mensajería activa, primero crea o sincroniza el cliente y espera unos segundos antes de usar su `clienteId` para crear una cuenta.

La colección [Devsu.Reportes.postman_collection.json](postman/Devsu.Reportes.postman_collection.json) valida el endpoint de estado de cuenta.

## Pruebas automatizadas

Ejecuta todas las pruebas desde la raíz de la solución:

```powershell
dotnet test Devsu.Microservicios.slnx --configuration Release
```

La prueba unitaria valida que la entidad de dominio `Cliente` rechace una identificación inválida. Las pruebas de integración usan `WebApplicationFactory` y SQLite en memoria, por lo que no dependen ni alteran SQL Server LocalDB. Cubren creación y actualización de cliente, identificación duplicada, género inválido, cliente inexistente o inactivo al crear una cuenta, saldo inicial negativo y retiro sin saldo disponible.

## Health checks

Cada API expone dos comprobaciones de salud en JSON:

- `GET /vivo`: liveness. Solo verifica que el proceso web esté ejecutándose; es apropiado para detectar una API detenida o bloqueada.
- `GET /salud`: readiness. Comprueba una conexión real a SQL Server y una conexión TCP a RabbitMQ. Devuelve HTTP 200 solo si las dependencias requeridas están disponibles; ante un fallo devuelve HTTP 503 con el nombre y detalle de la dependencia afectada.

Cuando `Mensajeria:Activa` es `false`, RabbitMQ aparece sano con el detalle de que está desactivado, permitiendo ejecutar las APIs locales sin el broker. Cuando está activa, la verificación intenta conectar al host y puerto configurados. MassTransit conserva además su propia conexión con RabbitMQ para publicar y consumir eventos.

## Despliegue completo con Docker

El archivo [docker-compose.yml](docker-compose.yml) levanta SQL Server, RabbitMQ y ambas APIs. SQL Server conserva sus datos en un volumen de Docker e inicializa [BaseDatos.sql](database/BaseDatos.sql) una sola vez antes de declararse saludable.

1. Si antes iniciaste únicamente RabbitMQ, detenlo para liberar los puertos:

```powershell
docker compose -f docker-compose.mensajeria.yml down
```

2. Copia el archivo de variables y reemplaza ambas claves por valores seguros. No publiques el archivo `.env`.

```powershell
Copy-Item .env.example .env
```

3. Construye e inicia toda la solución:

```powershell
docker compose up --build -d
docker compose ps
```

4. Comprueba los servicios:

```powershell
Invoke-WebRequest http://localhost:8081/salud
Invoke-WebRequest http://localhost:8082/salud
```

- Clientes API: `http://localhost:8081`
- Cuentas API: `http://localhost:8082`
- RabbitMQ Management: `http://localhost:15672` con usuario `devsu` y la clave `RABBITMQ_PASSWORD`.
- SQL Server: `localhost,14333`, usuario `sa` y la clave `SQL_SERVER_SA_PASSWORD`.

El perfil Docker expone HTTP para no almacenar certificados de desarrollo en el repositorio. Las ejecuciones locales desde Visual Studio continúan usando HTTPS con los puertos definidos en `launchSettings.json`.

Para diagnosticar un servicio, consulta sus registros, por ejemplo:

```powershell
docker compose logs sqlserver
docker compose logs cuentas-api
```

Para detener los contenedores sin borrar los datos:

```powershell
docker compose down
```

Para borrar también la base de datos persistida en Docker —acción irreversible—:

```powershell
docker compose down -v
```
