using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using GestorInformatico.Models;
using GestorInformatico.Models.ViewModels.LoginViewModels;

namespace GestorInformatico.Controllers;

public class AuthController : Controller
{
    private readonly SignInManager<Usuarios> _signInManager;
    private readonly UserManager<Usuarios> _userManager;

    public AuthController(SignInManager<Usuarios> signInManager, UserManager<Usuarios> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new InicioSesionViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(InicioSesionViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var input = model.CorreoElectronico?.Trim() ?? string.Empty;
        var user = await _userManager.FindByEmailAsync(input) ?? await _userManager.FindByNameAsync(input);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Correo electrónico/usuario o contraseña incorrectos.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Contrasena, model.RecordarSesion, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Administrador"))
            {
                return RedirectToAction("Index", "Administrador");
            }

            return RedirectToAction("Index", "Tecnico");
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "La cuenta se encuentra bloqueada temporalmente.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Correo electrónico/usuario o contraseña incorrectos.");
        return View(model);
    }

    [HttpGet]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Auth");
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
