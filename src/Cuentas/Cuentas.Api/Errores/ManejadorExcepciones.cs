using Cuentas.Aplicacion.Excepciones;
using Cuentas.Dominio.Excepciones;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cuentas.Api.Errores;

public sealed partial class ManejadorExcepciones(ILogger<ManejadorExcepciones> registrador, IHostEnvironment ambiente) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int codigoEstado = exception is SaldoNoDisponibleException ? StatusCodes.Status422UnprocessableEntity
            : exception is ClienteNoDisponibleException ? StatusCodes.Status409Conflict
            : exception is RecursoNoEncontradoException ? StatusCodes.Status404NotFound
            : exception is ExcepcionReglaDominioException ? StatusCodes.Status400BadRequest
            : exception is DbUpdateException ? StatusCodes.Status409Conflict
            : StatusCodes.Status500InternalServerError;
        string titulo = codigoEstado == StatusCodes.Status422UnprocessableEntity ? "Saldo no disponible"
            : exception is ClienteNoDisponibleException ? "Cliente no disponible"
            : codigoEstado == StatusCodes.Status404NotFound ? "Recurso no encontrado"
            : codigoEstado == StatusCodes.Status400BadRequest ? "Regla de negocio no cumplida"
            : codigoEstado == StatusCodes.Status409Conflict ? "Conflicto de persistencia" : "Error interno del servidor";
        RegistrarError(registrador, exception, codigoEstado, exception.Message);
        await Results.Problem(new ProblemDetails { Status = codigoEstado, Title = titulo, Detail = codigoEstado == 500 && !ambiente.IsDevelopment() ? "Ocurrió un error inesperado." : exception.Message }).ExecuteAsync(httpContext);
        return true;
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Error HTTP {CodigoEstado}: {Mensaje}")]
    private static partial void RegistrarError(ILogger registrador, Exception exception, int codigoEstado, string mensaje);
}
