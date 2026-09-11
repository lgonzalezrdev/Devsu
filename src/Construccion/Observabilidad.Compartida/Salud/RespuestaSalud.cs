using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Observabilidad.Compartida.Salud;

/// <summary>Genera una respuesta JSON uniforme para los endpoints de salud.</summary>
public static class RespuestaSalud
{
    public static Task EscribirAsync(HttpContext contexto, HealthReport reporte)
    {
        contexto.Response.ContentType = "application/json";
        ResultadoSalud respuesta = new(reporte.Status.ToString(), reporte.Entries.ToDictionary(entrada => entrada.Key, entrada => new DependenciaSalud(entrada.Value.Status.ToString(), entrada.Value.Description)));
        return contexto.Response.WriteAsJsonAsync(respuesta);
    }

    private sealed record ResultadoSalud(string Estado, IReadOnlyDictionary<string, DependenciaSalud> Dependencias);
    private sealed record DependenciaSalud(string Estado, string? Detalle);
}
