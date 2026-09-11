using Clientes.Aplicacion.Contratos;
using Clientes.Dominio.Entidades;
using Clientes.Aplicacion.Modelos;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Infraestructura.Persistencia;

public sealed class RepositorioClientes(ContextoClientes contextoClientes) : IRepositorioClientes
{
    public async Task<IReadOnlyCollection<Cliente>> ObtenerTodosAsync(CancellationToken tokenCancelacion) =>
        await contextoClientes.Clientes
            .AsNoTracking()
            .OrderBy(cliente => cliente.Nombre)
            .ToArrayAsync(tokenCancelacion);

    public async Task<(IReadOnlyCollection<Cliente> Clientes, int TotalRegistros)> ObtenerPaginadoAsync(ConsultaClientes consulta, CancellationToken tokenCancelacion)
    {
        IQueryable<Cliente> consultaBase = contextoClientes.Clientes.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(consulta.Nombre)) { consultaBase = consultaBase.Where(cliente => cliente.Nombre.Contains(consulta.Nombre)); }
        if (consulta.Estado.HasValue) { consultaBase = consultaBase.Where(cliente => cliente.Estado == consulta.Estado.Value); }
        int totalRegistros = await consultaBase.CountAsync(tokenCancelacion);
        IReadOnlyCollection<Cliente> clientes = await consultaBase.OrderBy(cliente => cliente.Nombre).Skip((consulta.Pagina - 1) * consulta.TamanoPagina).Take(consulta.TamanoPagina).ToArrayAsync(tokenCancelacion);
        return (clientes, totalRegistros);
    }

    public Task<Cliente?> ObtenerPorIdAsync(Guid clienteId, CancellationToken tokenCancelacion) =>
        contextoClientes.Clientes.SingleOrDefaultAsync(cliente => cliente.PersonaId == clienteId, tokenCancelacion);

    public Task AgregarAsync(Cliente cliente, CancellationToken tokenCancelacion) =>
        contextoClientes.Clientes.AddAsync(cliente, tokenCancelacion).AsTask();

    public void Eliminar(Cliente cliente) => contextoClientes.Clientes.Remove(cliente);

    public Task GuardarCambiosAsync(CancellationToken tokenCancelacion) =>
        contextoClientes.SaveChangesAsync(tokenCancelacion);
}
