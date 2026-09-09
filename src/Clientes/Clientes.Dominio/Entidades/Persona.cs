namespace Clientes.Dominio.Entidades;

public abstract class Persona
{
    protected Persona()
    {
    }

    protected Persona(
        Guid personaId,
        string nombre,
        string genero,
        int edad,
        string identificacion,
        string direccion,
        string telefono)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));
        }

        if (edad < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(edad), "La edad no puede ser negativa.");
        }

        PersonaId = personaId == Guid.Empty ? Guid.NewGuid() : personaId;
        Nombre = nombre.Trim();
        Genero = genero?.Trim() ?? string.Empty;
        Edad = edad;
        Identificacion = identificacion?.Trim() ?? string.Empty;
        Direccion = direccion?.Trim() ?? string.Empty;
        Telefono = telefono?.Trim() ?? string.Empty;
    }

    public Guid PersonaId { get; private set; }

    public string Nombre { get; private set; } = null!;

    public string Genero { get; private set; } = null!;

    public int Edad { get; private set; }

    public string Identificacion { get; private set; } = null!;

    public string Direccion { get; private set; } = null!;

    public string Telefono { get; private set; } = null!;
}
