namespace Cuentas.Aplicacion.Excepciones;

/// <summary>Indica que otra operación modificó la cuenta antes de confirmar el saldo.</summary>
public sealed class ConflictoConcurrenciaSaldoException : Exception
{
    public ConflictoConcurrenciaSaldoException()
        : base("El saldo de la cuenta cambió por otra operación. Consulte el saldo actual e intente nuevamente.")
    {
    }
}
