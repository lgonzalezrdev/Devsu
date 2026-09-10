using Clientes.Aplicacion.Modelos;

namespace Clientes.Aplicacion.Servicios;

public interface IServicioClientes
{
    /// <summary>Obtiene los clientes disponibles.</summary>
    Task<IReadOnlyCollection<ClienteRespuesta>> ObtenerTodosAsync(CancellationToken tokenCancelacion);

    /// <summary>Obtiene un cliente por su identificador.</summary>
    Task<ClienteRespuesta> ObtenerPorIdAsync(Guid clienteId, CancellationToken tokenCancelacion);

    /// <summary>Crea un cliente activo con la información validada.</summary>
    Task<ClienteRespuesta> CrearAsync(CrearClienteSolicitud solicitud, CancellationToken tokenCancelacion);

    /// <summary>Actualiza los datos generales de un cliente.</summary>
    Task ActualizarAsync(Guid clienteId, ActualizarClienteSolicitud solicitud, CancellationToken tokenCancelacion);

    /// <summary>Cambia únicamente el estado activo del cliente.</summary>
    Task ActualizarEstadoAsync(Guid clienteId, ActualizarEstadoClienteSolicitud solicitud, CancellationToken tokenCancelacion);

    /// <summary>Elimina un cliente existente.</summary>
    Task EliminarAsync(Guid clienteId, CancellationToken tokenCancelacion);

    /// <summary>Republica todos los clientes para reconstruir proyecciones de otros servicios.</summary>
    Task SincronizarTodosAsync(CancellationToken tokenCancelacion);
}
