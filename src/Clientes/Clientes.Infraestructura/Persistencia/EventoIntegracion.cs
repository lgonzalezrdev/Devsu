namespace Clientes.Infraestructura.Persistencia;

public sealed class EventoIntegracion
{
    public EventoIntegracion(Guid eventoId, string tipo, string contenido, DateTime ocurridoEn)
    {
        EventoId = eventoId;
        Tipo = tipo;
        Contenido = contenido;
        OcurridoEn = ocurridoEn;
    }

    public Guid EventoId { get; }
    public string Tipo { get; }
    public string Contenido { get; }
    public DateTime OcurridoEn { get; }
    public DateTime? ProcesadoEn { get; private set; }
    public int Intentos { get; private set; }
    public string? Error { get; private set; }

    public void MarcarProcesado() => ProcesadoEn = DateTime.UtcNow;
    public void RegistrarFallo(Exception excepcion) { Intentos++; Error = excepcion.Message; }
}

public sealed class EventoIntegracionRecibido
{
    public EventoIntegracionRecibido(Guid eventoId, string tipo) { EventoId = eventoId; Tipo = tipo; RecibidoEn = DateTime.UtcNow; }
    public Guid EventoId { get; }
    public string Tipo { get; }
    public DateTime RecibidoEn { get; }
}
