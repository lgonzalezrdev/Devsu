namespace Clientes.Aplicacion.Contratos;

public interface IEncriptadorContrasena
{
    /// <summary>Genera un hash seguro a partir de una contraseña sin cifrar.</summary>
    string GenerarHash(string contrasena);
}
