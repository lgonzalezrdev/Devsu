using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Clientes.Infraestructura.Persistencia;
using Clientes.Infraestructura.Seguridad;
using Clientes.Aplicacion.Contratos;
using Clientes.Infraestructura.Mensajeria;
using Clientes.Infraestructura.Mensajeria.Consumidores;
using MassTransit;

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
        servicios.AddScoped<IRepositorioReportes, RepositorioReportes>();
        servicios.AddSingleton<IEncriptadorContrasena, EncriptadorContrasena>();

        bool mensajeriaActiva = configuracion.GetValue<bool>("Mensajeria:Activa");

        if (mensajeriaActiva)
        {
            string host = configuracion["Mensajeria:Host"] ?? "localhost";
            ushort puerto = configuracion.GetValue<ushort?>("Mensajeria:Puerto") ?? 5672;
            string usuario = configuracion["Mensajeria:Usuario"] ?? "guest";
            string contrasena = configuracion["Mensajeria:Contrasena"] ?? "guest";

            servicios.AddMassTransit(configurador =>
            {
                configurador.AddConsumer<ConsumidorCuentaReporteSincronizada>();
                configurador.UsingRabbitMq((contexto, configuradorRabbitMq) =>
                {
                    configuradorRabbitMq.Host(host, puerto, "/", configuradorHost =>
                    {
                        configuradorHost.Username(usuario);
                        configuradorHost.Password(contrasena);
                    });
                    configuradorRabbitMq.UseMessageRetry(configuradorReintento => configuradorReintento.Intervals(
                        TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5)));
                    configuradorRabbitMq.ReceiveEndpoint("clientes-reportes", configuradorEndpoint =>
                    {
                        configuradorEndpoint.ConfigureConsumer<ConsumidorCuentaReporteSincronizada>(contexto);
                        configuradorEndpoint.UseMessageRetry(configuradorReintento => configuradorReintento.Intervals(
                            TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5)));
                    });
                });
            });
            servicios.AddScoped<IPublicadorEventosIntegracion, PublicadorEventosMassTransit>();
        }
        else
        {
            servicios.AddSingleton<IPublicadorEventosIntegracion, PublicadorEventosNulo>();
        }

        return servicios;
    }
}
