using Clientes.Dominio.Entidades;

namespace Clientes.Aplicacion.Modelos;

public sealed record ReporteEstadoCuentaRespuesta(
    string Cliente,
    DateTime FechaInicio,
    DateTime FechaFin,
    IReadOnlyCollection<CuentaEstadoCuentaRespuesta> Cuentas);

public sealed record CuentaEstadoCuentaRespuesta(
    string NumeroCuenta,
    string Tipo,
    decimal SaldoInicial,
    bool Estado,
    decimal SaldoDisponible,
    IReadOnlyCollection<MovimientoEstadoCuentaRespuesta> Movimientos)
{
    public static CuentaEstadoCuentaRespuesta DesdeEntidad(CuentaReporte cuenta, IReadOnlyCollection<MovimientoReporte> movimientos) =>
        new(
            cuenta.NumeroCuenta,
            cuenta.TipoCuenta,
            cuenta.SaldoInicial,
            cuenta.Estado,
            cuenta.SaldoDisponible,
            movimientos.Select(MovimientoEstadoCuentaRespuesta.DesdeEntidad).ToArray());
}

public sealed record MovimientoEstadoCuentaRespuesta(DateTime Fecha, string TipoMovimiento, decimal Valor, decimal Saldo)
{
    public static MovimientoEstadoCuentaRespuesta DesdeEntidad(MovimientoReporte movimiento) =>
        new(movimiento.Fecha, movimiento.TipoMovimiento, movimiento.Valor, movimiento.Saldo);
}
