using Clientes.Dominio.Entidades;
using Clientes.Dominio.Enumeraciones;
using Clientes.Dominio.Excepciones;
using Xunit;

namespace Clientes.Dominio.Pruebas;

public sealed class ClientePruebas
{
    [Fact]
    public void ConstructorConIdentificacionInvalidaLanzaErrorDeDominio()
    {
        ExcepcionReglaDominioException excepcion = Assert.Throws<ExcepcionReglaDominioException>(() => new Cliente(
            Guid.NewGuid(),
            "Jose Lema",
            Genero.Masculino,
            35,
            "12345",
            "Otavalo sn y principal",
            "098254785",
            "hash-valido"));

        Assert.Equal("La identificación debe tener exactamente 10 dígitos.", excepcion.Message);
    }

    [Theory]
    [InlineData("Jo", 30, "098254785", "El nombre debe tener entre 3 y 150 caracteres alfabéticos.")]
    [InlineData("Jose Lema", 131, "098254785", "La edad debe estar entre 0 y 130.")]
    [InlineData("Jose Lema", 30, "09825A", "El teléfono debe contener entre 7 y 15 dígitos.")]
    public void ConstructorConDatosPersonalesInvalidosLanzaErrorDeDominio(string nombre, int edad, string telefono, string mensajeEsperado)
    {
        ExcepcionReglaDominioException excepcion = Assert.Throws<ExcepcionReglaDominioException>(() => new Cliente(
            Guid.NewGuid(),
            nombre,
            Genero.Masculino,
            edad,
            "1234567890",
            "Otavalo sn y principal",
            telefono,
            "hash-valido"));

        Assert.Equal(mensajeEsperado, excepcion.Message);
    }
}
