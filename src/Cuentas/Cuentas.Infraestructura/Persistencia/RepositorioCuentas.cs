using Cuentas.Aplicacion.Contratos;
using Cuentas.Aplicacion.Excepciones;
using Cuentas.Dominio.Entidades;
using Cuentas.Aplicacion.Modelos;
using Microsoft.EntityFrameworkCore;

namespace Cuentas.Infraestructura.Persistencia;

public sealed class RepositorioCuentas(ContextoCuentas contextoCuentas) : IRepositorioCuentas
{
    public async Task<IReadOnlyCollection<Cuenta>> ObtenerCuentasAsync(CancellationToken tokenCancelacion) =>
        await contextoCuentas.Cuentas.AsNoTracking().OrderBy(cuenta => cuenta.NumeroCuenta).ToArrayAsync(tokenCancelacion);

    public Task<Cuenta?> ObtenerCuentaPorIdAsync(Guid cuentaId, CancellationToken tokenCancelacion) =>
        contextoCuentas.Cuentas.SingleOrDefaultAsync(cuenta => cuenta.CuentaId == cuentaId, tokenCancelacion);

    public async Task<(IReadOnlyCollection<Cuenta> Cuentas, int TotalRegistros)> ObtenerCuentasPaginadasAsync(ConsultaCuentas consulta, CancellationToken tokenCancelacion)
    {
        IQueryable<Cuenta> consultaBase = contextoCuentas.Cuentas.AsNoTracking();
        if (consulta.ClienteId.HasValue) { consultaBase = consultaBase.Where(cuenta => cuenta.ClienteId == consulta.ClienteId.Value); }
        if (consulta.Estado.HasValue) { consultaBase = consultaBase.Where(cuenta => cuenta.Estado == consulta.Estado.Value); }
        int totalRegistros = await consultaBase.CountAsync(tokenCancelacion);
        IReadOnlyCollection<Cuenta> cuentas = await consultaBase.OrderBy(cuenta => cuenta.NumeroCuenta).Skip((consulta.Pagina - 1) * consulta.TamanoPagina).Take(consulta.TamanoPagina).ToArrayAsync(tokenCancelacion);
        return (cuentas, totalRegistros);
    }

    public Task<bool> ExisteNumeroCuentaAsync(string numeroCuenta, CancellationToken tokenCancelacion) =>
        contextoCuentas.Cuentas.AnyAsync(cuenta => cuenta.NumeroCuenta == numeroCuenta, tokenCancelacion);

    public Task AgregarCuentaAsync(Cuenta cuenta, CancellationToken tokenCancelacion) =>
        contextoCuentas.Cuentas.AddAsync(cuenta, tokenCancelacion).AsTask();

    public async Task<IReadOnlyCollection<Movimiento>> ObtenerMovimientosAsync(Guid cuentaId, CancellationToken tokenCancelacion) =>
        await contextoCuentas.Movimientos.Where(movimiento => movimiento.CuentaId == cuentaId).OrderBy(movimiento => movimiento.Fecha).ThenBy(movimiento => movimiento.MovimientoId).ToArrayAsync(tokenCancelacion);

    public Task<Movimiento?> ObtenerMovimientoPorIdAsync(Guid movimientoId, CancellationToken tokenCancelacion) =>
        contextoCuentas.Movimientos.SingleOrDefaultAsync(movimiento => movimiento.MovimientoId == movimientoId, tokenCancelacion);

    public async Task<(IReadOnlyCollection<Movimiento> Movimientos, int TotalRegistros)> ObtenerMovimientosPaginadosAsync(Guid cuentaId, ConsultaMovimientos consulta, CancellationToken tokenCancelacion)
    {
        IQueryable<Movimiento> consultaBase = contextoCuentas.Movimientos.Where(movimiento => movimiento.CuentaId == cuentaId);
        if (consulta.FechaInicio.HasValue) { consultaBase = consultaBase.Where(movimiento => movimiento.Fecha >= consulta.FechaInicio.Value); }
        if (consulta.FechaFin.HasValue) { consultaBase = consultaBase.Where(movimiento => movimiento.Fecha <= consulta.FechaFin.Value); }
        int totalRegistros = await consultaBase.CountAsync(tokenCancelacion);
        IReadOnlyCollection<Movimiento> movimientos = await consultaBase.OrderBy(movimiento => movimiento.Fecha).ThenBy(movimiento => movimiento.MovimientoId).Skip((consulta.Pagina - 1) * consulta.TamanoPagina).Take(consulta.TamanoPagina).ToArrayAsync(tokenCancelacion);
        return (movimientos, totalRegistros);
    }

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
