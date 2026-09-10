using Clientes.Dominio.Entidades;

namespace Clientes.Aplicacion.Contratos;

/// <summary>Consulta y actualiza las proyecciones locales necesarias para estado de cuenta.</summary>
public interface IRepositorioReportes
{
    /// <summary>Obtiene las cuentas sincronizadas de un cliente.</summary>
    Task<IReadOnlyCollection<CuentaReporte>> ObtenerCuentasPorClienteAsync(Guid clienteId, CancellationToken tokenCancelacion);
    /// <summary>Obtiene movimientos de cuentas dentro del rango solicitado.</summary>
    Task<IReadOnlyCollection<MovimientoReporte>> ObtenerMovimientosAsync(IReadOnlyCollection<Guid> cuentasId, DateTime fechaInicio, DateTime fechaFin, CancellationToken tokenCancelacion);
    /// <summary>Busca una cuenta proyectada para aplicar una nueva instantánea.</summary>
    Task<CuentaReporte?> ObtenerCuentaPorIdAsync(Guid cuentaId, CancellationToken tokenCancelacion);
    /// <summary>Busca un movimiento proyectado para actualizarlo.</summary>
    Task<MovimientoReporte?> ObtenerMovimientoPorIdAsync(Guid movimientoId, CancellationToken tokenCancelacion);
    /// <summary>Agrega una cuenta proyectada.</summary>
    Task AgregarCuentaAsync(CuentaReporte cuenta, CancellationToken tokenCancelacion);
    /// <summary>Agrega un movimiento proyectado.</summary>
    Task AgregarMovimientoAsync(MovimientoReporte movimiento, CancellationToken tokenCancelacion);
    /// <summary>Persiste los cambios de las proyecciones.</summary>
    Task GuardarCambiosAsync(CancellationToken tokenCancelacion);
}
