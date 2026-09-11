using Clientes.Aplicacion.Contratos;
using Clientes.Infraestructura.Persistencia;
using Contratos.Compartidos.Eventos;
using System.Text.Json;

namespace Clientes.Infraestructura.Mensajeria;

public sealed class PublicadorEventosMassTransit(ContextoClientes contextoClientes) : IPublicadorEventosIntegracion
{
    public Task RegistrarAsync<TEvento>(TEvento eventoIntegracion, CancellationToken tokenCancelacion)
        where TEvento : class
    {
        Guid eventoId = eventoIntegracion switch { ClienteSincronizado cliente => cliente.EventoId, ClienteEliminado cliente => cliente.EventoId, _ => throw new InvalidOperationException("Tipo de evento de Clientes no admitido.") };
        EventoIntegracion evento = new(eventoId, typeof(TEvento).FullName ?? typeof(TEvento).Name, JsonSerializer.Serialize(eventoIntegracion), DateTime.UtcNow);
        contextoClientes.EventosIntegracion.Add(evento);
        return Task.CompletedTask;
    }
}
