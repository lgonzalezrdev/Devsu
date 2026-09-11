namespace Cuentas.Aplicacion.Contratos;

/// <summary>Registra eventos de Cuentas en una cola transaccional sin acoplar la aplicación al transporte.</summary>
public interface IPublicadorEventosIntegracion
{
    /// <summary>Deja un evento pendiente para que el proceso de salida lo publique de forma confiable.</summary>
    Task RegistrarAsync<TEvento>(TEvento eventoIntegracion, CancellationToken tokenCancelacion)
        where TEvento : class;
}
