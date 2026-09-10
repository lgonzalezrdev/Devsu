using Clientes.Aplicacion.Contratos;
using Clientes.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Infraestructura.Persistencia;

public sealed class RepositorioClientes(ContextoClientes contextoClientes) : IRepositorioClientes
{
    public async Task<IReadOnlyCollection<Cliente>> ObtenerTodosAsync(CancellationToken tokenCancelacion) =>
        await contextoClientes.Clientes
            .AsNoTracking()
            .OrderBy(cliente => cliente.Nombre)
            .ToArrayAsync(tokenCancelacion);

    public Task<Cliente?> ObtenerPorIdAsync(Guid clienteId, CancellationToken tokenCancelacion) =>
        contextoClientes.Clientes.SingleOrDefaultAsync(cliente => cliente.PersonaId == clienteId, tokenCancelacion);

    public Task AgregarAsync(Cliente cliente, CancellationToken tokenCancelacion) =>
        contextoClientes.Clientes.AddAsync(cliente, tokenCancelacion).AsTask();

    public void Eliminar(Cliente cliente) => contextoClientes.Clientes.Remove(cliente);

    public Task GuardarCambiosAsync(CancellationToken tokenCancelacion) =>
        contextoClientes.SaveChangesAsync(tokenCancelacion);
}
