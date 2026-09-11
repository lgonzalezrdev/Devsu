using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cuentas.Aplicacion.Modelos;
using Cuentas.Api.Serializacion;
using Cuentas.Dominio.Enumeraciones;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Cuentas.Integracion.Pruebas;

public sealed class CuentasApiPruebas : IClassFixture<FabricaCuentasPruebas>
{
    private static readonly JsonSerializerOptions OpcionesSerializacion = CrearOpcionesSerializacion();
    private readonly HttpClient clienteHttp;

    public CuentasApiPruebas(FabricaCuentasPruebas fabricaCuentas)
    {
        clienteHttp = fabricaCuentas.CreateClient();
    }

    [Fact]
    public async Task CrearCuentaParaClienteInexistenteRetornaConflicto()
    {
        CrearCuentaSolicitud solicitud = new()
        {
            ClienteId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            TipoCuenta = TipoCuenta.Ahorros,
            SaldoInicial = 100
        };

        HttpResponseMessage respuesta = await clienteHttp.PostAsJsonAsync("/api/cuentas", solicitud, OpcionesSerializacion);
        ProblemDetails? problema = await respuesta.Content.ReadFromJsonAsync<ProblemDetails>(OpcionesSerializacion);

        Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
        Assert.NotNull(problema);
        Assert.Contains("no existe", problema.Detail!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CrearCuentaParaClienteInactivoRetornaConflicto()
    {
        CrearCuentaSolicitud solicitud = new()
        {
            ClienteId = FabricaCuentasPruebas.ClienteInactivoId,
            TipoCuenta = TipoCuenta.Corriente,
            SaldoInicial = 100
        };

        HttpResponseMessage respuesta = await clienteHttp.PostAsJsonAsync("/api/cuentas", solicitud, OpcionesSerializacion);
        ProblemDetails? problema = await respuesta.Content.ReadFromJsonAsync<ProblemDetails>(OpcionesSerializacion);

        Assert.Equal(HttpStatusCode.Conflict, respuesta.StatusCode);
        Assert.NotNull(problema);
        Assert.Equal("No se puede crear una cuenta para un cliente inactivo.", problema.Detail);
    }

    [Fact]
    public async Task CrearCuentaConSaldoInicialNegativoRetornaSolicitudInvalida()
    {
        CrearCuentaSolicitud solicitud = new()
        {
            ClienteId = FabricaCuentasPruebas.ClienteActivoId,
            TipoCuenta = TipoCuenta.Ahorros,
            SaldoInicial = -1
        };

        HttpResponseMessage respuesta = await clienteHttp.PostAsJsonAsync("/api/cuentas", solicitud, OpcionesSerializacion);

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task JSONMalformadoRetornaErrorEnEspanol()
    {
        StringContent contenido = new("{\"clienteId\":", System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage respuesta = await clienteHttp.PostAsync("/api/cuentas", contenido);
        string detalle = await respuesta.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
        Assert.DoesNotContain("The JSON", detalle, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("The request", detalle, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RegistrarRetiroSinSaldoRetornaSaldoNoDisponible()
    {
        CuentaRespuesta cuenta = await CrearCuentaActivaAsync(50);
        CrearMovimientoSolicitud solicitud = new()
        {
            CuentaId = cuenta.CuentaId,
            TipoMovimiento = TipoMovimiento.Retiro,
            Valor = 51
        };

        HttpResponseMessage respuesta = await clienteHttp.PostAsJsonAsync("/api/movimientos", solicitud, OpcionesSerializacion);
        ProblemDetails? problema = await respuesta.Content.ReadFromJsonAsync<ProblemDetails>(OpcionesSerializacion);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, respuesta.StatusCode);
        Assert.NotNull(problema);
        Assert.Equal("Saldo no disponible", problema.Detail);
    }

    [Fact]
    public async Task ActualizarMovimientoHistoricoRecalculaSaldoDeLaCuenta()
    {
        CuentaRespuesta cuenta = await CrearCuentaActivaAsync(100);
        CrearMovimientoSolicitud solicitudCreacion = new()
        {
            CuentaId = cuenta.CuentaId,
            TipoMovimiento = TipoMovimiento.Deposito,
            Valor = 100
        };
        HttpResponseMessage respuestaCreacion = await clienteHttp.PostAsJsonAsync("/api/movimientos", solicitudCreacion, OpcionesSerializacion);
        MovimientoRespuesta? movimiento = await respuestaCreacion.Content.ReadFromJsonAsync<MovimientoRespuesta>(OpcionesSerializacion);
        ActualizarMovimientoSolicitud solicitudActualizacion = new()
        {
            TipoMovimiento = TipoMovimiento.Retiro,
            Valor = 50
        };

        HttpResponseMessage respuestaActualizacion = await clienteHttp.PutAsJsonAsync($"/api/movimientos/{movimiento!.MovimientoId}", solicitudActualizacion, OpcionesSerializacion);
        HttpResponseMessage respuestaCuenta = await clienteHttp.GetAsync($"/api/cuentas/{cuenta.CuentaId}");
        CuentaRespuesta? cuentaActualizada = await respuestaCuenta.Content.ReadFromJsonAsync<CuentaRespuesta>(OpcionesSerializacion);

        Assert.Equal(HttpStatusCode.Created, respuestaCreacion.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, respuestaActualizacion.StatusCode);
        Assert.Equal(HttpStatusCode.OK, respuestaCuenta.StatusCode);
        Assert.NotNull(cuentaActualizada);
        Assert.Equal(50, cuentaActualizada.SaldoDisponible);
    }

    [Fact]
    public async Task ListadoPaginadoFiltraCuentasYMovimientosPorFecha()
    {
        CuentaRespuesta cuenta = await CrearCuentaActivaAsync(100);
        CrearMovimientoSolicitud deposito = new() { CuentaId = cuenta.CuentaId, TipoMovimiento = TipoMovimiento.Deposito, Valor = 20 };
        await clienteHttp.PostAsJsonAsync("/api/movimientos", deposito, OpcionesSerializacion);

        HttpResponseMessage respuestaCuentas = await clienteHttp.GetAsync($"/api/cuentas?pagina=1&tamanoPagina=1&clienteId={FabricaCuentasPruebas.ClienteActivoId}&estado=true");
        HttpResponseMessage respuestaMovimientos = await clienteHttp.GetAsync($"/api/movimientos?cuentaId={cuenta.CuentaId}&pagina=1&tamanoPagina=1&fechaInicio=2000-01-01&fechaFin=2100-01-01");
        using JsonDocument cuentasJson = JsonDocument.Parse(await respuestaCuentas.Content.ReadAsStringAsync());
        using JsonDocument movimientosJson = JsonDocument.Parse(await respuestaMovimientos.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, respuestaCuentas.StatusCode);
        Assert.Equal(HttpStatusCode.OK, respuestaMovimientos.StatusCode);
        Assert.Equal(1, cuentasJson.RootElement.GetProperty("tamanoPagina").GetInt32());
        Assert.True(cuentasJson.RootElement.GetProperty("totalRegistros").GetInt32() >= 1);
        Assert.Equal(1, movimientosJson.RootElement.GetProperty("totalRegistros").GetInt32());
        Assert.Single(movimientosJson.RootElement.GetProperty("elementos").EnumerateArray());
    }

    private async Task<CuentaRespuesta> CrearCuentaActivaAsync(decimal saldoInicial)
    {
        CrearCuentaSolicitud solicitud = new()
        {
            ClienteId = FabricaCuentasPruebas.ClienteActivoId,
            TipoCuenta = TipoCuenta.Ahorros,
            SaldoInicial = saldoInicial
        };
        HttpResponseMessage respuesta = await clienteHttp.PostAsJsonAsync("/api/cuentas", solicitud, OpcionesSerializacion);
        CuentaRespuesta? cuenta = await respuesta.Content.ReadFromJsonAsync<CuentaRespuesta>(OpcionesSerializacion);

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        Assert.NotNull(cuenta);
        return cuenta;
    }

    private static JsonSerializerOptions CrearOpcionesSerializacion()
    {
        JsonSerializerOptions opciones = new(JsonSerializerDefaults.Web);
        opciones.Converters.Add(new JsonStringEnumConverter());
        opciones.Converters.Add(new ConvertidorFechaHora());
        return opciones;
    }
}
