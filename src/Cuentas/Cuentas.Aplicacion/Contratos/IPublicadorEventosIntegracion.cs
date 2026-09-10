namespace Cuentas.Aplicacion.Contratos;

/// <summary>Publica eventos de Cuentas sin acoplar la aplicación al transporte.</summary>
public interface IPublicadorEventosIntegracion
{
    /// <summary>Publica un evento para que sea procesado por otros microservicios.</summary>
    Task PublicarAsync<TEvento>(TEvento eventoIntegracion, CancellationToken tokenCancelacion)
        where TEvento : class;
}
