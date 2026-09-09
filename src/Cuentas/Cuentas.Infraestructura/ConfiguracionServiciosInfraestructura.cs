using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cuentas.Infraestructura;

public static class ConfiguracionServiciosInfraestructura
{
    public static IServiceCollection AgregarServiciosInfraestructuraCuentas(
        this IServiceCollection servicios,
        IConfiguration configuracion)
    {
        // Aquí se registrarán DbContext, repositorios, publicadores y consumidores de eventos.
        return servicios;
    }
}
