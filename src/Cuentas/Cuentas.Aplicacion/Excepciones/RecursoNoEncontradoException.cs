namespace Cuentas.Aplicacion.Excepciones;

public sealed class RecursoNoEncontradoException(string mensaje) : Exception(mensaje);
