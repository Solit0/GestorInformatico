using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestorInformatico.Models.ViewModels.Ordenes;

public class RecepcionOrdenesViewModel
{
    public FormNuevaOrdenViewModel FormularioNuevaOrden { get; set; } = new FormNuevaOrdenViewModel();
    
    public List<OrdenRecienteViewModel> OrdenesRecientes { get; set; } = new List<OrdenRecienteViewModel>();

    public IEnumerable<SelectListItem>? ListaClientes { get; set; }
    public IEnumerable<SelectListItem>? ListaEquipos { get; set; }
    public IEnumerable<SelectListItem>? ListaTecnicos { get; set; }
    public IEnumerable<SelectListItem>? ListaEstados { get; set; }
}