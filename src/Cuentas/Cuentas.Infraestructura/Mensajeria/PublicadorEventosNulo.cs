using Cuentas.Aplicacion.Contratos;

namespace Cuentas.Infraestructura.Mensajeria;

public sealed class PublicadorEventosNulo : IPublicadorEventosIntegracion
{
    public Task PublicarAsync<TEvento>(TEvento eventoIntegracion, CancellationToken tokenCancelacion)
        where TEvento : class => Task.CompletedTask;
}
