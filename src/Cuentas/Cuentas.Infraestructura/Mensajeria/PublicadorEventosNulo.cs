using Cuentas.Aplicacion.Contratos;

namespace Cuentas.Infraestructura.Mensajeria;

public sealed class PublicadorEventosNulo : IPublicadorEventosIntegracion
{
    public Task RegistrarAsync<TEvento>(TEvento eventoIntegracion, CancellationToken tokenCancelacion)
        where TEvento : class => Task.CompletedTask;
}
