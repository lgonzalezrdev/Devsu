using Cuentas.Aplicacion.Contratos;
using Cuentas.Aplicacion.Excepciones;
using Cuentas.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Cuentas.Infraestructura.Persistencia;

public sealed class RepositorioCuentas(ContextoCuentas contextoCuentas) : IRepositorioCuentas
{
    public async Task<IReadOnlyCollection<Cuenta>> ObtenerCuentasAsync(CancellationToken tokenCancelacion) =>
        await contextoCuentas.Cuentas.AsNoTracking().OrderBy(cuenta => cuenta.NumeroCuenta).ToArrayAsync(tokenCancelacion);

    public Task<Cuenta?> ObtenerCuentaPorIdAsync(Guid cuentaId, CancellationToken tokenCancelacion) =>
        contextoCuentas.Cuentas.SingleOrDefaultAsync(cuenta => cuenta.CuentaId == cuentaId, tokenCancelacion);

    public Task<bool> ExisteNumeroCuentaAsync(string numeroCuenta, CancellationToken tokenCancelacion) =>
        contextoCuentas.Cuentas.AnyAsync(cuenta => cuenta.NumeroCuenta == numeroCuenta, tokenCancelacion);

    public Task AgregarCuentaAsync(Cuenta cuenta, CancellationToken tokenCancelacion) =>
        contextoCuentas.Cuentas.AddAsync(cuenta, tokenCancelacion).AsTask();

    public async Task<IReadOnlyCollection<Movimiento>> ObtenerMovimientosAsync(Guid cuentaId, CancellationToken tokenCancelacion) =>
        await contextoCuentas.Movimientos.Where(movimiento => movimiento.CuentaId == cuentaId).OrderBy(movimiento => movimiento.Fecha).ThenBy(movimiento => movimiento.MovimientoId).ToArrayAsync(tokenCancelacion);

    public Task<Movimiento?> ObtenerMovimientoPorIdAsync(Guid movimientoId, CancellationToken tokenCancelacion) =>
        contextoCuentas.Movimientos.SingleOrDefaultAsync(movimiento => movimiento.MovimientoId == movimientoId, tokenCancelacion);

    public Task AgregarMovimientoAsync(Movimiento movimiento, CancellationToken tokenCancelacion) =>
        contextoCuentas.Movimientos.AddAsync(movimiento, tokenCancelacion).AsTask();

    public async Task GuardarCambiosAsync(CancellationToken tokenCancelacion)
    {
        try
        {
            await contextoCuentas.SaveChangesAsync(tokenCancelacion);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictoConcurrenciaSaldoException();
        }
    }
}
