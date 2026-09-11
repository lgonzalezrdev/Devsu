using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using Clientes.Aplicacion.Modelos;
using Clientes.Dominio.Enumeraciones;
using Clientes.Dominio.Entidades;
using Clientes.Infraestructura.Persistencia;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Clientes.Integracion.Pruebas;

public sealed class ClientesApiPruebas : IClassFixture<FabricaClientesPruebas>
{
    private static readonly JsonSerializerOptions OpcionesSerializacion = CrearOpcionesSerializacion();
    private readonly HttpClient clienteHttp;
    private readonly FabricaClientesPruebas fabricaClientes;

    public ClientesApiPruebas(FabricaClientesPruebas fabricaClientes)
    {
        this.fabricaClientes = fabricaClientes;
        clienteHttp = fabricaClientes.CreateClient();
    }

    [Fact]
    public async Task ReporteFiltraMovimientosPorRangoYConservaCuentaSinMovimientos()
    {
        ClienteRespuesta cliente = await CrearClienteParaReporteAsync("7890123456", "Cliente Reporte");
        Guid cuentaConMovimientosId = Guid.NewGuid();
        Guid cuentaSinMovimientosId = Guid.NewGuid();
        DateTime fechaDentroRango = new(2026, 9, 10, 10, 0, 0);

        using IServiceScope alcance = fabricaClientes.CrearAlcance();
        ContextoClientes contextoClientes = alcance.ServiceProvider.GetRequiredService<ContextoClientes>();
        contextoClientes.CuentasReporte.AddRange(
            new CuentaReporte(cuentaConMovimientosId, cliente.ClienteId, "111111", "Ahorros", 100, 150, true, fechaDentroRango),
            new CuentaReporte(cuentaSinMovimientosId, cliente.ClienteId, "222222", "Corriente", 200, 200, true, fechaDentroRango));
        contextoClientes.MovimientosReporte.AddRange(
            new MovimientoReporte(Guid.NewGuid(), cuentaConMovimientosId, fechaDentroRango, "Deposito", 50, 150),
            new MovimientoReporte(Guid.NewGuid(), cuentaConMovimientosId, fechaDentroRango.AddDays(-2), "Deposito", 30, 130));
        await contextoClientes.SaveChangesAsync();

        HttpResponseMessage respuesta = await clienteHttp.GetAsync($"/api/reportes?cliente={cliente.ClienteId}&fecha=2026-09-10%2000:00:00,2026-09-10%2023:59:59");
        string contenido = await respuesta.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.Contains("111111", contenido, StringComparison.Ordinal);
        Assert.Contains("222222", contenido, StringComparison.Ordinal);
        Assert.Contains("Deposito", contenido, StringComparison.Ordinal);
        Assert.DoesNotContain("130", contenido, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CrearClienteConDatosValidosRetornaCreado()
    {
        CrearClienteSolicitud solicitud = new()
        {
            Nombre = "Marianela Montalvo",
            Genero = Genero.Femenino,
            Edad = 32,
            Identificacion = "1234567890",
            Direccion = "Amazonas y NNUU",
            Telefono = "097548965",
            Contrasena = "5678"
        };

        HttpResponseMessage respuesta = await clienteHttp.PostAsJsonAsync("/api/clientes", solicitud, OpcionesSerializacion);
        ClienteRespuesta? clienteCreado = await respuesta.Content.ReadFromJsonAsync<ClienteRespuesta>(OpcionesSerializacion);

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        Assert.NotNull(clienteCreado);
        Assert.Equal("Marianela Montalvo", clienteCreado.Nombre);
        Assert.True(clienteCreado.Estado);
        Assert.NotEqual(Guid.Empty, clienteCreado.ClienteId);
    }

    [Fact]
    public async Task CrearClienteConIdentificacionRepetidaRetornaConflicto()
    {
        CrearClienteSolicitud solicitud = CrearSolicitud("4567890123", "Juan Osorio");

        HttpResponseMessage primeraRespuesta = await clienteHttp.PostAsJsonAsync("/api/clientes", solicitud, OpcionesSerializacion);
        HttpResponseMessage segundaRespuesta = await clienteHttp.PostAsJsonAsync("/api/clientes", solicitud, OpcionesSerializacion);

        Assert.Equal(HttpStatusCode.Created, primeraRespuesta.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, segundaRespuesta.StatusCode);
    }

    [Fact]
    public async Task CrearClienteConGeneroInexistenteRetornaSolicitudInvalida()
    {
        string contenidoJson = "{\"nombre\":\"Cliente Prueba\",\"genero\":\"Inexistente\",\"edad\":30,\"identificacion\":\"5678901234\",\"direccion\":\"Calle principal 123\",\"telefono\":\"0987654321\",\"contrasena\":\"1234\"}";
        StringContent contenido = new(contenidoJson, Encoding.UTF8, "application/json");

        HttpResponseMessage respuesta = await clienteHttp.PostAsync("/api/clientes", contenido);
        string detalle = await respuesta.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
        Assert.Contains("género", detalle, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task JSONMalformadoRetornaErrorEnEspanol()
    {
        StringContent contenido = new("{\"nombre\":", Encoding.UTF8, "application/json");

        HttpResponseMessage respuesta = await clienteHttp.PostAsync("/api/clientes", contenido);
        string detalle = await respuesta.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
        Assert.DoesNotContain("The JSON", detalle, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("The request", detalle, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ActualizarClienteConDatosValidosPersisteCambios()
    {
        CrearClienteSolicitud solicitudCreacion = CrearSolicitud("6789012345", "Maria Inicial");
        HttpResponseMessage respuestaCreacion = await clienteHttp.PostAsJsonAsync("/api/clientes", solicitudCreacion, OpcionesSerializacion);
        ClienteRespuesta? clienteCreado = await respuestaCreacion.Content.ReadFromJsonAsync<ClienteRespuesta>(OpcionesSerializacion);
        ActualizarClienteSolicitud solicitudActualizacion = new()
        {
            Nombre = "Maria Actualizada",
            Genero = Genero.Femenino,
            Edad = 34,
            Identificacion = "6789012345",
            Direccion = "Avenida actualizada 456",
            Telefono = "0991234567",
            Contrasena = null
        };

        HttpResponseMessage respuestaActualizacion = await clienteHttp.PutAsJsonAsync(
            $"/api/clientes/{clienteCreado!.ClienteId}",
            solicitudActualizacion,
            OpcionesSerializacion);
        HttpResponseMessage respuestaConsulta = await clienteHttp.GetAsync($"/api/clientes/{clienteCreado.ClienteId}");
        ClienteRespuesta? clienteActualizado = await respuestaConsulta.Content.ReadFromJsonAsync<ClienteRespuesta>(OpcionesSerializacion);

        Assert.Equal(HttpStatusCode.Created, respuestaCreacion.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, respuestaActualizacion.StatusCode);
        Assert.Equal(HttpStatusCode.OK, respuestaConsulta.StatusCode);
        Assert.NotNull(clienteActualizado);
        Assert.Equal("Maria Actualizada", clienteActualizado.Nombre);
        Assert.Equal(34, clienteActualizado.Edad);
    }

    [Fact]
    public async Task EliminarClienteRealizaBajaLogicaYConservaElRegistro()
    {
        ClienteRespuesta cliente = await CrearClienteParaReporteAsync("8901234567", "Cliente Baja Logica");

        HttpResponseMessage respuestaEliminar = await clienteHttp.DeleteAsync($"/api/clientes/{cliente.ClienteId}");
        HttpResponseMessage respuestaConsulta = await clienteHttp.GetAsync($"/api/clientes/{cliente.ClienteId}");
        ClienteRespuesta? clienteInactivo = await respuestaConsulta.Content.ReadFromJsonAsync<ClienteRespuesta>(OpcionesSerializacion);

        Assert.Equal(HttpStatusCode.NoContent, respuestaEliminar.StatusCode);
        Assert.Equal(HttpStatusCode.OK, respuestaConsulta.StatusCode);
        Assert.NotNull(clienteInactivo);
        Assert.False(clienteInactivo.Estado);
    }

    private static CrearClienteSolicitud CrearSolicitud(string identificacion, string nombre) => new()
    {
        Nombre = nombre,
        Genero = Genero.Masculino,
        Edad = 30,
        Identificacion = identificacion,
        Direccion = "Direccion de prueba 123",
        Telefono = "0987654321",
        Contrasena = "1234"
    };

    private async Task<ClienteRespuesta> CrearClienteParaReporteAsync(string identificacion, string nombre)
    {
        HttpResponseMessage respuesta = await clienteHttp.PostAsJsonAsync("/api/clientes", CrearSolicitud(identificacion, nombre), OpcionesSerializacion);
        ClienteRespuesta? cliente = await respuesta.Content.ReadFromJsonAsync<ClienteRespuesta>(OpcionesSerializacion);
        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        Assert.NotNull(cliente);
        return cliente;
    }

    private static JsonSerializerOptions CrearOpcionesSerializacion()
    {
        JsonSerializerOptions opciones = new(JsonSerializerDefaults.Web);
        opciones.Converters.Add(new JsonStringEnumConverter());
        return opciones;
    }
}
