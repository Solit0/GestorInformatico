using Microsoft.AspNetCore.Mvc.Rendering;
namespace GestorInformatico.Models.ViewModels.ClientesEquipos;

public class ClientesEquiposVM
{
    public int TotalClientes { get; set; }
    public int TotalEquipos { get; set; }
    
    public string? TerminoBusqueda { get; set; }
    public string? MarcaSeleccionada { get; set; }
    public IEnumerable<SelectListItem>? ListaMarcas { get; set; } 
    
    public List<ClientesViewModel> Clientes { get; set; } = new List<ClientesViewModel>();
    public List<EquiposViewModel> Equipos { get; set; } = new List<EquiposViewModel>();
    
}