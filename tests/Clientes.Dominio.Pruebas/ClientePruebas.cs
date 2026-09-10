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
}
