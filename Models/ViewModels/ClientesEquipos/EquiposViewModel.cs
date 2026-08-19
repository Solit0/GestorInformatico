namespace GestorInformatico.Models.ViewModels.ClientesEquipos;

public class EquiposViewModel
{
    public int Id { get; set; }
    
    public string IdFormateado => $"#{Id}"; 
    
    public string Nombre { get; set; } 
 
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public string MarcaYModelo => $"{Marca} {Modelo}"; 
    
    public string NumeroSerie { get; set; }
    
    public string NombreCliente { get; set; }
    
    public int CantidadReparaciones { get; set; }
    
    public string TextoBadgeHistorial => CantidadReparaciones switch
    {
        0 => "Sin historial",
        1 => "1 reparación",
        _ => $"{CantidadReparaciones} reparaciones"
    };
    
    public string ClaseColorBadge => CantidadReparaciones == 0 ? "bg-secondary" : "bg-info text-dark";
}