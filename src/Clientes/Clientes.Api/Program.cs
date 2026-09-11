using Clientes.Aplicacion;
using Clientes.Api.Errores;
using Clientes.Api.Serializacion;
using Clientes.Infraestructura;
using Clientes.Dominio.Enumeraciones;
using Clientes.Infraestructura.Salud;
using Observabilidad.Compartida.Salud;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json.Serialization;

WebApplicationBuilder constructor = WebApplication.CreateBuilder(args);

constructor.Logging.ClearProviders();
constructor.Logging.AddConsole();
constructor.Services.AddControllers().AddJsonOptions(opciones =>
    opciones.JsonSerializerOptions.Converters.Add(new ConvertidorGenero()));
IHealthChecksBuilder comprobacionesSalud = constructor.Services.AddHealthChecks();
comprobacionesSalud.AddCheck("api", () => HealthCheckResult.Healthy("La API de Clientes está en ejecución."), tags: ["vivo"]);
comprobacionesSalud.AddCheck<ComprobacionSqlServer>("sqlserver", tags: ["listo"]);
comprobacionesSalud.AddCheck<ComprobacionRabbitMq>("rabbitmq", tags: ["listo"]);
constructor.Services.AddProblemDetails();
constructor.Services.AddExceptionHandler<ManejadorExcepciones>();
constructor.Services.AgregarServiciosAplicacionClientes();
constructor.Services.AgregarServiciosInfraestructuraClientes(constructor.Configuration);

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
