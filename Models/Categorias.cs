using System.ComponentModel.DataAnnotations;

namespace GestorInformatico.Models;

public class Categorias
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
    [StringLength(50, ErrorMessage = "El nombre de la categoría no puede tener más de 50 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "La descripción no puede tener más de 200 caracteres")]
    public string? Descripcion { get; set; }
}
