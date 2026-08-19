namespace GestorInformatico.Models.ViewModels.Inventario;

public class GestionInventarioViewModel
{
    // KPIs
    public int TotalRepuestos { get; set; }
    public int TotalUnidades { get; set; }
    public int TiposRegistrados { get; set; }
    public decimal ValorTotalEstimado { get; set; }
    public int StockBajo { get; set; }
    public int Agotado { get; set; }

    // Filtros
    public string? TerminoBusqueda { get; set; }
    public string? CategoriaSeleccionada { get; set; }
    public string? EstadoSeleccionado { get; set; }

    public List<string> CategoriasDisponibles { get; set; } = new();
    public List<RepuestoItemViewModel> Repuestos { get; set; } = new();

    // Formularios para modales
    public CrearRepuestoViewModel NuevoRepuesto { get; set; } = new();
    public CrearCategoriaViewModel NuevaCategoria { get; set; } = new();
}
