using System.ComponentModel.DataAnnotations;
using Cuentas.Dominio.Enumeraciones;

namespace Cuentas.Aplicacion.Modelos;

public sealed class CrearCuentaSolicitud
{
    public Guid ClienteId { get; init; }
    [EnumDataType(typeof(TipoCuenta), ErrorMessage = "El tipo de cuenta debe ser Ahorros o Corriente.")]
    public TipoCuenta TipoCuenta { get; init; }
    [Range(0, 999999999, ErrorMessage = "El saldo inicial no puede ser negativo.")]
    public decimal SaldoInicial { get; init; }
}

public sealed class ActualizarCuentaSolicitud
{
    [EnumDataType(typeof(TipoCuenta), ErrorMessage = "El tipo de cuenta debe ser Ahorros o Corriente.")]
    public TipoCuenta TipoCuenta { get; init; }
}

public sealed record ActualizarEstadoCuentaSolicitud(bool Estado);

public sealed class CrearMovimientoSolicitud
{
    public Guid CuentaId { get; init; }
    [EnumDataType(typeof(TipoMovimiento), ErrorMessage = "El tipo de movimiento debe ser Deposito o Retiro.")]
    public TipoMovimiento TipoMovimiento { get; init; }
    [Range(0.01, 999999999, ErrorMessage = "El valor del movimiento debe ser positivo.")]
    public decimal Valor { get; init; }
}

public sealed class ActualizarMovimientoSolicitud
{
    [EnumDataType(typeof(TipoMovimiento), ErrorMessage = "El tipo de movimiento debe ser Deposito o Retiro.")]
    public TipoMovimiento TipoMovimiento { get; init; }
    [Range(0.01, 999999999, ErrorMessage = "El valor del movimiento debe ser positivo.")]
    public decimal Valor { get; init; }
}
