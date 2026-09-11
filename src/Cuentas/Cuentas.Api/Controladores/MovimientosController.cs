using Cuentas.Aplicacion.Modelos;
using Cuentas.Aplicacion.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Cuentas.Api.Controladores;

[ApiController]
[Route("api/movimientos")]
public sealed class MovimientosController(IServicioCuentas servicioCuentas) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ResultadoPaginado<MovimientoRespuesta>>> ObtenerPorCuenta([FromQuery] Guid cuentaId, [FromQuery] ConsultaMovimientos consulta, CancellationToken tokenCancelacion) => Ok(await servicioCuentas.ObtenerMovimientosPaginadosAsync(cuentaId, consulta, tokenCancelacion));

    [HttpPost]
    public async Task<ActionResult<MovimientoRespuesta>> Crear(CrearMovimientoSolicitud solicitud, CancellationToken tokenCancelacion)
    {
        MovimientoRespuesta movimiento = await servicioCuentas.CrearMovimientoAsync(solicitud, tokenCancelacion);
        return Created($"api/movimientos/{movimiento.MovimientoId}", movimiento);
    }

    [HttpPut("{movimientoId:guid}")]
    public async Task<IActionResult> Actualizar(Guid movimientoId, ActualizarMovimientoSolicitud solicitud, CancellationToken tokenCancelacion)
    {
        await servicioCuentas.ActualizarMovimientoAsync(movimientoId, solicitud, tokenCancelacion);
        return NoContent();
    }
}
