using Clientes.Aplicacion.Contratos;
using Clientes.Aplicacion.Excepciones;
using Clientes.Aplicacion.Modelos;
using Clientes.Dominio.Entidades;

namespace Clientes.Aplicacion.Servicios;

public sealed class ServicioClientes(
    IRepositorioClientes repositorioClientes,
    IEncriptadorContrasena encriptadorContrasena) : IServicioClientes
{
    public async Task<IReadOnlyCollection<ClienteRespuesta>> ObtenerTodosAsync(CancellationToken tokenCancelacion)
    {
        IReadOnlyCollection<Cliente> clientes = await repositorioClientes.ObtenerTodosAsync(tokenCancelacion);
        return clientes.Select(ClienteRespuesta.DesdeEntidad).ToArray();
    }

    public async Task<ClienteRespuesta> ObtenerPorIdAsync(Guid clienteId, CancellationToken tokenCancelacion)
    {
        Cliente cliente = await ObtenerClienteRequeridoAsync(clienteId, tokenCancelacion);
        return ClienteRespuesta.DesdeEntidad(cliente);
    }

    public async Task<ClienteRespuesta> CrearAsync(CrearClienteSolicitud solicitud, CancellationToken tokenCancelacion)
    {
        Cliente cliente = new(
            Guid.NewGuid(),
            solicitud.Nombre,
            solicitud.Genero,
            solicitud.Edad,
            solicitud.Identificacion,
            solicitud.Direccion,
            solicitud.Telefono,
            encriptadorContrasena.GenerarHash(solicitud.Contrasena));

        await repositorioClientes.AgregarAsync(cliente, tokenCancelacion);
        await repositorioClientes.GuardarCambiosAsync(tokenCancelacion);

        return ClienteRespuesta.DesdeEntidad(cliente);
    }

    public async Task ActualizarAsync(Guid clienteId, ActualizarClienteSolicitud solicitud, CancellationToken tokenCancelacion)
    {
        Cliente cliente = await ObtenerClienteRequeridoAsync(clienteId, tokenCancelacion);
        cliente.Actualizar(
            solicitud.Nombre,
            solicitud.Genero,
            solicitud.Edad,
            solicitud.Identificacion,
            solicitud.Direccion,
            solicitud.Telefono);

        if (!string.IsNullOrWhiteSpace(solicitud.Contrasena))
        {
            cliente.CambiarContrasena(encriptadorContrasena.GenerarHash(solicitud.Contrasena));
        }

        await repositorioClientes.GuardarCambiosAsync(tokenCancelacion);
    }

    public async Task ActualizarEstadoAsync(Guid clienteId, ActualizarEstadoClienteSolicitud solicitud, CancellationToken tokenCancelacion)
    {
        Cliente cliente = await ObtenerClienteRequeridoAsync(clienteId, tokenCancelacion);
        cliente.CambiarEstado(solicitud.Estado);
        await repositorioClientes.GuardarCambiosAsync(tokenCancelacion);
    }

    public async Task EliminarAsync(Guid clienteId, CancellationToken tokenCancelacion)
    {
        Cliente cliente = await ObtenerClienteRequeridoAsync(clienteId, tokenCancelacion);
        repositorioClientes.Eliminar(cliente);
        await repositorioClientes.GuardarCambiosAsync(tokenCancelacion);
    }

    private async Task<Cliente> ObtenerClienteRequeridoAsync(Guid clienteId, CancellationToken tokenCancelacion) =>
        await repositorioClientes.ObtenerPorIdAsync(clienteId, tokenCancelacion)
        ?? throw new ClienteNoEncontradoException(clienteId);
}
