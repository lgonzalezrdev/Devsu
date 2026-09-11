using Contratos.Compartidos.Eventos;
using Cuentas.Infraestructura.Persistencia;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Observabilidad.Compartida.Trazabilidad;
using System.Text.Json;

namespace Cuentas.Infraestructura.Mensajeria;

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
        ContextoCuentas contextoCuentas = alcance.ServiceProvider.GetRequiredService<ContextoCuentas>();
        IPublishEndpoint publicador = alcance.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        List<EventoIntegracion> eventos = await contextoCuentas.EventosIntegracion.Where(evento => evento.ProcesadoEn == null).OrderBy(evento => evento.OcurridoEn).Take(20).ToListAsync(tokenCancelacion);
        foreach (EventoIntegracion evento in eventos)
        {
            try
            {
                await publicador.Publish(JsonSerializer.Deserialize<CuentaReporteSincronizada>(evento.Contenido) ?? throw new InvalidOperationException("Evento inválido."), tokenCancelacion);
                evento.MarcarProcesado();
                MetricasMensajeria.RegistrarPublicado(evento.Tipo);
            }
            catch (Exception excepcion)
            {
                evento.RegistrarFallo(excepcion);
                MetricasMensajeria.RegistrarFallo(evento.Tipo);
            }
        }
        await contextoCuentas.SaveChangesAsync(tokenCancelacion);
    }
}
