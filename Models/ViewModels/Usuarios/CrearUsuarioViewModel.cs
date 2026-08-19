using System.ComponentModel.DataAnnotations;

namespace GestorInformatico.Models.ViewModels.Usuarios;

public class CrearUsuarioViewModel
{
    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [StringLength(120, ErrorMessage = "El nombre no puede exceder 120 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El DUI es obligatorio.")]
    [RegularExpression(@"^\d{8}-\d$", ErrorMessage = "El DUI debe tener el formato 12345678-9 (8 dígitos, guion y 1 dígito).")]
    public string DUI { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    [Required(ErrorMessage = "El rol es obligatorio.")]
    public string Rol { get; set; } = "Tecnico";

    [Required(ErrorMessage = "El sexo es obligatorio.")]
    public string Sexo { get; set; } = "Masculino";

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    [DataType(DataType.Date)]
    public DateOnly FechaNacimiento { get; set; } = new DateOnly(2000, 1, 1);

    [StringLength(120, ErrorMessage = "La dirección no puede exceder 120 caracteres.")]
    public string? Direccion { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe confirmar la contraseña.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
