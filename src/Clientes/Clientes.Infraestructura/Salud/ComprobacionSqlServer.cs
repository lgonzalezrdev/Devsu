using Clientes.Infraestructura.Persistencia;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Clientes.Infraestructura.Salud;

/// <summary>Comprueba que la base de datos de Clientes acepte una conexión EF Core.</summary>
public sealed class ComprobacionSqlServer(ContextoClientes contextoClientes) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            bool disponible = await contextoClientes.Database.CanConnectAsync(cancellationToken);
            return disponible ? HealthCheckResult.Healthy("SQL Server está disponible.") : HealthCheckResult.Unhealthy("SQL Server no aceptó la conexión.");
        }
        catch (Exception excepcion)
        {
            return HealthCheckResult.Unhealthy("No fue posible conectar con SQL Server.", excepcion);
        }
    }
}
