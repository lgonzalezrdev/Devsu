using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cuentas.Api.Serializacion;

public sealed class ConvertidorFechaHora : JsonConverter<DateTime>
{
    private const string FormatoFechaHora = "yyyy-MM-dd HH:mm:ss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string textoFecha = reader.GetString() ?? string.Empty;

        if (!DateTime.TryParseExact(textoFecha, FormatoFechaHora, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha))
        {
            throw new JsonException("La fecha debe tener el formato yyyy-MM-dd HH:mm:ss.");
        }

        return fecha;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString(FormatoFechaHora, CultureInfo.InvariantCulture));
}
