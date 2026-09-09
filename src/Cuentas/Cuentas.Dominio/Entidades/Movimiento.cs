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
        DateTimeOffset fecha,
        TipoMovimiento tipoMovimiento,
        decimal valor,
        decimal saldo)
    {
        if (cuentaId == Guid.Empty)
        {
            throw new ArgumentException("La cuenta es obligatoria.", nameof(cuentaId));
        }

        MovimientoId = movimientoId == Guid.Empty ? Guid.NewGuid() : movimientoId;
        CuentaId = cuentaId;
        Fecha = fecha;
        TipoMovimiento = tipoMovimiento;
        Valor = valor;
        Saldo = saldo;
    }

    public Guid MovimientoId { get; private set; }

    public Guid CuentaId { get; private set; }

    public DateTimeOffset Fecha { get; private set; }

    public TipoMovimiento TipoMovimiento { get; private set; }

    public decimal Valor { get; private set; }

    public decimal Saldo { get; private set; }
}
