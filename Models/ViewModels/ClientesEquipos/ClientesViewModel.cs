namespace GestorInformatico.Models.ViewModels.ClientesEquipos;

public class ClientesViewModel
{
    public int Id { get; set; }
    
    public string IdFormateado => $"#{Id}"; 

    public string Nombre { get; set; }
    
    public string DUI { get; set; }
    
    public string Telefono { get; set; }
    public string Email { get; set; }
    public string Contacto => $"{Telefono} - {Email}";

    public string Direccion { get; set; }
    
    public int CantidadEquipos { get; set; }
    
    public string TextoBadgeEquipos => CantidadEquipos == 1 ? "1 Equipo" : $"{CantidadEquipos} Equipos";
}