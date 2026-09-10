using Clientes.Aplicacion.Contratos;
using Microsoft.AspNetCore.Identity;

namespace Clientes.Infraestructura.Seguridad;

public sealed class EncriptadorContrasena : IEncriptadorContrasena
{
    private readonly PasswordHasher<object> encriptador = new();

    public string GenerarHash(string contrasena)
    {
        if (string.IsNullOrWhiteSpace(contrasena))
        {
            throw new ArgumentException("La contraseña es obligatoria.", nameof(contrasena));
        }

        return encriptador.HashPassword(new object(), contrasena);
    }
}
