using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestorInformatico.Models.ViewModels.Inventario;

public class AsignarRepuestoViewModel
{
    [Required(ErrorMessage = "Debe seleccionar una orden de reparación")]
    public int OrdenReparacionId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un repuesto")]
    public int RepuestoId { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Cantidad { get; set; } = 1;

    public string? Nota { get; set; }

    public IEnumerable<SelectListItem>? ListaOrdenes { get; set; }
    public IEnumerable<SelectListItem>? ListaRepuestos { get; set; }
}
