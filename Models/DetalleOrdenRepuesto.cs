using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorInformatico.Models;

public class DetalleOrdenRepuesto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "La cantidad es obligatoria")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero")]
    public int Cantidad { get; set; }
    
    [Required(ErrorMessage = "El precio unitario es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor que cero")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioUnitario { get; set; } 

    [Required(ErrorMessage = "La orden de reparacion es obligatoria")]
    public int OrdenReparacionId { get; set; }
    public OrdenReparacion OrdenReparacion { get; set; }
    
    [Required(ErrorMessage = "El repuesto es obligatorio")]
    public int RepuestoId { get; set; }
    public Repuestos Repuesto { get; set; }
    
    public decimal SubTotal => Cantidad * PrecioUnitario;
}