using Cuentas.Aplicacion.Contratos;
using Cuentas.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Cuentas.Infraestructura.Persistencia;

public sealed class RepositorioClientesIntegracion(ContextoCuentas contextoCuentas) : IRepositorioClientesIntegracion
{
    public Task<ClienteIntegracion?> ObtenerPorIdAsync(Guid clienteId, CancellationToken tokenCancelacion) =>
        contextoCuentas.ClientesIntegracion.SingleOrDefaultAsync(cliente => cliente.ClienteId == clienteId, tokenCancelacion);

    public Task AgregarAsync(ClienteIntegracion cliente, CancellationToken tokenCancelacion) =>
        contextoCuentas.ClientesIntegracion.AddAsync(cliente, tokenCancelacion).AsTask();

    public void Eliminar(ClienteIntegracion cliente) => contextoCuentas.ClientesIntegracion.Remove(cliente);

    public Task GuardarCambiosAsync(CancellationToken tokenCancelacion) => contextoCuentas.SaveChangesAsync(tokenCancelacion);
}
