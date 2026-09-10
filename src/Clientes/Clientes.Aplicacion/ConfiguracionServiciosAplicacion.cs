using Microsoft.Extensions.DependencyInjection;
using Clientes.Aplicacion.Servicios;

namespace Clientes.Aplicacion;

public static class ConfiguracionServiciosAplicacion
{
    public static IServiceCollection AgregarServiciosAplicacionClientes(this IServiceCollection servicios)
    {
        servicios.AddScoped<IServicioClientes, ServicioClientes>();
        return servicios;
    }
}
