using Microsoft.Extensions.DependencyInjection;
using Cuentas.Aplicacion.Servicios;

namespace Cuentas.Aplicacion;

public static class ConfiguracionServiciosAplicacion
{
    public static IServiceCollection AgregarServiciosAplicacionCuentas(this IServiceCollection servicios)
    {
        servicios.AddScoped<IServicioCuentas, ServicioCuentas>();
        return servicios;
    }
}
