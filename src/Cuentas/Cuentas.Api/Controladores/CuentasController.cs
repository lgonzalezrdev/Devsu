using Cuentas.Aplicacion.Modelos;
using Cuentas.Aplicacion.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Cuentas.Api.Controladores;

[ApiController]
[Route("api/cuentas")]
public sealed class CuentasController(IServicioCuentas servicioCuentas) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ResultadoPaginado<CuentaRespuesta>>> ObtenerTodos([FromQuery] ConsultaCuentas consulta, CancellationToken tokenCancelacion) => Ok(await servicioCuentas.ObtenerCuentasPaginadasAsync(consulta, tokenCancelacion));

    [HttpGet("{cuentaId:guid}")]
    public async Task<ActionResult<CuentaRespuesta>> ObtenerPorId(Guid cuentaId, CancellationToken tokenCancelacion) => Ok(await servicioCuentas.ObtenerCuentaAsync(cuentaId, tokenCancelacion));

    [HttpPost]
    public async Task<ActionResult<CuentaRespuesta>> Crear(CrearCuentaSolicitud solicitud, CancellationToken tokenCancelacion)
    {
        CuentaRespuesta cuenta = await servicioCuentas.CrearCuentaAsync(solicitud, tokenCancelacion);
        return CreatedAtAction(nameof(ObtenerPorId), new { cuentaId = cuenta.CuentaId }, cuenta);
    }

    [HttpPut("{cuentaId:guid}")]
    public async Task<IActionResult> Actualizar(Guid cuentaId, ActualizarCuentaSolicitud solicitud, CancellationToken tokenCancelacion)
    {
        await servicioCuentas.ActualizarCuentaAsync(cuentaId, solicitud, tokenCancelacion);
        return NoContent();
    }

    [HttpPatch("{cuentaId:guid}/estado")]
    public async Task<IActionResult> ActualizarEstado(Guid cuentaId, ActualizarEstadoCuentaSolicitud solicitud, CancellationToken tokenCancelacion)
    {
        await servicioCuentas.ActualizarEstadoCuentaAsync(cuentaId, solicitud, tokenCancelacion);
        return NoContent();
    }

    [HttpPost("sincronizar")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SincronizarTodos(CancellationToken tokenCancelacion)
    {
        await servicioCuentas.SincronizarTodosAsync(tokenCancelacion);
        return Accepted();
    }
}
