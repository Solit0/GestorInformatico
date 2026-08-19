using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestorInformatico.Models.ViewModels.Ordenes;

public class CrearOrdenViewModel
{
    [Required]
    public string Descripcion { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un equipo")]
    public int EquipoId { get; set; }

    [Required(ErrorMessage = "Debe asignar un técnico")]
    public string TecnicoId { get; set; } 
    
    public IEnumerable<SelectListItem>? ListaEquipos { get; set; }
    public IEnumerable<SelectListItem>? ListaTecnicos { get; set; }
}