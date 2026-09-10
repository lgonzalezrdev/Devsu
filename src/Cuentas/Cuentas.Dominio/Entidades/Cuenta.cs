using Cuentas.Dominio.Enumeraciones;

namespace Cuentas.Dominio.Entidades;

public sealed class Cuenta
{
    private Cuenta()
    {
    }

    public Cuenta(
        Guid cuentaId,
        Guid clienteId,
        string numeroCuenta,
        TipoCuenta tipoCuenta,
        decimal saldoInicial,
        bool estado = true)
    {
        if (clienteId == Guid.Empty)
        {
            throw new Excepciones.ExcepcionReglaDominioException("El cliente es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(numeroCuenta) || numeroCuenta.Length != 6 || !numeroCuenta.All(char.IsDigit))
        {
            throw new Excepciones.ExcepcionReglaDominioException("El número de cuenta debe tener exactamente 6 dígitos.");
        }

        if (saldoInicial < 0)
        {
            throw new Excepciones.ExcepcionReglaDominioException("El saldo inicial no puede ser negativo.");
        }

        if (!Enum.IsDefined(tipoCuenta))
        {
            throw new Excepciones.ExcepcionReglaDominioException("El tipo de cuenta debe ser Ahorros o Corriente.");
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

    public bool Estado { get; private set; } = true;

    /// <summary>Actualiza el tipo de cuenta sin modificar su número asignado.</summary>
    public void Actualizar(TipoCuenta tipoCuenta)
    {
        if (!Enum.IsDefined(tipoCuenta))
        {
            throw new Excepciones.ExcepcionReglaDominioException("El tipo de cuenta debe ser Ahorros o Corriente.");
        }

        TipoCuenta = tipoCuenta;
    }

    /// <summary>Cambia el estado de la cuenta y evita inactivaciones repetidas.</summary>
    public void CambiarEstado(bool estado)
    {
        if (!estado && !Estado)
        {
            throw new Excepciones.ExcepcionReglaDominioException("La cuenta ya se encuentra inactiva; el estado no ha cambiado.");
        }

        Estado = estado;
    }

    /// <summary>Registra un movimiento positivo y actualiza el saldo disponible.</summary>
    public Movimiento RegistrarMovimiento(DateTime fecha, TipoMovimiento tipoMovimiento, decimal valor)
    {
        if (!Estado)
        {
            throw new Excepciones.ExcepcionReglaDominioException("No se pueden registrar movimientos en una cuenta inactiva.");
        }

        Movimiento movimiento = new(Guid.NewGuid(), CuentaId, fecha, tipoMovimiento, valor, 0);
        decimal nuevoSaldo = SaldoDisponible + movimiento.Valor;

        if (nuevoSaldo < 0)
        {
            throw new Excepciones.SaldoNoDisponibleException();
        }

        SaldoDisponible = nuevoSaldo;
        movimiento.ActualizarSaldo(nuevoSaldo);
        return movimiento;
    }

    /// <summary>Recalcula saldos después de modificar un movimiento histórico.</summary>
    public void RecalcularSaldos(IEnumerable<Movimiento> movimientos)
    {
        decimal saldoCalculado = SaldoInicial;

        foreach (Movimiento movimiento in movimientos.OrderBy(movimiento => movimiento.Fecha).ThenBy(movimiento => movimiento.MovimientoId))
        {
            saldoCalculado += movimiento.Valor;

            if (saldoCalculado < 0)
            {
                throw new Excepciones.SaldoNoDisponibleException();
            }

            movimiento.ActualizarSaldo(saldoCalculado);
        }

        SaldoDisponible = saldoCalculado;
    }
}
