namespace GestorInformatico.Models.ViewModels.Inventario;

public class VerInventarioViewModel
{
    public int Id { get; set; }
    
    public string IdFormateado => $"#REP-{Id}"; 

    public string Nombre { get; set; }
    
    public string Descripcion { get; set; } 
    
    public string Tipo { get; set; } 
    
    public decimal Precio { get; set; }
    
    public int StockDisponible { get; set; }
    
    public string TextoStock => StockDisponible == 1 ? "1 unidad" : $"{StockDisponible} unidades";

    // --- LÓGICA DE ESTADOS Y COLORES ---
    
    public string TextoEstadoStock => StockDisponible == 0 ? "Agotado" 
        : (StockDisponible < 3 ? "Stock Bajo" : "En Stock");
    
    public string ClaseColorTextoStock => StockDisponible == 0 ? "text-danger" 
        : (StockDisponible < 3 ? "text-warning" : "text-success");

    public string ClaseColorBadge => StockDisponible == 0 ? "bg-danger" 
        : (StockDisponible < 3 ? "bg-warning text-dark" : "bg-success");
    
    public bool PuedeUsarse => StockDisponible > 0;
}