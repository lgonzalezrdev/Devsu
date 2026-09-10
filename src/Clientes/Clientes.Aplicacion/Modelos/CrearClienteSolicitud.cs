using System.ComponentModel.DataAnnotations;
using Clientes.Dominio.Enumeraciones;

namespace Clientes.Aplicacion.Modelos;

public sealed class CrearClienteSolicitud
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [RegularExpression(@"^[\p{L}\s'-]{3,150}$", ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres alfabéticos.")]
    public string Nombre { get; init; } = null!;

    [EnumDataType(typeof(Genero), ErrorMessage = "El género debe ser Masculino, Femenino, Otro o NoEspecificado.")]
    public Genero Genero { get; init; }

    [Range(0, 130, ErrorMessage = "La edad debe estar entre 0 y 130.")]
    public int Edad { get; init; }

    [Required(ErrorMessage = "La identificación es obligatoria.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "La identificación debe tener exactamente 10 dígitos.")]
    public string Identificacion { get; init; } = null!;

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [StringLength(250, MinimumLength = 5, ErrorMessage = "La dirección debe tener entre 5 y 250 caracteres.")]
    public string Direccion { get; init; } = null!;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(@"^\d{7,15}$", ErrorMessage = "El teléfono debe contener entre 7 y 15 dígitos.")]
    public string Telefono { get; init; } = null!;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 4, ErrorMessage = "La contraseña debe tener entre 4 y 100 caracteres.")]
    public string Contrasena { get; init; } = null!;

}
