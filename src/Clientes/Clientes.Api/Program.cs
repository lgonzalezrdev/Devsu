using Clientes.Aplicacion;
using Clientes.Api.Errores;
using Clientes.Api.Serializacion;
using Clientes.Infraestructura;
using Clientes.Dominio.Enumeraciones;
using System.Text.Json.Serialization;

WebApplicationBuilder constructor = WebApplication.CreateBuilder(args);

constructor.Logging.ClearProviders();
constructor.Logging.AddConsole();
constructor.Services.AddControllers().AddJsonOptions(opciones =>
    opciones.JsonSerializerOptions.Converters.Add(new ConvertidorGenero()));
constructor.Services.AddHealthChecks();
constructor.Services.AddProblemDetails();
constructor.Services.AddExceptionHandler<ManejadorExcepciones>();
constructor.Services.AgregarServiciosAplicacionClientes();
constructor.Services.AgregarServiciosInfraestructuraClientes(constructor.Configuration);

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
