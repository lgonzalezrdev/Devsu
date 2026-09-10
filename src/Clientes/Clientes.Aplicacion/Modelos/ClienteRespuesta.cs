using Clientes.Dominio.Entidades;
using Clientes.Dominio.Enumeraciones;

namespace Clientes.Aplicacion.Modelos;

public sealed record ClienteRespuesta(
    Guid ClienteId,
    string Nombre,
    Genero Genero,
    int Edad,
    string Identificacion,
    string Direccion,
    string Telefono,
    bool Estado)
{
    public static ClienteRespuesta DesdeEntidad(Cliente cliente) => new(
        cliente.ClienteId,
        cliente.Nombre,
        cliente.Genero,
        cliente.Edad,
        cliente.Identificacion,
        cliente.Direccion,
        cliente.Telefono,
        cliente.Estado);
}
