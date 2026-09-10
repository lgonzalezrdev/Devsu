using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Clientes.Infraestructura.Persistencia;
using Clientes.Infraestructura.Seguridad;
using Clientes.Aplicacion.Contratos;

namespace Clientes.Infraestructura;

public static class ConfiguracionServiciosInfraestructura
{
    public static IServiceCollection AgregarServiciosInfraestructuraClientes(
        this IServiceCollection servicios,
        IConfiguration configuracion)
    {
        string cadenaConexion = configuracion.GetConnectionString("Clientes")
            ?? throw new InvalidOperationException("No se configuró la cadena de conexión 'Clientes'.");

        servicios.AddDbContext<ContextoClientes>(opciones =>
            opciones.UseSqlServer(cadenaConexion, sqlServer => sqlServer.EnableRetryOnFailure()));

        servicios.AddScoped<IRepositorioClientes, RepositorioClientes>();
        servicios.AddSingleton<IEncriptadorContrasena, EncriptadorContrasena>();

        // Aquí se registrarán publicadores y consumidores de eventos.
        return servicios;
    }
}
