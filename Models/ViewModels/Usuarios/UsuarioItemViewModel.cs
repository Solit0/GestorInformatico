namespace GestorInformatico.Models.ViewModels.Usuarios;

public class UsuarioItemViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string DUI { get; set; } = string.Empty;
    public string Sexo { get; set; } = string.Empty;
    public DateOnly FechaNacimiento { get; set; }
    public string? Direccion { get; set; }
    public string Rol { get; set; } = string.Empty;
    public bool EsActivo { get; set; }
}
