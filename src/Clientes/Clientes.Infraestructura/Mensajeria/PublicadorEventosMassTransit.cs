using Clientes.Aplicacion.Contratos;
using MassTransit;

namespace Clientes.Infraestructura.Mensajeria;

public sealed class PublicadorEventosMassTransit(IPublishEndpoint publicador) : IPublicadorEventosIntegracion
{
    public Task PublicarAsync<TEvento>(TEvento eventoIntegracion, CancellationToken tokenCancelacion)
        where TEvento : class => publicador.Publish(eventoIntegracion, tokenCancelacion);
}
