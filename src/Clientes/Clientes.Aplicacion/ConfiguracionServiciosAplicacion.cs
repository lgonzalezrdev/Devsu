using Microsoft.Extensions.DependencyInjection;

namespace Clientes.Aplicacion;

public static class ConfiguracionServiciosAplicacion
{
    public static IServiceCollection AgregarServiciosAplicacionClientes(this IServiceCollection servicios)
    {
        // Aquí se registrarán casos de uso, validadores y manejadores de comandos/consultas.
        return servicios;
    }
}
