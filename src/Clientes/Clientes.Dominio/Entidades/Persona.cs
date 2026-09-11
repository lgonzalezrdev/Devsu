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
        ValidarDatos(nombre, genero, edad, identificacion, direccion, telefono);

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
        ValidarDatos(nombre, genero, edad, identificacion, direccion, telefono);

        Nombre = nombre.Trim();
        Genero = genero;
        Edad = edad;
        Identificacion = identificacion?.Trim() ?? string.Empty;
        Direccion = direccion?.Trim() ?? string.Empty;
        Telefono = telefono?.Trim() ?? string.Empty;
    }

    private static void ValidarDatos(string nombre, Genero genero, int edad, string identificacion, string direccion, string telefono)
    {
        string nombreNormalizado = nombre?.Trim() ?? string.Empty;
        if (nombreNormalizado.Length < 3 || nombreNormalizado.Length > 150 || !nombreNormalizado.All(caracter => char.IsLetter(caracter) || char.IsWhiteSpace(caracter) || caracter is '\'' or '-'))
        {
            throw new Excepciones.ExcepcionReglaDominioException("El nombre debe tener entre 3 y 150 caracteres alfabéticos.");
        }

        if (edad is < 0 or > 130)
        {
            throw new Excepciones.ExcepcionReglaDominioException("La edad debe estar entre 0 y 130.");
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

        string telefonoNormalizado = telefono?.Trim() ?? string.Empty;
        if (telefonoNormalizado.Length < 7 || telefonoNormalizado.Length > 15 || !telefonoNormalizado.All(char.IsDigit))
        {
            throw new Excepciones.ExcepcionReglaDominioException("El teléfono debe contener entre 7 y 15 dígitos.");
        }
    }
}
