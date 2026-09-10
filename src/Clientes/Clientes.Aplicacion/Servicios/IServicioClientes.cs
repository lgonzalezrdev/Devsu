using Clientes.Aplicacion.Modelos;

namespace Clientes.Aplicacion.Servicios;

public interface IServicioClientes
{
    Task<IReadOnlyCollection<ClienteRespuesta>> ObtenerTodosAsync(CancellationToken tokenCancelacion);

    Task<ClienteRespuesta> ObtenerPorIdAsync(Guid clienteId, CancellationToken tokenCancelacion);

    Task<ClienteRespuesta> CrearAsync(CrearClienteSolicitud solicitud, CancellationToken tokenCancelacion);

    Task ActualizarAsync(Guid clienteId, ActualizarClienteSolicitud solicitud, CancellationToken tokenCancelacion);

    Task ActualizarEstadoAsync(Guid clienteId, ActualizarEstadoClienteSolicitud solicitud, CancellationToken tokenCancelacion);

    Task EliminarAsync(Guid clienteId, CancellationToken tokenCancelacion);
}
