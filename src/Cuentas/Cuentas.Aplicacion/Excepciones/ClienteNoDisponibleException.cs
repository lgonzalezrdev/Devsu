namespace Cuentas.Aplicacion.Excepciones;

public sealed class ClienteNoDisponibleException(string mensaje) : Exception(mensaje);
