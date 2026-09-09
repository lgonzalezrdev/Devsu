using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Clientes.Infraestructura.Persistencia;

namespace Clientes.Infraestructura;

public static class ConfiguracionServiciosInfraestructura
{
    public static IServiceCollection AgregarServiciosInfraestructuraClientes(
        this IServiceCollection servicios,
        IConfiguration configuracion)
    {
        var cadenaConexion = configuracion.GetConnectionString("Clientes")
            ?? throw new InvalidOperationException("No se configuró la cadena de conexión 'Clientes'.");

        servicios.AddDbContext<ContextoClientes>(opciones =>
            opciones.UseSqlServer(cadenaConexion, sqlServer => sqlServer.EnableRetryOnFailure()));

        // Aquí se registrarán repositorios, publicadores y consumidores de eventos.
        return servicios;
    }
}
