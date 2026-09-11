using Cuentas.Aplicacion.Contratos;
using Cuentas.Infraestructura.Persistencia;
using Contratos.Compartidos.Eventos;
using System.Text.Json;

namespace Cuentas.Infraestructura.Mensajeria;

public sealed class PublicadorEventosMassTransit(ContextoCuentas contextoCuentas) : IPublicadorEventosIntegracion
{
    public Task RegistrarAsync<TEvento>(TEvento eventoIntegracion, CancellationToken tokenCancelacion)
        where TEvento : class
    {
        if (eventoIntegracion is not CuentaReporteSincronizada cuenta)
        {
            throw new InvalidOperationException("Tipo de evento de Cuentas no admitido.");
        }
        EventoIntegracion evento = new(cuenta.EventoId, typeof(TEvento).FullName ?? typeof(TEvento).Name, JsonSerializer.Serialize(eventoIntegracion), DateTime.UtcNow);
        contextoCuentas.EventosIntegracion.Add(evento);
        return Task.CompletedTask;
    }
}
