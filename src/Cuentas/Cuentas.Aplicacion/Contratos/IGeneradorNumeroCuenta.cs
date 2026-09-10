namespace Cuentas.Aplicacion.Contratos;

public interface IGeneradorNumeroCuenta
{
    /// <summary>Genera un candidato de seis dígitos para una nueva cuenta.</summary>
    string Generar();
}
