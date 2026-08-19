namespace GestorInformatico.Models.ViewModels.LoginViewModels;
using System.ComponentModel.DataAnnotations;
public class RecuperarClaveViewModel
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
    public string CorreoElectronico { get; set; }
}