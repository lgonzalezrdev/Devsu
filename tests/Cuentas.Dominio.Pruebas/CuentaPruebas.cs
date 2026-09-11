using Cuentas.Dominio.Entidades;
using Cuentas.Dominio.Enumeraciones;
using Cuentas.Dominio.Excepciones;
using Xunit;

namespace Cuentas.Dominio.Pruebas;

public sealed class CuentaPruebas
{
    [Fact]
    public void RetiroSinSaldoLanzaExcepcionYNoModificaSaldo()
    {
        Cuenta cuenta = CrearCuenta(100);
        Assert.Throws<SaldoNoDisponibleException>(() => cuenta.RegistrarMovimiento(DateTime.Now, TipoMovimiento.Retiro, 101));
        Assert.Equal(100, cuenta.SaldoDisponible);
    }

    [Fact]
    public void RecalcularMovimientosActualizaSaldoDisponible()
    {
        Cuenta cuenta = CrearCuenta(100);
        Movimiento deposito = cuenta.RegistrarMovimiento(new DateTime(2026, 1, 1), TipoMovimiento.Deposito, 50);
        Movimiento retiro = cuenta.RegistrarMovimiento(new DateTime(2026, 1, 2), TipoMovimiento.Retiro, 20);
        cuenta.RecalcularSaldos([deposito, retiro]);
        Assert.Equal(130, cuenta.SaldoDisponible);
        Assert.Equal(150, deposito.Saldo);
        Assert.Equal(130, retiro.Saldo);
    }

    private static Cuenta CrearCuenta(decimal saldoInicial) => new(Guid.NewGuid(), Guid.NewGuid(), "123456", TipoCuenta.Ahorros, saldoInicial);
}
