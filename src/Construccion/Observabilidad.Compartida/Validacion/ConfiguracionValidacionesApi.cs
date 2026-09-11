using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Observabilidad.Compartida.Validacion;

/// <summary>Centraliza los mensajes de enlace de modelos y JSON para que las APIs respondan siempre en español.</summary>
public static class ConfiguracionValidacionesApi
{
    public static void ConfigurarMensajesModelBinding(MvcOptions opciones)
    {
        opciones.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"El valor '{valor}' no es válido para el campo {campo}.");
        opciones.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(campo => $"El campo {campo} es obligatorio.");
        opciones.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => "Se requiere un valor.");
        opciones.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
        opciones.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(campo => $"El valor enviado para {campo} no es válido.");
        opciones.ModelBindingMessageProvider.SetValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
        opciones.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(campo => $"El campo {campo} debe ser numérico.");
        opciones.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(campo => $"El campo {campo} es obligatorio.");
    }

    public static BadRequestObjectResult CrearRespuestaError(ActionContext contextoAccion)
    {
        Dictionary<string, string[]> errores = contextoAccion.ModelState.ToDictionary(
            entrada => entrada.Key,
            entrada => entrada.Value?.Errors.Select(error => ObtenerMensaje(entrada.Key, error)).ToArray() ?? []);
        ValidationProblemDetails problema = new(errores)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Los datos de la solicitud no son válidos.",
            Extensions = { ["idTrazabilidad"] = contextoAccion.HttpContext.TraceIdentifier }
        };
        return new BadRequestObjectResult(problema);
    }

    private static string ObtenerMensaje(string campo, ModelError error)
    {
        if (error.Exception is JsonException excepcionJson && excepcionJson.Message.StartsWith("El ", StringComparison.Ordinal))
        {
            return excepcionJson.Message;
        }

        if (campo.Equals("$.clienteId", StringComparison.OrdinalIgnoreCase))
        {
            return "El campo clienteId debe tener un GUID válido.";
        }

        if (campo.Equals("$.genero", StringComparison.OrdinalIgnoreCase))
        {
            return "El género debe ser Masculino, Femenino, Otro o NoEspecificado.";
        }

        if (campo.Equals("$.tipoCuenta", StringComparison.OrdinalIgnoreCase))
        {
            return "El tipo de cuenta debe ser Ahorros o Corriente.";
        }

        if (campo.Equals("$.tipoMovimiento", StringComparison.OrdinalIgnoreCase))
        {
            return "El tipo de movimiento debe ser Deposito o Retiro.";
        }

        if (error.Exception is JsonException
            || error.Exception?.Message.Contains("JSON", StringComparison.OrdinalIgnoreCase) == true
            || error.ErrorMessage.Contains("JSON", StringComparison.OrdinalIgnoreCase)
            || campo == "$")
        {
            return "El contenido JSON no es válido.";
        }

        if (!string.IsNullOrWhiteSpace(error.ErrorMessage) && !error.ErrorMessage.StartsWith("The ", StringComparison.OrdinalIgnoreCase))
        {
            return error.ErrorMessage;
        }

        return campo.Equals("solicitud", StringComparison.OrdinalIgnoreCase)
            ? "El cuerpo de la solicitud es obligatorio."
            : $"El valor enviado para el campo {campo.TrimStart('$', '.')} no es válido.";
    }
}
