using Cuentas.Dominio.Enumeraciones;

namespace Cuentas.Dominio.Entidades;

public sealed class Cuenta
{
    private readonly List<Movimiento> movimientos = [];

    private Cuenta()
    {
    }

    public Cuenta(
        Guid cuentaId,
        Guid clienteId,
        string numeroCuenta,
        TipoCuenta tipoCuenta,
        decimal saldoInicial,
        bool estado)
    {
        if (clienteId == Guid.Empty)
        {
            throw new ArgumentException("El cliente es obligatorio.", nameof(clienteId));
        }

        if (string.IsNullOrWhiteSpace(numeroCuenta))
        {
            throw new ArgumentException("El número de cuenta es obligatorio.", nameof(numeroCuenta));
        }

        if (saldoInicial < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(saldoInicial), "El saldo inicial no puede ser negativo.");
        }

        CuentaId = cuentaId == Guid.Empty ? Guid.NewGuid() : cuentaId;
        ClienteId = clienteId;
        NumeroCuenta = numeroCuenta.Trim();
        TipoCuenta = tipoCuenta;
        SaldoInicial = saldoInicial;
        SaldoDisponible = saldoInicial;
        Estado = estado;
    }

    public Guid CuentaId { get; private set; }

    public Guid ClienteId { get; private set; }

    public string NumeroCuenta { get; private set; } = null!;

    public TipoCuenta TipoCuenta { get; private set; }

    public decimal SaldoInicial { get; private set; }

    public decimal SaldoDisponible { get; private set; }

    public bool Estado { get; private set; }

    public IReadOnlyCollection<Movimiento> Movimientos => movimientos.AsReadOnly();
}
