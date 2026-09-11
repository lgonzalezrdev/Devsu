using System.Diagnostics.Metrics;

namespace Observabilidad.Compartida.Trazabilidad;

/// <summary>Expone contadores de eventos publicados, consumidos y fallidos.</summary>
public static class MetricasMensajeria
{
    private static readonly Meter Medidor = new("Devsu.Mensajeria", "1.0.0");
    private static readonly Counter<long> EventosPublicados = Medidor.CreateCounter<long>("devsu.eventos.publicados.total");
    private static readonly Counter<long> EventosFallidos = Medidor.CreateCounter<long>("devsu.eventos.fallidos.total");

    public static void RegistrarPublicado(string tipoEvento) => EventosPublicados.Add(1, new KeyValuePair<string, object?>("tipo_evento", tipoEvento));
    public static void RegistrarFallo(string tipoEvento) => EventosFallidos.Add(1, new KeyValuePair<string, object?>("tipo_evento", tipoEvento));
}
