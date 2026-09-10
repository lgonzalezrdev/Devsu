namespace Clientes.Dominio.Entidades;

/// <summary>Proyección local de una cuenta recibida desde el microservicio Cuentas.</summary>
public sealed class CuentaReporte
{
    private CuentaReporte()
    {
    }

    public CuentaReporte(Guid cuentaId, Guid clienteId, string numeroCuenta, string tipoCuenta, decimal saldoInicial, decimal saldoDisponible, bool estado, DateTime actualizadoEn)
    {
        CuentaId = cuentaId;
        ClienteId = clienteId;
        NumeroCuenta = numeroCuenta;
        TipoCuenta = tipoCuenta;
        SaldoInicial = saldoInicial;
        SaldoDisponible = saldoDisponible;
        Estado = estado;
        ActualizadoEn = actualizadoEn;
    }

    public Guid CuentaId { get; private set; }
    public Guid ClienteId { get; private set; }
    public string NumeroCuenta { get; private set; } = null!;
    public string TipoCuenta { get; private set; } = null!;
    public decimal SaldoInicial { get; private set; }
    public decimal SaldoDisponible { get; private set; }
    public bool Estado { get; private set; }
    public DateTime ActualizadoEn { get; private set; }

    /// <summary>Aplica la instantánea más reciente de la cuenta.</summary>
    public bool Actualizar(string numeroCuenta, string tipoCuenta, decimal saldoInicial, decimal saldoDisponible, bool estado, DateTime actualizadoEn)
    {
        if (actualizadoEn < ActualizadoEn)
        {
            return false;
        }

        NumeroCuenta = numeroCuenta;
        TipoCuenta = tipoCuenta;
        SaldoInicial = saldoInicial;
        SaldoDisponible = saldoDisponible;
        Estado = estado;
        ActualizadoEn = actualizadoEn;
        return true;
    }
}
