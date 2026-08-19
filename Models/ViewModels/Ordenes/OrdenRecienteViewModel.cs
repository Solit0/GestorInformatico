namespace GestorInformatico.Models.ViewModels.Ordenes;

public class OrdenRecienteViewModel
{
    public int Id { get; set; }
    
    public string NumeroOrdenFormateado => $"#ORD-{Id}"; 
    
    public DateTime FechaIngreso { get; set; }
    
    public string NombreCliente { get; set; }
    
    public string NombreEquipo { get; set; } 
    
    public string NombreTecnico { get; set; }
    
    public string Estado { get; set; }
}