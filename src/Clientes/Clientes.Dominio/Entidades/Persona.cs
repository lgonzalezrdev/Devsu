namespace Clientes.Dominio.Entidades;

using Clientes.Dominio.Enumeraciones;

public abstract class Persona
{
    protected Persona()
    {
    }

    protected Persona(
        Guid personaId,
        string nombre,
        Genero genero,
        int edad,
        string identificacion,
        string direccion,
        string telefono)
    {
        if (string.IsNullOrWhiteSpace(nombre) || nombre.Trim().Length > 150)
        {
            throw new Excepciones.ExcepcionReglaDominioException("El nombre es obligatorio y no puede superar 150 caracteres.");
        }

        if (edad < 0)
        {
            throw new Excepciones.ExcepcionReglaDominioException("La edad no puede ser negativa.");
        }

        if (!Enum.IsDefined(genero))
        {
            throw new Excepciones.ExcepcionReglaDominioException("El género debe ser Masculino, Femenino, Otro o NoEspecificado.");
        }

        if (string.IsNullOrWhiteSpace(identificacion) || identificacion.Length != 10 || !identificacion.All(char.IsDigit))
        {
            throw new Excepciones.ExcepcionReglaDominioException("La identificación debe tener exactamente 10 dígitos.");
        }

        if (string.IsNullOrWhiteSpace(direccion) || direccion.Trim().Length < 5 || direccion.Trim().Length > 250)
        {
            throw new Excepciones.ExcepcionReglaDominioException("La dirección debe tener entre 5 y 250 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(telefono) || telefono.Trim().Length > 20)
        {
            throw new Excepciones.ExcepcionReglaDominioException("El teléfono es obligatorio y no puede superar 20 caracteres.");
        }

        PersonaId = personaId == Guid.Empty ? Guid.NewGuid() : personaId;
        Nombre = nombre.Trim();
        Genero = genero;
        Edad = edad;
        Identificacion = identificacion?.Trim() ?? string.Empty;
        Direccion = direccion?.Trim() ?? string.Empty;
        Telefono = telefono?.Trim() ?? string.Empty;
    }

    public Guid PersonaId { get; private set; }

    public string Nombre { get; private set; } = null!;

    public Genero Genero { get; private set; }

    public int Edad { get; private set; }

    public string Identificacion { get; private set; } = null!;

    public string Direccion { get; private set; } = null!;

    public string Telefono { get; private set; } = null!;

    protected void ActualizarDatosPersonales(
        string nombre,
        Genero genero,
        int edad,
        string identificacion,
        string direccion,
        string telefono)
    {
        if (string.IsNullOrWhiteSpace(nombre) || nombre.Trim().Length > 150)
        {
            throw new Excepciones.ExcepcionReglaDominioException("El nombre es obligatorio y no puede superar 150 caracteres.");
        }

        if (edad < 0)
        {
            throw new Excepciones.ExcepcionReglaDominioException("La edad no puede ser negativa.");
        }

        if (!Enum.IsDefined(genero))
        {
            throw new Excepciones.ExcepcionReglaDominioException("El género debe ser Masculino, Femenino, Otro o NoEspecificado.");
        }

        if (string.IsNullOrWhiteSpace(identificacion) || identificacion.Length != 10 || !identificacion.All(char.IsDigit))
        {
            throw new Excepciones.ExcepcionReglaDominioException("La identificación debe tener exactamente 10 dígitos.");
        }

        if (string.IsNullOrWhiteSpace(direccion) || direccion.Trim().Length < 5 || direccion.Trim().Length > 250)
        {
            throw new Excepciones.ExcepcionReglaDominioException("La dirección debe tener entre 5 y 250 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(telefono) || telefono.Trim().Length > 20)
        {
            throw new Excepciones.ExcepcionReglaDominioException("El teléfono es obligatorio y no puede superar 20 caracteres.");
        }

        Nombre = nombre.Trim();
        Genero = genero;
        Edad = edad;
        Identificacion = identificacion?.Trim() ?? string.Empty;
        Direccion = direccion?.Trim() ?? string.Empty;
        Telefono = telefono?.Trim() ?? string.Empty;
    }
}
