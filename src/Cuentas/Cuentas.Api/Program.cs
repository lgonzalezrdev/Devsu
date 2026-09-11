using Cuentas.Aplicacion;
using Cuentas.Api.Errores;
using Cuentas.Api.Serializacion;
using Cuentas.Infraestructura;
using Cuentas.Dominio.Enumeraciones;
using Cuentas.Infraestructura.Salud;
using Observabilidad.Compartida.Salud;
using Observabilidad.Compartida.Validacion;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

WebApplicationBuilder constructor = WebApplication.CreateBuilder(args);

constructor.Logging.ClearProviders();
constructor.Logging.AddConsole();
constructor.Services.AddControllers(opciones => ConfiguracionValidacionesApi.ConfigurarMensajesModelBinding(opciones)).AddJsonOptions(opcionesJson =>
{
    opcionesJson.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter<TipoCuenta>(allowIntegerValues: false));
    opcionesJson.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter<TipoMovimiento>(allowIntegerValues: false));
    opcionesJson.JsonSerializerOptions.Converters.Add(new ConvertidorFechaHora());
});
constructor.Services.Configure<ApiBehaviorOptions>(opciones =>
    opciones.InvalidModelStateResponseFactory = ConfiguracionValidacionesApi.CrearRespuestaError);
IHealthChecksBuilder comprobacionesSalud = constructor.Services.AddHealthChecks();
comprobacionesSalud.AddCheck("api", () => HealthCheckResult.Healthy("La API de Cuentas está en ejecución."), tags: ["vivo"]);
comprobacionesSalud.AddCheck<ComprobacionSqlServer>("sqlserver", tags: ["listo"]);
comprobacionesSalud.AddCheck<ComprobacionRabbitMq>("rabbitmq", tags: ["listo"]);
constructor.Services.AddProblemDetails();
constructor.Services.AddExceptionHandler<ManejadorExcepciones>();
constructor.Services.AgregarServiciosAplicacionCuentas();
constructor.Services.AgregarServiciosInfraestructuraCuentas(constructor.Configuration);

WebApplication aplicacion = constructor.Build();

aplicacion.UseExceptionHandler();

bool redireccionHttpsActiva = constructor.Configuration.GetValue("RedireccionHttps:Activa", true);

if (!aplicacion.Environment.IsDevelopment() && redireccionHttpsActiva)
{
    aplicacion.UseHttpsRedirection();
}

aplicacion.MapControllers();
aplicacion.MapHealthChecks("/vivo", new HealthCheckOptions
{
    Predicate = comprobacion => comprobacion.Tags.Contains("vivo"),
    ResponseWriter = RespuestaSalud.EscribirAsync
});
aplicacion.MapHealthChecks("/salud", new HealthCheckOptions
{
    Predicate = comprobacion => comprobacion.Tags.Contains("listo"),
    ResponseWriter = RespuestaSalud.EscribirAsync
});

aplicacion.Run();

public partial class Program;
