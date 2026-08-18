using Microsoft.AspNetCore.Mvc;

namespace GestorInformatico.Controllers;

public class AdministradorController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Usuarios()
    {
        return View();
    }

    public IActionResult Inventario()
    {
        return View();
    }
}
