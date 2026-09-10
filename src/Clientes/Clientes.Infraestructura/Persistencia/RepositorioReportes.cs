using Clientes.Aplicacion.Contratos;
using Clientes.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Infraestructura.Persistencia;

public sealed class RepositorioReportes(ContextoClientes contextoClientes) : IRepositorioReportes
{
    public async Task<IReadOnlyCollection<CuentaReporte>> ObtenerCuentasPorClienteAsync(Guid clienteId, CancellationToken tokenCancelacion) =>
        await contextoClientes.CuentasReporte.AsNoTracking()
            .Where(cuenta => cuenta.ClienteId == clienteId)
            .OrderBy(cuenta => cuenta.NumeroCuenta)
            .ToArrayAsync(tokenCancelacion);

    public async Task<IReadOnlyCollection<MovimientoReporte>> ObtenerMovimientosAsync(IReadOnlyCollection<Guid> cuentasId, DateTime fechaInicio, DateTime fechaFin, CancellationToken tokenCancelacion)
    {
        if (cuentasId.Count == 0)
        {
            return Array.Empty<MovimientoReporte>();
        }

        return await contextoClientes.MovimientosReporte.AsNoTracking()
            .Where(movimiento => cuentasId.Contains(movimiento.CuentaId) && movimiento.Fecha >= fechaInicio && movimiento.Fecha <= fechaFin)
            .OrderBy(movimiento => movimiento.Fecha)
            .ThenBy(movimiento => movimiento.MovimientoId)
            .ToArrayAsync(tokenCancelacion);
    }

    public Task<CuentaReporte?> ObtenerCuentaPorIdAsync(Guid cuentaId, CancellationToken tokenCancelacion) =>
        contextoClientes.CuentasReporte.SingleOrDefaultAsync(cuenta => cuenta.CuentaId == cuentaId, tokenCancelacion);

    public Task<MovimientoReporte?> ObtenerMovimientoPorIdAsync(Guid movimientoId, CancellationToken tokenCancelacion) =>
        contextoClientes.MovimientosReporte.SingleOrDefaultAsync(movimiento => movimiento.MovimientoId == movimientoId, tokenCancelacion);

    public Task AgregarCuentaAsync(CuentaReporte cuenta, CancellationToken tokenCancelacion) =>
        contextoClientes.CuentasReporte.AddAsync(cuenta, tokenCancelacion).AsTask();

    public Task AgregarMovimientoAsync(MovimientoReporte movimiento, CancellationToken tokenCancelacion) =>
        contextoClientes.MovimientosReporte.AddAsync(movimiento, tokenCancelacion).AsTask();

    public Task GuardarCambiosAsync(CancellationToken tokenCancelacion) => contextoClientes.SaveChangesAsync(tokenCancelacion);
}
