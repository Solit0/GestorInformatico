using System.Runtime.InteropServices.JavaScript;
using System.ComponentModel.DataAnnotations;

namespace GestorInformatico.Models;

public class OrdenReparacion
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "La fecha de ingreso es obligatoria")]
    public DateTime FechaIngreso { get; set; }
    
    
    public DateTime? FechaSalida { get; set; }
    
    [Required(ErrorMessage = "El equipo es obligatorio")]
    public int EquipoId { get; set; }
    public Equipos Equipo { get; set; }
    
    public ICollection<DetalleOrdenRepuesto> RepuestosUsados { get; set; } = new List<DetalleOrdenRepuesto>();
    
    [Required(ErrorMessage = "El tecnico es obligatorio")]
    public int TecnicoId { get; set; }
    public Tecnicos Tecnico { get; set; }
    
    [Required(ErrorMessage = "El estado es obligatorio")]
    [AllowedValues("Pendiente", "En reparacion", "Reparado", "No se pudo")]
    [StringLength(20)]
    public string Estado { get; set; }
    
    [StringLength(1000)]
    public string? Observaciones { get; set; }
    
    [StringLength(1000)]
    [Required(ErrorMessage = "La descripcion es obligatoria")]
    public string Descripcion { get; set; }
}