using Clientes.Aplicacion.Modelos;

namespace Clientes.Aplicacion.Servicios;

/// <summary>Genera consultas de estado de cuenta desde proyecciones locales.</summary>
public interface IServicioReportes
{
    /// <summary>Genera el estado de cuenta de un cliente en el rango inclusivo solicitado.</summary>
    Task<ReporteEstadoCuentaRespuesta> ObtenerEstadoCuentaAsync(Guid clienteId, string fecha, CancellationToken tokenCancelacion);
}
