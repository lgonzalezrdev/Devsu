namespace Clientes.Aplicacion.Contratos;

public interface IEncriptadorContrasena
{
    string GenerarHash(string contrasena);
}
