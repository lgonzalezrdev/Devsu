using Cuentas.Dominio.Entidades;

namespace Cuentas.Aplicacion.Contratos;

/// <summary>Accede a la proyección local de clientes sincronizada por eventos.</summary>
public interface IRepositorioClientesIntegracion
{
    /// <summary>Obtiene un cliente previamente sincronizado por su identificador.</summary>
    Task<ClienteIntegracion?> ObtenerPorIdAsync(Guid clienteId, CancellationToken tokenCancelacion);

    /// <summary>Inserta una nueva proyección local de cliente.</summary>
    Task AgregarAsync(ClienteIntegracion cliente, CancellationToken tokenCancelacion);

    /// <summary>Elimina una proyección local cuando el cliente fue eliminado en su servicio origen.</summary>
    void Eliminar(ClienteIntegracion cliente);

    /// <summary>Confirma los cambios de la proyección local.</summary>
    Task GuardarCambiosAsync(CancellationToken tokenCancelacion);
}
