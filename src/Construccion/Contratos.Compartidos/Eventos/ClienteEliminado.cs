namespace Contratos.Compartidos.Eventos;

/// <summary>Indica que la proyección local de un cliente debe ser retirada.</summary>
public sealed record ClienteEliminado(Guid EventoId, Guid ClienteId, DateTime FechaOcurrencia, string IdCorrelacion);
