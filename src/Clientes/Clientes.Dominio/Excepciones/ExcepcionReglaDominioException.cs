namespace Clientes.Dominio.Excepciones;

public sealed class ExcepcionReglaDominioException(string mensaje) : Exception(mensaje);
