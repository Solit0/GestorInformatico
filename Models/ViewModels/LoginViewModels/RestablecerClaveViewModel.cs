using System.ComponentModel.DataAnnotations;

namespace GestorInformatico.Models.ViewModels.LoginViewModels;

public class RestablecerClaveViewModel
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]
    public string NuevaContrasena { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe confirmar la contraseña.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar contraseña")]
    [Compare("NuevaContrasena", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmarContrasena { get; set; } = string.Empty;
}
