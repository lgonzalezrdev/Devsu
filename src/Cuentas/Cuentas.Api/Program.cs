using Cuentas.Aplicacion;
using Cuentas.Api.Errores;
using Cuentas.Api.Serializacion;
using Cuentas.Infraestructura;
using Cuentas.Dominio.Enumeraciones;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

WebApplicationBuilder constructor = WebApplication.CreateBuilder(args);

constructor.Logging.ClearProviders();
constructor.Logging.AddConsole();
constructor.Services.AddControllers().AddJsonOptions(opcionesJson =>
{
    opcionesJson.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter<TipoCuenta>(allowIntegerValues: false));
    opcionesJson.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter<TipoMovimiento>(allowIntegerValues: false));
    opcionesJson.JsonSerializerOptions.Converters.Add(new ConvertidorFechaHora());
});
constructor.Services.Configure<ApiBehaviorOptions>(opciones =>
{
    opciones.InvalidModelStateResponseFactory = contextoAccion =>
    {
        if (contextoAccion.ModelState.ContainsKey("$.clienteId"))
        {
            ProblemDetails problema = new()
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Identificador de cliente inválido",
                Detail = "El campo clienteId debe tener un GUID válido.",
                Extensions = { ["idTrazabilidad"] = contextoAccion.HttpContext.TraceIdentifier }
            };

            return new BadRequestObjectResult(problema);
        }

        ValidationProblemDetails problemaValidacion = new(contextoAccion.ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Los datos de la solicitud no son válidos.",
            Extensions = { ["idTrazabilidad"] = contextoAccion.HttpContext.TraceIdentifier }
        };

        return new BadRequestObjectResult(problemaValidacion);
    };
});
constructor.Services.AddHealthChecks();
constructor.Services.AddProblemDetails();
constructor.Services.AddExceptionHandler<ManejadorExcepciones>();
constructor.Services.AgregarServiciosAplicacionCuentas();
constructor.Services.AgregarServiciosInfraestructuraCuentas(constructor.Configuration);

WebApplication aplicacion = constructor.Build();

aplicacion.UseExceptionHandler();

if (!aplicacion.Environment.IsDevelopment())
{
    aplicacion.UseHttpsRedirection();
}

aplicacion.MapControllers();
aplicacion.MapHealthChecks("/salud");

aplicacion.Run();

public partial class Program;
