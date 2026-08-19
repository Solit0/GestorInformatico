namespace GestorInformatico.Models.ViewModels.Ordenes;
using System.ComponentModel.DataAnnotations;
public class FormNuevaOrdenViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un cliente")]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar el equipo del cliente")]
    [Display(Name = "Equipo del Cliente")]
    public int EquipoId { get; set; }

    [Required(ErrorMessage = "La fecha de ingreso es obligatoria")]
    [Display(Name = "Fecha de Ingreso")]
    public DateTime FechaIngreso { get; set; } = DateTime.Now; 

    [Required(ErrorMessage = "Debe asignar un técnico")]
    [Display(Name = "Técnico Asignado")]
    public string TecnicoId { get; set; }

    [Required(ErrorMessage = "Debe detallar el problema reportado")]
    [StringLength(1000, ErrorMessage = "La descripción es muy larga")]
    [Display(Name = "Descripción de la Falla / Problema Reportado")]
    public string Descripcion { get; set; }

    [StringLength(1000, ErrorMessage = "Las observaciones son muy largas")]
    [Display(Name = "Observaciones y Diagnóstico Inicial")]
    public string? Observaciones { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un estado inicial")]
    [Display(Name = "Estado Inicial")]
    public string Estado { get; set; } = "Pendiente";
}