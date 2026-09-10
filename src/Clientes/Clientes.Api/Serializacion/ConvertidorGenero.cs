using Clientes.Dominio.Enumeraciones;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Clientes.Api.Serializacion;

public sealed class ConvertidorGenero : JsonConverter<Genero>
{
    public override Genero Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("El género debe enviarse como texto: Masculino, Femenino, Otro o NoEspecificado.");
        }

        string textoGenero = reader.GetString() ?? string.Empty;
        bool esGeneroValido = Enum.TryParse(textoGenero, true, out Genero genero)
            && Enum.IsDefined(genero);

        if (!esGeneroValido)
        {
            throw new JsonException("El género debe ser Masculino, Femenino, Otro o NoEspecificado.");
        }

        return genero;
    }

    public override void Write(Utf8JsonWriter writer, Genero value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}
