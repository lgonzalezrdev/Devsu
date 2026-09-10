namespace Clientes.Dominio.Entidades;

using Clientes.Dominio.Enumeraciones;

public sealed class Cliente : Persona
{
    private Cliente()
    {
    }

    public Cliente(
        Guid personaId,
        string nombre,
        Genero genero,
        int edad,
        string identificacion,
        string direccion,
        string telefono,
        string contrasenaHash)
        : base(personaId, nombre, genero, edad, identificacion, direccion, telefono)
    {
        if (string.IsNullOrWhiteSpace(contrasenaHash))
        {
            throw new Excepciones.ExcepcionReglaDominioException("La contraseña es obligatoria.");
        }

        ContrasenaHash = contrasenaHash;
    }

    // ClienteId es la misma clave heredada de Persona en el modelo de dominio.
    public Guid ClienteId => PersonaId;

    public string ContrasenaHash { get; private set; } = null!;

    public bool Estado { get; private set; } = true;

    /// <summary>Cambia el estado y evita inactivar dos veces el mismo cliente.</summary>
    public void CambiarEstado(bool estado)
    {
        if (!estado && !Estado)
        {
            throw new Excepciones.ExcepcionReglaDominioException("El cliente ya se encuentra inactivo; el estado no ha cambiado.");
        }

        Estado = estado;
    }

    /// <summary>Actualiza los datos personales que pertenecen al cliente.</summary>
    public void Actualizar(
        string nombre,
        Genero genero,
        int edad,
        string identificacion,
        string direccion,
        string telefono)
    {
        ActualizarDatosPersonales(nombre, genero, edad, identificacion, direccion, telefono);
    }

    /// <summary>Reemplaza la contraseña almacenada por un hash válido.</summary>
    public void CambiarContrasena(string contrasenaHash)
    {
        if (string.IsNullOrWhiteSpace(contrasenaHash))
        {
            throw new Excepciones.ExcepcionReglaDominioException("La contraseña es obligatoria.");
        }

        ContrasenaHash = contrasenaHash;
    }
}
