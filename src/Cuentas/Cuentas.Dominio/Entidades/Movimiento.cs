using Cuentas.Dominio.Enumeraciones;

namespace Cuentas.Dominio.Entidades;

public sealed class Movimiento
{
    private Movimiento()
    {
    }

    public Movimiento(
        Guid movimientoId,
        Guid cuentaId,
        DateTime fecha,
        TipoMovimiento tipoMovimiento,
        decimal valor,
        decimal saldo)
    {
        if (cuentaId == Guid.Empty)
        {
            throw new Excepciones.ExcepcionReglaDominioException("La cuenta es obligatoria.");
        }

        ValidarMonto(tipoMovimiento, valor);

        MovimientoId = movimientoId == Guid.Empty ? Guid.NewGuid() : movimientoId;
        CuentaId = cuentaId;
        Fecha = fecha;
        TipoMovimiento = tipoMovimiento;
        Valor = ObtenerValorFirmado(tipoMovimiento, valor);
        Saldo = saldo;
    }

    public Guid MovimientoId { get; private set; }

    public Guid CuentaId { get; private set; }

    public DateTime Fecha { get; private set; }

    public TipoMovimiento TipoMovimiento { get; private set; }

    public decimal Valor { get; private set; }

    public decimal Saldo { get; private set; }

    /// <summary>Actualiza el tipo y monto positivo; un retiro se guarda como débito.</summary>
    public void Actualizar(TipoMovimiento tipoMovimiento, decimal valor)
    {
        ValidarMonto(tipoMovimiento, valor);
        TipoMovimiento = tipoMovimiento;
        Valor = ObtenerValorFirmado(tipoMovimiento, valor);
    }

    /// <summary>Establece el saldo resultante calculado por la cuenta.</summary>
    public void ActualizarSaldo(decimal saldo) => Saldo = saldo;

    /// <summary>Verifica que el monto recibido sea positivo y el tipo sea válido.</summary>
    public static void ValidarMonto(TipoMovimiento tipoMovimiento, decimal valor)
    {
        if (!Enum.IsDefined(tipoMovimiento))
        {
            throw new Excepciones.ExcepcionReglaDominioException("El tipo de movimiento debe ser Deposito o Retiro.");
        }

        if (valor <= 0)
        {
            throw new Excepciones.ExcepcionReglaDominioException("El valor del movimiento debe ser positivo.");
        }
    }

    private static decimal ObtenerValorFirmado(TipoMovimiento tipoMovimiento, decimal valor) =>
        tipoMovimiento == TipoMovimiento.Retiro ? -valor : valor;
}
