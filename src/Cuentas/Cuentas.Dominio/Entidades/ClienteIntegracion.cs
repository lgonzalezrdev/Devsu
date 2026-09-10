namespace Cuentas.Dominio.Entidades;

/// <summary>Proyección local, de solo integración, del cliente propietario de una cuenta.</summary>
public sealed class ClienteIntegracion
{
    private ClienteIntegracion()
    {
    }

    public ClienteIntegracion(Guid clienteId, string nombre, bool estado, DateTime actualizadoEn)
    {
        if (clienteId == Guid.Empty)
        {
            throw new Excepciones.ExcepcionReglaDominioException("El identificador del cliente es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new Excepciones.ExcepcionReglaDominioException("El nombre del cliente es obligatorio.");
        }

        ClienteId = clienteId;
        Nombre = nombre.Trim();
        Estado = estado;
        ActualizadoEn = actualizadoEn;
    }

    public Guid ClienteId { get; private set; }

    public string Nombre { get; private set; } = null!;

    public bool Estado { get; private set; }

    public DateTime ActualizadoEn { get; private set; }

    /// <summary>Aplica el estado más reciente recibido desde el microservicio Clientes.</summary>
    public void Actualizar(string nombre, bool estado, DateTime actualizadoEn)
    {
        if (actualizadoEn < ActualizadoEn)
        {
            return;
        }

        Nombre = nombre.Trim();
        Estado = estado;
        ActualizadoEn = actualizadoEn;
    }
}
