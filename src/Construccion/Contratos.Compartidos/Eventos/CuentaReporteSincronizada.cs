namespace Contratos.Compartidos.Eventos;

/// <summary>Transfiere una instantánea de cuenta y movimientos para el reporte distribuido.</summary>
public sealed record CuentaReporteSincronizada(
    Guid EventoId,
    Guid CuentaId,
    Guid ClienteId,
    string NumeroCuenta,
    string TipoCuenta,
    decimal SaldoInicial,
    decimal SaldoDisponible,
    bool Estado,
    DateTime FechaOcurrencia,
    IReadOnlyCollection<MovimientoReporteSincronizado> Movimientos,
    string IdCorrelacion);

/// <summary>Representa un movimiento dentro de la instantánea de una cuenta.</summary>
public sealed record MovimientoReporteSincronizado(
    Guid MovimientoId,
    DateTime Fecha,
    string TipoMovimiento,
    decimal Valor,
    decimal Saldo);
