namespace GestorInformatico.Models.ViewModels.LoginViewModels;
using System.ComponentModel.DataAnnotations;

public class InicioSesionViewModel
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
    public string CorreoElectronico { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; }
}