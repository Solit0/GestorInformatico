namespace GestorInformatico.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
public class Usuarios : IdentityUser
{
    
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(120)]
    public string Nombre { get; set; }
    
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
    
    public bool DebeCambiarPassword { get; set; } = true;
    
}