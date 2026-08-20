using System.ComponentModel.DataAnnotations;

namespace GestorInformatico.Models.ViewModels.ClientesEquipos;

public class CrearClienteFormViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El DUI es obligatorio")]
    [RegularExpression(@"^\d{8}-\d$", ErrorMessage = "Debe contener 8 dígitos, guion y 1 dígito (ej. 12345678-9)")]
    public string DUI { get; set; }

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [RegularExpression(@"^[0-9]{4}-?[0-9]{4}$", ErrorMessage = "Debe ser un teléfono válido (ej. 2222-3333 o 22223333)")]
    public string Telefono { get; set; }

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "El correo no es válido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La dirección es obligatoria")]
    public string Direccion { get; set; }

    public bool RecibeNotificacionesCorreo { get; set; } = true;
}
