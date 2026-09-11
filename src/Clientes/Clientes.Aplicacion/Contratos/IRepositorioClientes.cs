using Clientes.Dominio.Entidades;
using Clientes.Aplicacion.Modelos;

namespace Clientes.Aplicacion.Contratos;

public interface IRepositorioClientes
{
    /// <summary>Obtiene todos los clientes para consulta.</summary>
    Task<IReadOnlyCollection<Cliente>> ObtenerTodosAsync(CancellationToken tokenCancelacion);
    Task<(IReadOnlyCollection<Cliente> Clientes, int TotalRegistros)> ObtenerPaginadoAsync(ConsultaClientes consulta, CancellationToken tokenCancelacion);

    /// <summary>Busca un cliente por su identificador único.</summary>
    Task<Cliente?> ObtenerPorIdAsync(Guid clienteId, CancellationToken tokenCancelacion);

    /// <summary>Agrega un cliente al contexto de persistencia.</summary>
    Task AgregarAsync(Cliente cliente, CancellationToken tokenCancelacion);

    /// <summary>Marca un cliente para su eliminación física.</summary>
    void Eliminar(Cliente cliente);

    /// <summary>Confirma los cambios pendientes en la persistencia.</summary>
    Task GuardarCambiosAsync(CancellationToken tokenCancelacion);
}
