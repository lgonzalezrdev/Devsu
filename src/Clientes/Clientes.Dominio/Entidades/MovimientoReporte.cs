namespace Clientes.Dominio.Entidades;

/// <summary>Proyección local de un movimiento usada exclusivamente por el reporte.</summary>
public sealed class MovimientoReporte
{
    private MovimientoReporte()
    {
    }

    public MovimientoReporte(Guid movimientoId, Guid cuentaId, DateTime fecha, string tipoMovimiento, decimal valor, decimal saldo)
    {
        MovimientoId = movimientoId;
        CuentaId = cuentaId;
        Fecha = fecha;
        TipoMovimiento = tipoMovimiento;
        Valor = valor;
        Saldo = saldo;
    }

    public Guid MovimientoId { get; private set; }
    public Guid CuentaId { get; private set; }
    public DateTime Fecha { get; private set; }
    public string TipoMovimiento { get; private set; } = null!;
    public decimal Valor { get; private set; }
    public decimal Saldo { get; private set; }

    /// <summary>Actualiza el movimiento cuando llega una instantánea posterior de su cuenta.</summary>
    public void Actualizar(DateTime fecha, string tipoMovimiento, decimal valor, decimal saldo)
    {
        Fecha = fecha;
        TipoMovimiento = tipoMovimiento;
        Valor = valor;
        Saldo = saldo;
    }
}
