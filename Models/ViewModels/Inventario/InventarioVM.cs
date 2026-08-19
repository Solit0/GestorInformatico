using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestorInformatico.Models.ViewModels.Inventario;

public class InventarioVM
{
    public int TotalRepuestos { get; set; }
    public int StockBajo { get; set; }
    public int Agotado { get; set; }
    
    public string? TerminoBusqueda { get; set; }
    
    public string? CategoriaSeleccionada { get; set; }
    public IEnumerable<SelectListItem>? ListaCategorias { get; set; }
    
    public string? EstadoSeleccionado { get; set; }
    public IEnumerable<SelectListItem>? ListaEstados { get; set; }
    
    public List<VerInventarioViewModel> Repuestos { get; set; } = new List<VerInventarioViewModel>();
}