using Cuentas.Aplicacion.Contratos;
using MassTransit;

namespace Cuentas.Infraestructura.Mensajeria;

public sealed class PublicadorEventosMassTransit(IPublishEndpoint publicador) : IPublicadorEventosIntegracion
{
    public Task PublicarAsync<TEvento>(TEvento eventoIntegracion, CancellationToken tokenCancelacion)
        where TEvento : class => publicador.Publish(eventoIntegracion, tokenCancelacion);
}
