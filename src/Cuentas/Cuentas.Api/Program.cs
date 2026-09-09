using Cuentas.Aplicacion;
using Cuentas.Infraestructura;

var constructor = WebApplication.CreateBuilder(args);

constructor.Services.AddControllers();
constructor.Services.AddHealthChecks();
constructor.Services.AgregarServiciosAplicacionCuentas();
constructor.Services.AgregarServiciosInfraestructuraCuentas(constructor.Configuration);

var aplicacion = constructor.Build();

aplicacion.UseExceptionHandler();
aplicacion.UseHttpsRedirection();
aplicacion.MapControllers();
aplicacion.MapHealthChecks("/salud");

aplicacion.Run();

public partial class Program;
