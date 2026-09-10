namespace Clientes.Aplicacion.Excepciones;

public sealed class ConsultaReporteInvalidaException(string mensaje) : Exception(mensaje);
