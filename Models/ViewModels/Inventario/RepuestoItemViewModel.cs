namespace GestorInformatico.Models.ViewModels.Inventario;

public class RepuestoItemViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int StockDisponible { get; set; }
    public int StockMinimo { get; set; } = 3;

    public string Estado => StockDisponible == 0 ? "Agotado"
        : (StockDisponible < 3 ? "Stock Bajo" : "En Stock");

    public bool PuedeEliminarse { get; set; } = true;
}
