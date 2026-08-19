using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestorInformatico.Models.ViewModels.FlujoTrabajo;

public class FlujoVM
{
    public string? TerminoBusqueda { get; set; }
    
    public string? TecnicoSeleccionado { get; set; }
    public IEnumerable<SelectListItem>? ListaTecnicos { get; set; }

    // Las 4 columnas de tu diseño
    public List<EstadosReparacionViewModel> OrdenesPendientes { get; set; } = new List<EstadosReparacionViewModel>();
    public List<EstadosReparacionViewModel> OrdenesEnReparacion { get; set; } = new List<EstadosReparacionViewModel>();
    public List<EstadosReparacionViewModel> OrdenesReparadas { get; set; } = new List<EstadosReparacionViewModel>();
    public List<EstadosReparacionViewModel> OrdenesNoSePudo { get; set; } = new List<EstadosReparacionViewModel>();
    
    public int TotalPendientes => OrdenesPendientes.Count;
    public int TotalEnReparacion => OrdenesEnReparacion.Count;
    public int TotalReparadas => OrdenesReparadas.Count;
    public int TotalNoSePudo => OrdenesNoSePudo.Count;
}