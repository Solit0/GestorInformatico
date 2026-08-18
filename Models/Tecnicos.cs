namespace GestorInformatico.Models;
using System.ComponentModel.DataAnnotations;
public class Tecnicos
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(120)]
    public string Nombre { get; set; }
    
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "El telefono es obligatorio")]
    [StringLength(9)]
    [RegularExpression(@"^[0-9]{4}-?[0-9]{4}$", ErrorMessage = "Debe ser un teléfono válido (ej. 2222-3333 o 22223333)")]
    public string Telefono { get; set; }
    
    [StringLength(120)]
    public string? Direccion { get; set; }
    
    [Required(ErrorMessage = "El DUI es obligatorio")]
    [RegularExpression(@"^\d{8}-\d$", ErrorMessage = "Debe contener 8 dígitos seguidos de un guion y un dígito")]
    public string DUI { get; set; }
    
    [Required(ErrorMessage = "El sexo es obligatorio")]
    [AllowedValues("Masculino", "Femenino")]
    [StringLength(15)]
    public string Sexo { get; set; }
    
    [DataType(DataType.Date)]
    public DateOnly FechaNacimiento { get; set; }
    
    public ICollection<OrdenReparacion> OrdenesReparacion { get; set; } = new List<OrdenReparacion>();
    
}