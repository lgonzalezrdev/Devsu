using Clientes.Aplicacion;
using Clientes.Infraestructura;
using Clientes.Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;

var constructor = WebApplication.CreateBuilder(args);

constructor.Services.AddControllers();
constructor.Services.AddHealthChecks();
constructor.Services.AddProblemDetails();
constructor.Services.AgregarServiciosAplicacionClientes();
constructor.Services.AgregarServiciosInfraestructuraClientes(constructor.Configuration);

var aplicacion = constructor.Build();

if (constructor.Configuration.GetValue<bool>("BaseDatos:InicializarAlArrancar"))
{
    await using var alcance = aplicacion.Services.CreateAsyncScope();
    var contexto = alcance.ServiceProvider.GetRequiredService<ContextoClientes>();
    await contexto.Database.EnsureCreatedAsync();
}

aplicacion.UseExceptionHandler();
aplicacion.UseHttpsRedirection();
aplicacion.MapControllers();
aplicacion.MapHealthChecks("/salud");

aplicacion.Run();

public partial class Program;
