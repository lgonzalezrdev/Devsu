using Cuentas.Aplicacion;
using Cuentas.Infraestructura;

WebApplicationBuilder constructor = WebApplication.CreateBuilder(args);

constructor.Logging.ClearProviders();
constructor.Logging.AddConsole();
constructor.Services.AddControllers();
constructor.Services.AddHealthChecks();
constructor.Services.AddProblemDetails();
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
