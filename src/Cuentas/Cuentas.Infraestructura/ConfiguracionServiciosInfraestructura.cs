using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Cuentas.Infraestructura.Persistencia;
using Cuentas.Aplicacion.Contratos;
using Cuentas.Infraestructura.Generacion;
using Cuentas.Infraestructura.Mensajeria.Consumidores;
using Cuentas.Infraestructura.Mensajeria;
using MassTransit;

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
        servicios.AddScoped<IRepositorioClientesIntegracion, RepositorioClientesIntegracion>();
        servicios.AddSingleton<IGeneradorNumeroCuenta, GeneradorNumeroCuenta>();

        bool mensajeriaActiva = configuracion.GetValue<bool>("Mensajeria:Activa");

        if (mensajeriaActiva)
        {
            string host = configuracion["Mensajeria:Host"] ?? "localhost";
            ushort puerto = configuracion.GetValue<ushort?>("Mensajeria:Puerto") ?? 5672;
            string usuario = configuracion["Mensajeria:Usuario"] ?? "guest";
            string contrasena = configuracion["Mensajeria:Contrasena"] ?? "guest";

            servicios.AddMassTransit(configurador =>
            {
                configurador.AddConsumer<ConsumidorClienteSincronizado>();
                configurador.AddConsumer<ConsumidorClienteEliminado>();
                configurador.UsingRabbitMq((contexto, configuradorRabbitMq) =>
                {
                    configuradorRabbitMq.Host(host, puerto, "/", configuradorHost =>
                    {
                        configuradorHost.Username(usuario);
                        configuradorHost.Password(contrasena);
                    });
                    configuradorRabbitMq.ReceiveEndpoint("cuentas-clientes", configuradorEndpoint =>
                    {
                        configuradorEndpoint.ConfigureConsumer<ConsumidorClienteSincronizado>(contexto);
                        configuradorEndpoint.ConfigureConsumer<ConsumidorClienteEliminado>(contexto);
                        configuradorEndpoint.UseMessageRetry(configuradorReintento => configuradorReintento.Intervals(
                            TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5)));
                    });
                });
            });
            servicios.AddScoped<IPublicadorEventosIntegracion, PublicadorEventosMassTransit>();
            servicios.AddHostedService<ProcesadorEventosIntegracion>();
        }
        else
        {
            servicios.AddSingleton<IPublicadorEventosIntegracion, PublicadorEventosNulo>();
        }

        return servicios;
    }
}
