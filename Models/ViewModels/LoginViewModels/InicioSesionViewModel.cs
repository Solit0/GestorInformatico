namespace GestorInformatico.Models.ViewModels.LoginViewModels;
using System.ComponentModel.DataAnnotations;

public class InicioSesionViewModel
{
    [Required(ErrorMessage = "El correo electrónico o usuario es obligatorio.")]
    public string CorreoElectronico { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; } = string.Empty;

    public bool RecordarSesion { get; set; }
}