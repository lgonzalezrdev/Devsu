using Cuentas.Infraestructura.Persistencia;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Cuentas.Infraestructura.Salud;

/// <summary>Comprueba que la base de datos de Cuentas acepte una conexión EF Core.</summary>
public sealed class ComprobacionSqlServer(ContextoCuentas contextoCuentas) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            bool disponible = await contextoCuentas.Database.CanConnectAsync(cancellationToken);
            return disponible ? HealthCheckResult.Healthy("SQL Server está disponible.") : HealthCheckResult.Unhealthy("SQL Server no aceptó la conexión.");
        }
        catch (Exception excepcion)
        {
            return HealthCheckResult.Unhealthy("No fue posible conectar con SQL Server.", excepcion);
        }
    }
}
