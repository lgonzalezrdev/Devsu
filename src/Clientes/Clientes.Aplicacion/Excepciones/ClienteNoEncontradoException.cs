namespace Clientes.Aplicacion.Excepciones;

public sealed class ClienteNoEncontradoException(Guid clienteId)
    : Exception($"No existe el cliente con identificador '{clienteId}'.");
