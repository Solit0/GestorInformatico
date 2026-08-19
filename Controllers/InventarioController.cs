using Microsoft.AspNetCore.Mvc;

namespace GestorInformatico.Controllers;

public class InventarioController : Controller
{
    public IActionResult Index(string? terminoBusqueda, string? categoriaSeleccionada)
    {
        return RedirectToAction("Inventario", "Tecnico", new { terminoBusqueda, categoriaSeleccionada });
    }
}