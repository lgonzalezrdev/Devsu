namespace Contratos.Compartidos.Eventos;

/// <summary>Propaga la información mínima de un cliente necesaria en otros microservicios.</summary>
public sealed record ClienteSincronizado(
    Guid ClienteId,
    string Nombre,
    bool Estado,
    DateTime FechaOcurrencia);
