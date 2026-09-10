using Cuentas.Dominio.Entidades;
using Cuentas.Dominio.Enumeraciones;

namespace Cuentas.Aplicacion.Modelos;

public sealed record CuentaRespuesta(Guid CuentaId, Guid ClienteId, string NumeroCuenta, TipoCuenta TipoCuenta, decimal SaldoInicial, decimal SaldoDisponible, bool Estado)
{
    public static CuentaRespuesta DesdeEntidad(Cuenta cuenta) => new(cuenta.CuentaId, cuenta.ClienteId, cuenta.NumeroCuenta, cuenta.TipoCuenta, cuenta.SaldoInicial, cuenta.SaldoDisponible, cuenta.Estado);
}

public sealed record MovimientoRespuesta(Guid MovimientoId, Guid CuentaId, DateTime Fecha, TipoMovimiento TipoMovimiento, decimal Valor, decimal Saldo)
{
    public static MovimientoRespuesta DesdeEntidad(Movimiento movimiento) => new(movimiento.MovimientoId, movimiento.CuentaId, movimiento.Fecha, movimiento.TipoMovimiento, movimiento.Valor, movimiento.Saldo);
}
