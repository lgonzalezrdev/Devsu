using Clientes.Aplicacion.Contratos;
using Clientes.Dominio.Entidades;
using Contratos.Compartidos.Eventos;
using MassTransit;

namespace Clientes.Infraestructura.Mensajeria.Consumidores;

public sealed class ConsumidorCuentaReporteSincronizada(IRepositorioReportes repositorioReportes) : IConsumer<CuentaReporteSincronizada>
{
    public async Task Consume(ConsumeContext<CuentaReporteSincronizada> context)
    {
        CuentaReporteSincronizada eventoIntegracion = context.Message;
        CuentaReporte? cuenta = await repositorioReportes.ObtenerCuentaPorIdAsync(eventoIntegracion.CuentaId, context.CancellationToken);
        bool eventoAplicable = true;

        if (cuenta is null)
        {
            CuentaReporte nuevaCuenta = new(
                eventoIntegracion.CuentaId,
                eventoIntegracion.ClienteId,
                eventoIntegracion.NumeroCuenta,
                eventoIntegracion.TipoCuenta,
                eventoIntegracion.SaldoInicial,
                eventoIntegracion.SaldoDisponible,
                eventoIntegracion.Estado,
                eventoIntegracion.FechaOcurrencia);
            await repositorioReportes.AgregarCuentaAsync(nuevaCuenta, context.CancellationToken);
        }
        else
        {
            eventoAplicable = cuenta.Actualizar(
                eventoIntegracion.NumeroCuenta,
                eventoIntegracion.TipoCuenta,
                eventoIntegracion.SaldoInicial,
                eventoIntegracion.SaldoDisponible,
                eventoIntegracion.Estado,
                eventoIntegracion.FechaOcurrencia);
        }

        if (eventoAplicable)
        {
            foreach (MovimientoReporteSincronizado movimientoIntegracion in eventoIntegracion.Movimientos)
            {
                MovimientoReporte? movimiento = await repositorioReportes.ObtenerMovimientoPorIdAsync(movimientoIntegracion.MovimientoId, context.CancellationToken);

                if (movimiento is null)
                {
                    MovimientoReporte nuevoMovimiento = new(
                        movimientoIntegracion.MovimientoId,
                        eventoIntegracion.CuentaId,
                        movimientoIntegracion.Fecha,
                        movimientoIntegracion.TipoMovimiento,
                        movimientoIntegracion.Valor,
                        movimientoIntegracion.Saldo);
                    await repositorioReportes.AgregarMovimientoAsync(nuevoMovimiento, context.CancellationToken);
                }
                else
                {
                    movimiento.Actualizar(
                        movimientoIntegracion.Fecha,
                        movimientoIntegracion.TipoMovimiento,
                        movimientoIntegracion.Valor,
                        movimientoIntegracion.Saldo);
                }
            }
        }

        await repositorioReportes.GuardarCambiosAsync(context.CancellationToken);
    }
}
