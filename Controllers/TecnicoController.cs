using Microsoft.AspNetCore.Mvc;

namespace GestorInformatico.Controllers;

public class TecnicoController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ClientesEquipos()
    {
        return View();
    }

    public IActionResult RecepcionOrdenes()
    {
        return View();
    }

    public IActionResult FlujoTrabajo()
    {
        return View();
    }

    public IActionResult Inventario()
    {
        return View();
    }

    public IActionResult CambiarPassword()
    {
        return View();
    }
}
