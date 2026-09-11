using Clientes.Aplicacion.Contratos;
using Clientes.Aplicacion.Excepciones;
using Clientes.Aplicacion.Modelos;
using Clientes.Dominio.Entidades;
using Clientes.Dominio.Excepciones;
using Contratos.Compartidos.Eventos;
using System.Diagnostics;

namespace Clientes.Aplicacion.Servicios;

public sealed class ServicioClientes(
    IRepositorioClientes repositorioClientes,
    IEncriptadorContrasena encriptadorContrasena,
    IPublicadorEventosIntegracion publicadorEventos) : IServicioClientes
{
    public async Task<IReadOnlyCollection<ClienteRespuesta>> ObtenerTodosAsync(CancellationToken tokenCancelacion)
    {
        IReadOnlyCollection<Cliente> clientes = await repositorioClientes.ObtenerTodosAsync(tokenCancelacion);
        return clientes.Select(ClienteRespuesta.DesdeEntidad).ToArray();
    }

    public async Task<ResultadoPaginado<ClienteRespuesta>> ObtenerPaginadoAsync(ConsultaClientes consulta, CancellationToken tokenCancelacion)
    {
        if (consulta.Pagina < 1 || consulta.TamanoPagina is < 1 or > 100) { throw new ExcepcionReglaDominioException("pagina debe ser mayor a cero y tamanoPagina debe estar entre 1 y 100."); }
        (IReadOnlyCollection<Cliente> clientes, int totalRegistros) = await repositorioClientes.ObtenerPaginadoAsync(consulta, tokenCancelacion);
        return new ResultadoPaginado<ClienteRespuesta>(clientes.Select(ClienteRespuesta.DesdeEntidad).ToArray(), consulta.Pagina, consulta.TamanoPagina, totalRegistros);
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
        await PublicarClienteSincronizadoAsync(cliente, tokenCancelacion);
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

        await PublicarClienteSincronizadoAsync(cliente, tokenCancelacion);
        await repositorioClientes.GuardarCambiosAsync(tokenCancelacion);
    }

    public async Task ActualizarEstadoAsync(Guid clienteId, ActualizarEstadoClienteSolicitud solicitud, CancellationToken tokenCancelacion)
    {
        Cliente cliente = await ObtenerClienteRequeridoAsync(clienteId, tokenCancelacion);
        cliente.CambiarEstado(solicitud.Estado);
        await PublicarClienteSincronizadoAsync(cliente, tokenCancelacion);
        await repositorioClientes.GuardarCambiosAsync(tokenCancelacion);
    }

    public async Task EliminarAsync(Guid clienteId, CancellationToken tokenCancelacion)
    {
        Cliente cliente = await ObtenerClienteRequeridoAsync(clienteId, tokenCancelacion);
        cliente.CambiarEstado(false);
        await PublicarClienteSincronizadoAsync(cliente, tokenCancelacion);
        await repositorioClientes.GuardarCambiosAsync(tokenCancelacion);
    }

    public async Task SincronizarTodosAsync(CancellationToken tokenCancelacion)
    {
        IReadOnlyCollection<Cliente> clientes = await repositorioClientes.ObtenerTodosAsync(tokenCancelacion);

        foreach (Cliente cliente in clientes)
        {
            await PublicarClienteSincronizadoAsync(cliente, tokenCancelacion);
        }
        await repositorioClientes.GuardarCambiosAsync(tokenCancelacion);
    }

    private async Task<Cliente> ObtenerClienteRequeridoAsync(Guid clienteId, CancellationToken tokenCancelacion) =>
        await repositorioClientes.ObtenerPorIdAsync(clienteId, tokenCancelacion)
        ?? throw new ClienteNoEncontradoException(clienteId);

    private Task PublicarClienteSincronizadoAsync(Cliente cliente, CancellationToken tokenCancelacion) =>
        publicadorEventos.RegistrarAsync(
            new ClienteSincronizado(Guid.NewGuid(), cliente.ClienteId, cliente.Nombre, cliente.Estado, DateTime.UtcNow, ObtenerIdCorrelacion()),
            tokenCancelacion);

    private static string ObtenerIdCorrelacion() => Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
}
