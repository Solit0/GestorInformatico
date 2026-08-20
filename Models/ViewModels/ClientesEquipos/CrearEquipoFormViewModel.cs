using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestorInformatico.Models.ViewModels.ClientesEquipos;

public class CrearEquipoFormViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un cliente")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "El nombre del equipo es obligatorio")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "La marca es obligatoria")]
    public string Marca { get; set; }

    [Required(ErrorMessage = "El modelo es obligatorio")]
    public string Modelo { get; set; }

    [Required(ErrorMessage = "El número de serie es obligatorio")]
    public string NumeroSerie { get; set; }

    public IEnumerable<SelectListItem>? ListaClientes { get; set; }
}
