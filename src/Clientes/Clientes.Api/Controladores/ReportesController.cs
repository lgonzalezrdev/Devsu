using Clientes.Aplicacion.Modelos;
using Clientes.Aplicacion.Servicios;
using Clientes.Aplicacion.Excepciones;
using Microsoft.AspNetCore.Mvc;

namespace Clientes.Api.Controladores;

[ApiController]
[Route("api/reportes")]
public sealed class ReportesController(IServicioReportes servicioReportes) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<ReporteEstadoCuentaRespuesta>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReporteEstadoCuentaRespuesta>> ObtenerEstadoCuenta(
        [FromQuery(Name = "cliente")] string? cliente,
        [FromQuery] string? fecha,
        CancellationToken tokenCancelacion)
    {
        if (!Guid.TryParse(cliente, out Guid clienteId))
        {
            throw new ConsultaReporteInvalidaException("El parámetro cliente debe contener un identificador GUID válido.");
        }

        return Ok(await servicioReportes.ObtenerEstadoCuentaAsync(clienteId, fecha ?? string.Empty, tokenCancelacion));
    }
}
