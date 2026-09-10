using System.Security.Cryptography;
using System.Globalization;
using Cuentas.Aplicacion.Contratos;

namespace Cuentas.Infraestructura.Generacion;

public sealed class GeneradorNumeroCuenta : IGeneradorNumeroCuenta
{
    public string Generar()
    {
        int numero = RandomNumberGenerator.GetInt32(100000, 1_000_000);
        return numero.ToString(CultureInfo.InvariantCulture);
    }
}
