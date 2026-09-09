using Cuentas.Aplicacion;
using Cuentas.Infraestructura;
using Cuentas.Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;

var constructor = WebApplication.CreateBuilder(args);

constructor.Services.AddControllers();
constructor.Services.AddHealthChecks();
constructor.Services.AddProblemDetails();
constructor.Services.AgregarServiciosAplicacionCuentas();
constructor.Services.AgregarServiciosInfraestructuraCuentas(constructor.Configuration);

var aplicacion = constructor.Build();

if (constructor.Configuration.GetValue<bool>("BaseDatos:InicializarAlArrancar"))
{
    await using var alcance = aplicacion.Services.CreateAsyncScope();
    var contexto = alcance.ServiceProvider.GetRequiredService<ContextoCuentas>();
    await contexto.Database.EnsureCreatedAsync();
}

aplicacion.UseExceptionHandler();
aplicacion.UseHttpsRedirection();
aplicacion.MapControllers();
aplicacion.MapHealthChecks("/salud");

aplicacion.Run();

public partial class Program;
