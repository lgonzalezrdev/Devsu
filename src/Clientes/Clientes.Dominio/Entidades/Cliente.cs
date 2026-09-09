namespace Clientes.Dominio.Entidades;

public sealed class Cliente : Persona
{
    private Cliente()
    {
    }

    public Cliente(
        Guid personaId,
        string nombre,
        string genero,
        int edad,
        string identificacion,
        string direccion,
        string telefono,
        string contrasenaHash,
        bool estado)
        : base(personaId, nombre, genero, edad, identificacion, direccion, telefono)
    {
        if (string.IsNullOrWhiteSpace(contrasenaHash))
        {
            throw new ArgumentException("La contraseña es obligatoria.", nameof(contrasenaHash));
        }

        ContrasenaHash = contrasenaHash;
        Estado = estado;
    }

    // ClienteId es la misma clave heredada de Persona en el modelo de dominio.
    public Guid ClienteId => PersonaId;

    public string ContrasenaHash { get; private set; } = null!;

    public bool Estado { get; private set; }

    public void CambiarEstado(bool estado) => Estado = estado;
}
