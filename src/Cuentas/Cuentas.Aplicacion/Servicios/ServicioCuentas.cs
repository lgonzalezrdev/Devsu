using Cuentas.Aplicacion.Contratos;
using Cuentas.Aplicacion.Excepciones;
using Cuentas.Aplicacion.Modelos;
using Cuentas.Dominio.Entidades;
using Cuentas.Dominio.Excepciones;
using Contratos.Compartidos.Eventos;
using System.Diagnostics;

namespace Cuentas.Aplicacion.Servicios;

public sealed class ServicioCuentas(
    IRepositorioCuentas repositorioCuentas,
    IGeneradorNumeroCuenta generadorNumeroCuenta,
    IRepositorioClientesIntegracion repositorioClientes,
    IPublicadorEventosIntegracion publicadorEventos) : IServicioCuentas
{
    public async Task<IReadOnlyCollection<CuentaRespuesta>> ObtenerCuentasAsync(CancellationToken tokenCancelacion)
    {
        IReadOnlyCollection<Cuenta> cuentas = await repositorioCuentas.ObtenerCuentasAsync(tokenCancelacion);
        return cuentas.Select(CuentaRespuesta.DesdeEntidad).ToArray();
    }

    public async Task<CuentaRespuesta> ObtenerCuentaAsync(Guid cuentaId, CancellationToken tokenCancelacion) =>
        CuentaRespuesta.DesdeEntidad(await ObtenerCuentaRequeridaAsync(cuentaId, tokenCancelacion));

    public async Task<CuentaRespuesta> CrearCuentaAsync(CrearCuentaSolicitud solicitud, CancellationToken tokenCancelacion)
    {
        ClienteIntegracion? cliente = await repositorioClientes.ObtenerPorIdAsync(solicitud.ClienteId, tokenCancelacion);

        if (cliente is null)
        {
            throw new ClienteNoDisponibleException("El cliente indicado no existe o aún no se ha sincronizado. Espere unos segundos e intente nuevamente.");
        }

        if (!cliente.Estado)
        {
            throw new ClienteNoDisponibleException("No se puede crear una cuenta para un cliente inactivo.");
        }

        string numeroCuenta = await GenerarNumeroCuentaUnicoAsync(tokenCancelacion);
        Cuenta cuenta = new(Guid.NewGuid(), solicitud.ClienteId, numeroCuenta, solicitud.TipoCuenta, solicitud.SaldoInicial);
        await repositorioCuentas.AgregarCuentaAsync(cuenta, tokenCancelacion);
        await PublicarCuentaReporteAsync(cuenta, tokenCancelacion);
        await repositorioCuentas.GuardarCambiosAsync(tokenCancelacion);
        return CuentaRespuesta.DesdeEntidad(cuenta);
    }

    public async Task ActualizarCuentaAsync(Guid cuentaId, ActualizarCuentaSolicitud solicitud, CancellationToken tokenCancelacion)
    {
        Cuenta cuenta = await ObtenerCuentaRequeridaAsync(cuentaId, tokenCancelacion);
        cuenta.Actualizar(solicitud.TipoCuenta);
        await PublicarCuentaReporteAsync(cuenta, tokenCancelacion);
        await repositorioCuentas.GuardarCambiosAsync(tokenCancelacion);
    }

    public async Task ActualizarEstadoCuentaAsync(Guid cuentaId, ActualizarEstadoCuentaSolicitud solicitud, CancellationToken tokenCancelacion)
    {
        Cuenta cuenta = await ObtenerCuentaRequeridaAsync(cuentaId, tokenCancelacion);
        cuenta.CambiarEstado(solicitud.Estado);
        await PublicarCuentaReporteAsync(cuenta, tokenCancelacion);
        await repositorioCuentas.GuardarCambiosAsync(tokenCancelacion);
    }

    public async Task<IReadOnlyCollection<MovimientoRespuesta>> ObtenerMovimientosAsync(Guid cuentaId, CancellationToken tokenCancelacion)
    {
        await ObtenerCuentaRequeridaAsync(cuentaId, tokenCancelacion);
        IReadOnlyCollection<Movimiento> movimientos = await repositorioCuentas.ObtenerMovimientosAsync(cuentaId, tokenCancelacion);
        return movimientos.Select(MovimientoRespuesta.DesdeEntidad).ToArray();
    }

    public async Task<MovimientoRespuesta> CrearMovimientoAsync(CrearMovimientoSolicitud solicitud, CancellationToken tokenCancelacion)
    {
        Cuenta cuenta = await ObtenerCuentaRequeridaAsync(solicitud.CuentaId, tokenCancelacion);
        Movimiento movimiento = cuenta.RegistrarMovimiento(DateTime.Now, solicitud.TipoMovimiento, solicitud.Valor);
        await repositorioCuentas.AgregarMovimientoAsync(movimiento, tokenCancelacion);
        await PublicarCuentaReporteAsync(cuenta, tokenCancelacion, movimiento);
        await repositorioCuentas.GuardarCambiosAsync(tokenCancelacion);
        return MovimientoRespuesta.DesdeEntidad(movimiento);
    }

    public async Task ActualizarMovimientoAsync(Guid movimientoId, ActualizarMovimientoSolicitud solicitud, CancellationToken tokenCancelacion)
    {
        Movimiento movimiento = await repositorioCuentas.ObtenerMovimientoPorIdAsync(movimientoId, tokenCancelacion)
            ?? throw new RecursoNoEncontradoException("No existe el movimiento solicitado.");
        Cuenta cuenta = await ObtenerCuentaRequeridaAsync(movimiento.CuentaId, tokenCancelacion);
        movimiento.Actualizar(solicitud.TipoMovimiento, solicitud.Valor);
        IReadOnlyCollection<Movimiento> movimientos = await repositorioCuentas.ObtenerMovimientosAsync(cuenta.CuentaId, tokenCancelacion);
        cuenta.RecalcularSaldos(movimientos);
        await PublicarCuentaReporteAsync(cuenta, tokenCancelacion);
        await repositorioCuentas.GuardarCambiosAsync(tokenCancelacion);
    }

    public async Task SincronizarTodosAsync(CancellationToken tokenCancelacion)
    {
        IReadOnlyCollection<Cuenta> cuentas = await repositorioCuentas.ObtenerCuentasAsync(tokenCancelacion);

        foreach (Cuenta cuenta in cuentas)
        {
            await PublicarCuentaReporteAsync(cuenta, tokenCancelacion);
        }
        await repositorioCuentas.GuardarCambiosAsync(tokenCancelacion);
    }

    private async Task<Cuenta> ObtenerCuentaRequeridaAsync(Guid cuentaId, CancellationToken tokenCancelacion) =>
        await repositorioCuentas.ObtenerCuentaPorIdAsync(cuentaId, tokenCancelacion)
        ?? throw new RecursoNoEncontradoException("No existe la cuenta solicitada.");

    private async Task<string> GenerarNumeroCuentaUnicoAsync(CancellationToken tokenCancelacion)
    {
        const int maximosIntentos = 10;

        for (int intento = 0; intento < maximosIntentos; intento++)
        {
            string numeroCuenta = generadorNumeroCuenta.Generar();
            bool existe = await repositorioCuentas.ExisteNumeroCuentaAsync(numeroCuenta, tokenCancelacion);

            if (!existe)
            {
                return numeroCuenta;
            }
        }

        throw new ExcepcionReglaDominioException("No fue posible generar un número de cuenta único. Intente nuevamente.");
    }

    private async Task PublicarCuentaReporteAsync(Cuenta cuenta, CancellationToken tokenCancelacion, Movimiento? movimientoPendiente = null)
    {
        IReadOnlyCollection<Movimiento> movimientos = await repositorioCuentas.ObtenerMovimientosAsync(cuenta.CuentaId, tokenCancelacion);
        IReadOnlyCollection<Movimiento> movimientosCompletos = movimientoPendiente is null || movimientos.Any(movimiento => movimiento.MovimientoId == movimientoPendiente.MovimientoId)
            ? movimientos
            : movimientos.Append(movimientoPendiente).ToArray();
        IReadOnlyCollection<MovimientoReporteSincronizado> movimientosIntegracion = movimientosCompletos
            .Select(movimiento => new MovimientoReporteSincronizado(
                movimiento.MovimientoId,
                movimiento.Fecha,
                movimiento.TipoMovimiento.ToString(),
                movimiento.Valor,
                movimiento.Saldo))
            .ToArray();
        CuentaReporteSincronizada eventoIntegracion = new(
            Guid.NewGuid(),
            cuenta.CuentaId,
            cuenta.ClienteId,
            cuenta.NumeroCuenta,
            cuenta.TipoCuenta.ToString(),
            cuenta.SaldoInicial,
            cuenta.SaldoDisponible,
            cuenta.Estado,
            DateTime.UtcNow,
            movimientosIntegracion,
            Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N"));
        await publicadorEventos.RegistrarAsync(eventoIntegracion, tokenCancelacion);
    }
}
