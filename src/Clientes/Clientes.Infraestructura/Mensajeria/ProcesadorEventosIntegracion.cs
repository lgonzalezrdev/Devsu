using Clientes.Infraestructura.Persistencia;
using Contratos.Compartidos.Eventos;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Observabilidad.Compartida.Trazabilidad;
using System.Text.Json;

namespace Clientes.Infraestructura.Mensajeria;

public sealed class ProcesadorEventosIntegracion(IServiceScopeFactory fabricaAlcances) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await ProcesarAsync(stoppingToken); }
            catch (Exception) when (!stoppingToken.IsCancellationRequested) { }
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }

    private async Task ProcesarAsync(CancellationToken tokenCancelacion)
    {
        using IServiceScope alcance = fabricaAlcances.CreateScope();
        ContextoClientes contextoClientes = alcance.ServiceProvider.GetRequiredService<ContextoClientes>();
        IPublishEndpoint publicador = alcance.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        List<EventoIntegracion> eventos = await contextoClientes.EventosIntegracion.Where(evento => evento.ProcesadoEn == null).OrderBy(evento => evento.OcurridoEn).Take(20).ToListAsync(tokenCancelacion);
        foreach (EventoIntegracion evento in eventos)
        {
            try
            {
                await PublicarAsync(evento, publicador, tokenCancelacion);
                evento.MarcarProcesado();
                MetricasMensajeria.RegistrarPublicado(evento.Tipo);
            }
            catch (Exception excepcion)
            {
                evento.RegistrarFallo(excepcion);
                MetricasMensajeria.RegistrarFallo(evento.Tipo);
            }
        }
        await contextoClientes.SaveChangesAsync(tokenCancelacion);
    }

    private static Task PublicarAsync(EventoIntegracion evento, IPublishEndpoint publicador, CancellationToken tokenCancelacion) =>
        evento.Tipo.EndsWith(nameof(ClienteSincronizado), StringComparison.Ordinal)
            ? publicador.Publish(JsonSerializer.Deserialize<ClienteSincronizado>(evento.Contenido) ?? throw new InvalidOperationException("Evento inválido."), tokenCancelacion)
            : publicador.Publish(JsonSerializer.Deserialize<ClienteEliminado>(evento.Contenido) ?? throw new InvalidOperationException("Evento inválido."), tokenCancelacion);
}
