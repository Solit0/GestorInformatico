using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorInformatico.Models;

public class Repuestos
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(120)]
    public string Nombre { get; set; }
    
    [Required(ErrorMessage = "El tipo es obligatorio")]
    [StringLength(50)]
    public string Tipo { get; set; }
    
    [Required(ErrorMessage = "El precio es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Precio { get; set; }
    
    [Required(ErrorMessage = "El stock disponible es obligatorio")]
    [Range(0, int.MaxValue, ErrorMessage = "El stock disponible no puede ser negativo")]
    public int StockDisponible { get; set; }
    
    public ICollection<DetalleOrdenRepuesto> HistorialUso { get; set; } = new List<DetalleOrdenRepuesto>();
}