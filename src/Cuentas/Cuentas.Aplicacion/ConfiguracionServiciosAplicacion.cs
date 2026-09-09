using Microsoft.Extensions.DependencyInjection;

namespace Cuentas.Aplicacion;

public static class ConfiguracionServiciosAplicacion
{
    public static IServiceCollection AgregarServiciosAplicacionCuentas(this IServiceCollection servicios)
    {
        // Aquí se registrarán casos de uso, validadores y manejadores de comandos/consultas.
        return servicios;
    }
}
