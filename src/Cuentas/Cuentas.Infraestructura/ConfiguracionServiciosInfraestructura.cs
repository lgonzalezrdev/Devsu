using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Cuentas.Infraestructura.Persistencia;

namespace Cuentas.Infraestructura;

public static class ConfiguracionServiciosInfraestructura
{
    public static IServiceCollection AgregarServiciosInfraestructuraCuentas(
        this IServiceCollection servicios,
        IConfiguration configuracion)
    {
        var cadenaConexion = configuracion.GetConnectionString("Cuentas")
            ?? throw new InvalidOperationException("No se configuró la cadena de conexión 'Cuentas'.");

        servicios.AddDbContext<ContextoCuentas>(opciones =>
            opciones.UseSqlServer(cadenaConexion, sqlServer => sqlServer.EnableRetryOnFailure()));

        // Aquí se registrarán repositorios, publicadores y consumidores de eventos.
        return servicios;
    }
}
