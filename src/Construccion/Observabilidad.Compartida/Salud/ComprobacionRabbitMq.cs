using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Sockets;

namespace Observabilidad.Compartida.Salud;

/// <summary>Comprueba que RabbitMQ acepte conexiones TCP en la configuración indicada.</summary>
public sealed class ComprobacionRabbitMq(IConfiguration configuracion) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (!configuracion.GetValue<bool>("Mensajeria:Activa"))
        {
            return HealthCheckResult.Healthy("La mensajería está desactivada para este entorno.");
        }

        string host = configuracion["Mensajeria:Host"] ?? "localhost";
        int puerto = configuracion.GetValue<int?>("Mensajeria:Puerto") ?? 5672;
        try
        {
            using TcpClient cliente = new();
            await cliente.ConnectAsync(host, puerto, cancellationToken);
            return HealthCheckResult.Healthy("RabbitMQ acepta conexiones.");
        }
        catch (Exception excepcion)
        {
            return HealthCheckResult.Unhealthy("No fue posible conectar con RabbitMQ.", excepcion);
        }
    }
}
