namespace GestorInformatico.Models.ViewModels.Administrador;

public class AdminDashboardViewModel
{
    public int TotalUsuarios { get; set; }
    public int TecnicosActivos { get; set; }
    public decimal ValorTotalAlmacen { get; set; }
    public int CategoriasActivas { get; set; }
    public int ReparacionesEnCurso { get; set; }
    public int ReparacionesPendientes { get; set; }
    public int AlertasStock { get; set; }
    public int ArticulosEnExistencia { get; set; }
}
