using System.ComponentModel.DataAnnotations;

namespace GestorInformatico.Models;

public class Equipos
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "La marca es obligatoria")]
    [StringLength(50)]
    public string Marca { get; set; }
    
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(120)]
    public string Nombre { get; set; }
    
    [Required(ErrorMessage = "El modelo es obligatorio")]
    [StringLength(120)]
    public string Modelo { get; set; }
    
    [Required(ErrorMessage = "El numero de serie es obligatorio")]
    [StringLength(100)]
    public string NumeroSerie { get; set; }
    
    [Required(ErrorMessage = "El cliente es obligatorio")]
    public int ClienteId { get; set; }
    public Clientes Cliente { get; set; } 
    
    public ICollection<OrdenReparacion> HistorialReparaciones { get; set; } = new List<OrdenReparacion>();
}
    
