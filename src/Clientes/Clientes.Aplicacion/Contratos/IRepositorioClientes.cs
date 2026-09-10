using Clientes.Dominio.Entidades;

namespace Clientes.Aplicacion.Contratos;

public interface IRepositorioClientes
{
    Task<IReadOnlyCollection<Cliente>> ObtenerTodosAsync(CancellationToken tokenCancelacion);

    Task<Cliente?> ObtenerPorIdAsync(Guid clienteId, CancellationToken tokenCancelacion);

    Task AgregarAsync(Cliente cliente, CancellationToken tokenCancelacion);

    void Eliminar(Cliente cliente);

    Task GuardarCambiosAsync(CancellationToken tokenCancelacion);
}
