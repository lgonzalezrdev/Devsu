using Clientes.Aplicacion.Modelos;
using Clientes.Aplicacion.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Clientes.Api.Controladores;

[ApiController]
[Route("api/clientes")]
public sealed class ClientesController(IServicioClientes servicioClientes) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<ClienteRespuesta>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ClienteRespuesta>>> ObtenerTodos(CancellationToken tokenCancelacion) =>
        Ok(await servicioClientes.ObtenerTodosAsync(tokenCancelacion));

    [HttpGet("{clienteId:guid}")]
    [ProducesResponseType<ClienteRespuesta>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteRespuesta>> ObtenerPorId(Guid clienteId, CancellationToken tokenCancelacion) =>
        Ok(await servicioClientes.ObtenerPorIdAsync(clienteId, tokenCancelacion));

    [HttpPost]
    [ProducesResponseType<ClienteRespuesta>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClienteRespuesta>> Crear(
        CrearClienteSolicitud solicitud,
        CancellationToken tokenCancelacion)
    {
        ClienteRespuesta cliente = await servicioClientes.CrearAsync(solicitud, tokenCancelacion);
        return CreatedAtAction(nameof(ObtenerPorId), new { clienteId = cliente.ClienteId }, cliente);
    }

    [HttpPut("{clienteId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(
        Guid clienteId,
        ActualizarClienteSolicitud solicitud,
        CancellationToken tokenCancelacion)
    {
        await servicioClientes.ActualizarAsync(clienteId, solicitud, tokenCancelacion);
        return NoContent();
    }

    [HttpPatch("{clienteId:guid}/estado")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarEstado(
        Guid clienteId,
        ActualizarEstadoClienteSolicitud solicitud,
        CancellationToken tokenCancelacion)
    {
        await servicioClientes.ActualizarEstadoAsync(clienteId, solicitud, tokenCancelacion);
        return NoContent();
    }

    [HttpDelete("{clienteId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Eliminar(Guid clienteId, CancellationToken tokenCancelacion)
    {
        await servicioClientes.EliminarAsync(clienteId, tokenCancelacion);
        return NoContent();
    }

    [HttpPost("sincronizar")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SincronizarTodos(CancellationToken tokenCancelacion)
    {
        await servicioClientes.SincronizarTodosAsync(tokenCancelacion);
        return Accepted();
    }
}
