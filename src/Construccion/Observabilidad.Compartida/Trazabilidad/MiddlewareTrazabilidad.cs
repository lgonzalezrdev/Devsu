using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Observabilidad.Compartida.Trazabilidad;

/// <summary>Asigna y propaga un identificador de correlación para cada solicitud HTTP.</summary>
public sealed class MiddlewareTrazabilidad(RequestDelegate siguiente, ILogger<MiddlewareTrazabilidad> registrador)
{
    private const string EncabezadoCorrelacion = "X-Correlation-Id";
    private static readonly Meter Medidor = new("Devsu.Observabilidad", "1.0.0");
    private static readonly Counter<long> Solicitudes = Medidor.CreateCounter<long>("devsu.solicitudes.total");

    public async Task InvokeAsync(HttpContext contexto)
    {
        string idCorrelacion = contexto.Request.Headers[EncabezadoCorrelacion].FirstOrDefault()
            ?? Activity.Current?.TraceId.ToString()
            ?? Guid.NewGuid().ToString("N");
        contexto.TraceIdentifier = idCorrelacion;
        contexto.Response.Headers[EncabezadoCorrelacion] = idCorrelacion;
        using IDisposable? alcance = registrador.BeginScope(new Dictionary<string, object> { ["IdCorrelacion"] = idCorrelacion });
        Solicitudes.Add(1, new KeyValuePair<string, object?>("ruta", contexto.Request.Path.Value), new KeyValuePair<string, object?>("metodo", contexto.Request.Method));
        await siguiente(contexto);
    }
}
