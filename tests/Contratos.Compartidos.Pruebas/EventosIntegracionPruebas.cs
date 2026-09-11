using Contratos.Compartidos.Eventos;
using System.Text.Json;
using Xunit;

namespace Contratos.Compartidos.Pruebas;

public sealed class EventosIntegracionPruebas
{
    [Fact]
    public void ClienteSincronizadoConservaIdentificadoresAlSerializar()
    {
        ClienteSincronizado eventoOriginal = new(Guid.NewGuid(), Guid.NewGuid(), "Jose Lema", true, new DateTime(2026, 1, 1), "correlacion-prueba");
        string contenido = JsonSerializer.Serialize(eventoOriginal);
        ClienteSincronizado eventoRecuperado = JsonSerializer.Deserialize<ClienteSincronizado>(contenido)!;
        Assert.Equal(eventoOriginal.EventoId, eventoRecuperado.EventoId);
        Assert.Equal(eventoOriginal.ClienteId, eventoRecuperado.ClienteId);
        Assert.Equal("correlacion-prueba", eventoRecuperado.IdCorrelacion);
    }
}
