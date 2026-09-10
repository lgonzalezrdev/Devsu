using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Cuentas.Infraestructura.Persistencia;
using Cuentas.Aplicacion.Contratos;
using Cuentas.Infraestructura.Generacion;

namespace Cuentas.Infraestructura;

public static class ConfiguracionServiciosInfraestructura
{
    public static IServiceCollection AgregarServiciosInfraestructuraCuentas(
        this IServiceCollection servicios,
        IConfiguration configuracion)
    {
        string cadenaConexion = configuracion.GetConnectionString("Cuentas")
            ?? throw new InvalidOperationException("No se configuró la cadena de conexión 'Cuentas'.");

        servicios.AddDbContext<ContextoCuentas>(opciones =>
            opciones.UseSqlServer(cadenaConexion, sqlServer => sqlServer.EnableRetryOnFailure()));

        servicios.AddScoped<IRepositorioCuentas, RepositorioCuentas>();
        servicios.AddSingleton<IGeneradorNumeroCuenta, GeneradorNumeroCuenta>();

        // Aquí se registrarán publicadores y consumidores de eventos.
        return servicios;
    }
}
