using System.Globalization;
using Clientes.Aplicacion.Contratos;
using Clientes.Aplicacion.Excepciones;
using Clientes.Aplicacion.Modelos;
using Clientes.Dominio.Entidades;

namespace Clientes.Aplicacion.Servicios;

public sealed class ServicioReportes(IRepositorioClientes repositorioClientes, IRepositorioReportes repositorioReportes) : IServicioReportes
{
    private const string FormatoFecha = "yyyy-MM-dd HH:mm:ss";

    public async Task<ReporteEstadoCuentaRespuesta> ObtenerEstadoCuentaAsync(Guid clienteId, string fecha, CancellationToken tokenCancelacion)
    {
        Cliente cliente = await repositorioClientes.ObtenerPorIdAsync(clienteId, tokenCancelacion)
            ?? throw new ClienteNoEncontradoException(clienteId);
        (DateTime fechaInicio, DateTime fechaFin) = ObtenerRangoFechas(fecha);
        IReadOnlyCollection<CuentaReporte> cuentas = await repositorioReportes.ObtenerCuentasPorClienteAsync(clienteId, tokenCancelacion);
        IReadOnlyCollection<Guid> cuentasId = cuentas.Select(cuenta => cuenta.CuentaId).ToArray();
        IReadOnlyCollection<MovimientoReporte> movimientos = await repositorioReportes.ObtenerMovimientosAsync(cuentasId, fechaInicio, fechaFin, tokenCancelacion);
        IReadOnlyCollection<CuentaEstadoCuentaRespuesta> cuentasRespuesta = cuentas
            .Select(cuenta => CuentaEstadoCuentaRespuesta.DesdeEntidad(
                cuenta,
                movimientos.Where(movimiento => movimiento.CuentaId == cuenta.CuentaId).ToArray()))
            .ToArray();

        return new ReporteEstadoCuentaRespuesta(cliente.Nombre, fechaInicio, fechaFin, cuentasRespuesta);
    }

    private static (DateTime FechaInicio, DateTime FechaFin) ObtenerRangoFechas(string fecha)
    {
        string[] partes = fecha.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (partes.Length != 2
            || !DateTime.TryParseExact(partes[0], FormatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaInicio)
            || !DateTime.TryParseExact(partes[1], FormatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaFin)
            || fechaInicio > fechaFin)
        {
            throw new ConsultaReporteInvalidaException("El parámetro fecha debe tener el formato 'yyyy-MM-dd HH:mm:ss,yyyy-MM-dd HH:mm:ss' y un rango válido.");
        }

        return (fechaInicio, fechaFin);
    }
}
