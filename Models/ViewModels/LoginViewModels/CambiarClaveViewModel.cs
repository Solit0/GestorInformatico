namespace GestorInformatico.Models.ViewModels.LoginViewModels;
using System.ComponentModel.DataAnnotations;

public class CambiarClaveViewModel
{
    [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
    [DataType(DataType.Password)]
    public string ContrasenaActual { get; set; }

    [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    public string NuevaContrasena { get; set; }

    [Required(ErrorMessage = "Debe confirmar la nueva contraseña.")]
    [DataType(DataType.Password)]
    [Compare("NuevaContrasena", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmarNuevaContrasena { get; set; }
}