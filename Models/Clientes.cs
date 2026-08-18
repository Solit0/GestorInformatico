namespace GestorInformatico.Models;
using System.ComponentModel.DataAnnotations;

public class Clientes
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(120)]
    public string Nombre { get; set; }
    
    [Required(ErrorMessage = "La direccion es obligatoria")]
    [StringLength(120)]
    public string Direccion { get; set; }
    
    [Required(ErrorMessage = "El telefono es obligatorio")]
    [RegularExpression(@"^[0-9]{4}-?[0-9]{4}$", ErrorMessage = "Debe ser un teléfono válido (ej. 2222-3333 o 22223333)")]
    public string Telefono { get; set; }
    
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "El DUI es obligatorio")]
    [RegularExpression(@"^\d{8}-\d$", ErrorMessage = "Debe contener 8 dígitos seguidos de un guion y un dígito")]
    public string DUI { get; set; }
    
    public ICollection<Equipos> Equipos { get; set; } = new List<Equipos>();
    
    public bool RecibeNotificacionesCorreo { get; set; } = true;
}