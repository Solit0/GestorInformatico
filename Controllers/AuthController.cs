using Microsoft.AspNetCore.Mvc;

namespace GestorInformatico.Controllers;

public class AuthController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult RecuperarPassword()
    {
        return View();
    }

    [HttpGet]
    public IActionResult RestablecerPassword()
    {
        return View();
    }
}
