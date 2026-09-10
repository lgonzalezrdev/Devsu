using Cuentas.Aplicacion.Modelos;

namespace Cuentas.Aplicacion.Servicios;

public interface IServicioCuentas
{
    /// <summary>Obtiene las cuentas disponibles.</summary>
    Task<IReadOnlyCollection<CuentaRespuesta>> ObtenerCuentasAsync(CancellationToken tokenCancelacion);
    /// <summary>Obtiene una cuenta por su identificador.</summary>
    Task<CuentaRespuesta> ObtenerCuentaAsync(Guid cuentaId, CancellationToken tokenCancelacion);
    /// <summary>Crea una cuenta y asigna un número único en el servidor.</summary>
    Task<CuentaRespuesta> CrearCuentaAsync(CrearCuentaSolicitud solicitud, CancellationToken tokenCancelacion);
    /// <summary>Actualiza los datos modificables de una cuenta.</summary>
    Task ActualizarCuentaAsync(Guid cuentaId, ActualizarCuentaSolicitud solicitud, CancellationToken tokenCancelacion);
    /// <summary>Cambia el estado activo de una cuenta.</summary>
    Task ActualizarEstadoCuentaAsync(Guid cuentaId, ActualizarEstadoCuentaSolicitud solicitud, CancellationToken tokenCancelacion);
    /// <summary>Obtiene los movimientos de una cuenta.</summary>
    Task<IReadOnlyCollection<MovimientoRespuesta>> ObtenerMovimientosAsync(Guid cuentaId, CancellationToken tokenCancelacion);
    /// <summary>Registra un movimiento y actualiza el saldo de la cuenta.</summary>
    Task<MovimientoRespuesta> CrearMovimientoAsync(CrearMovimientoSolicitud solicitud, CancellationToken tokenCancelacion);
    /// <summary>Actualiza un movimiento y recalcula los saldos de la cuenta.</summary>
    Task ActualizarMovimientoAsync(Guid movimientoId, ActualizarMovimientoSolicitud solicitud, CancellationToken tokenCancelacion);
    /// <summary>Republica las cuentas para reconstruir la proyección de reportes.</summary>
    Task SincronizarTodosAsync(CancellationToken tokenCancelacion);
}
