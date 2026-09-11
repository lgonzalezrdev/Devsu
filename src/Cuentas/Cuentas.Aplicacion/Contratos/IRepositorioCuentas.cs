using Cuentas.Dominio.Entidades;
using Cuentas.Aplicacion.Modelos;

namespace Cuentas.Aplicacion.Contratos;

public interface IRepositorioCuentas
{
    /// <summary>Obtiene las cuentas registradas para consulta.</summary>
    Task<IReadOnlyCollection<Cuenta>> ObtenerCuentasAsync(CancellationToken tokenCancelacion);
    Task<(IReadOnlyCollection<Cuenta> Cuentas, int TotalRegistros)> ObtenerCuentasPaginadasAsync(ConsultaCuentas consulta, CancellationToken tokenCancelacion);
    /// <summary>Busca una cuenta por su identificador.</summary>
    Task<Cuenta?> ObtenerCuentaPorIdAsync(Guid cuentaId, CancellationToken tokenCancelacion);
    /// <summary>Indica si un número de cuenta ya está asignado.</summary>
    Task<bool> ExisteNumeroCuentaAsync(string numeroCuenta, CancellationToken tokenCancelacion);
    /// <summary>Agrega una cuenta al contexto de persistencia.</summary>
    Task AgregarCuentaAsync(Cuenta cuenta, CancellationToken tokenCancelacion);
    /// <summary>Obtiene los movimientos de una cuenta en orden cronológico.</summary>
    Task<IReadOnlyCollection<Movimiento>> ObtenerMovimientosAsync(Guid cuentaId, CancellationToken tokenCancelacion);
    Task<(IReadOnlyCollection<Movimiento> Movimientos, int TotalRegistros)> ObtenerMovimientosPaginadosAsync(Guid cuentaId, ConsultaMovimientos consulta, CancellationToken tokenCancelacion);
    /// <summary>Busca un movimiento por su identificador.</summary>
    Task<Movimiento?> ObtenerMovimientoPorIdAsync(Guid movimientoId, CancellationToken tokenCancelacion);
    /// <summary>Agrega un movimiento al contexto de persistencia.</summary>
    Task AgregarMovimientoAsync(Movimiento movimiento, CancellationToken tokenCancelacion);
    /// <summary>Confirma los cambios pendientes en la persistencia.</summary>
    Task GuardarCambiosAsync(CancellationToken tokenCancelacion);
}
