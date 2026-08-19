namespace GestorInformatico.Models.ViewModels.Usuarios;

public class GestionUsuariosViewModel
{
    public List<UsuarioItemViewModel> Usuarios { get; set; } = new();
    public CrearUsuarioViewModel NuevoUsuario { get; set; } = new();
    public string? TerminoBusqueda { get; set; }
    public string? RolSeleccionado { get; set; }
    public string? EstadoSeleccionado { get; set; }
}
