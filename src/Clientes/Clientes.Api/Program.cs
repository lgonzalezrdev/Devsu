using Clientes.Aplicacion;
using Clientes.Infraestructura;

var constructor = WebApplication.CreateBuilder(args);

constructor.Services.AddControllers();
constructor.Services.AddHealthChecks();
constructor.Services.AgregarServiciosAplicacionClientes();
constructor.Services.AgregarServiciosInfraestructuraClientes(constructor.Configuration);

var aplicacion = constructor.Build();

aplicacion.UseExceptionHandler();
aplicacion.UseHttpsRedirection();
aplicacion.MapControllers();
aplicacion.MapHealthChecks("/salud");

aplicacion.Run();

public partial class Program;
