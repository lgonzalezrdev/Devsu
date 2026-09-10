using Clientes.Aplicacion.Contratos;

namespace Clientes.Infraestructura.Mensajeria;

/// <summary>Permite ejecutar Clientes localmente cuando el transporte de eventos está desactivado.</summary>
public sealed class PublicadorEventosNulo : IPublicadorEventosIntegracion
{
    public Task PublicarAsync<TEvento>(TEvento eventoIntegracion, CancellationToken tokenCancelacion)
        where TEvento : class => Task.CompletedTask;
}
