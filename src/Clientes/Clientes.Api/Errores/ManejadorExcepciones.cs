using Clientes.Aplicacion.Excepciones;
using Clientes.Dominio.Excepciones;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Api.Errores;

public sealed partial class ManejadorExcepciones(
    ILogger<ManejadorExcepciones> registrador,
    IHostEnvironment ambiente) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        int codigoEstado = StatusCodes.Status500InternalServerError;
        string titulo = "Error interno del servidor";

        if (exception is ClienteNoEncontradoException)
        {
            codigoEstado = StatusCodes.Status404NotFound;
            titulo = "Cliente no encontrado";
        }
        else if (exception is ExcepcionReglaDominioException)
        {
            codigoEstado = StatusCodes.Status400BadRequest;
            titulo = "Regla de negocio no cumplida";
        }
        else if (exception is ConsultaReporteInvalidaException)
        {
            codigoEstado = StatusCodes.Status400BadRequest;
            titulo = "Consulta de reporte inválida";
        }
        else if (exception is DbUpdateException)
        {
            codigoEstado = StatusCodes.Status409Conflict;
            titulo = "Conflicto de persistencia";
        }

        RegistrarError(registrador, exception, codigoEstado, exception.Message);

        await Results.Problem(new ProblemDetails
        {
            Status = codigoEstado,
            Title = titulo,
            Detail = codigoEstado == StatusCodes.Status500InternalServerError
                ? ambiente.IsDevelopment()
                    ? exception.Message
                    : "Ocurrió un error inesperado. Consulte el identificador de trazabilidad."
                : exception.Message,
            Extensions = { ["idTrazabilidad"] = httpContext.TraceIdentifier }
        }).ExecuteAsync(httpContext);

        return true;
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Error HTTP {CodigoEstado}: {Mensaje}")]
    private static partial void RegistrarError(ILogger registrador, Exception exception, int codigoEstado, string mensaje);
}
