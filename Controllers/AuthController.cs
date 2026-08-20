using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GestorInformatico.Models;
using GestorInformatico.Models.ViewModels.LoginViewModels;
using GestorInformatico.Services;

namespace GestorInformatico.Controllers;

public class AuthController : Controller
{
    private readonly SignInManager<Usuarios> _signInManager;
    private readonly UserManager<Usuarios> _userManager;
    private readonly IEmailService _emailService;

    public AuthController(SignInManager<Usuarios> signInManager, UserManager<Usuarios> userManager, IEmailService emailService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Administrador");
        }

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
            if (user.DebeCambiarPassword)
            {
                return RedirectToAction("CambiarPassword", "Tecnico");
            }

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
        return View(new RecuperarClaveViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RecuperarPassword(RecuperarClaveViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.CorreoElectronico.Trim());

        if (user == null)
        {
            TempData["Success"] = "Si el correo está registrado, recibirás instrucciones para restablecer tu contraseña.";
            return RedirectToAction(nameof(RecuperarPassword));
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetUrl = Url.Action(nameof(RestablecerPassword), "Auth", new { userId = user.Id, token }, Request.Scheme);

        var cuerpoHtml = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #dee2e6; border-radius: 8px;'>
                <h2 style='color: #0d6efd;'>Restablecer Contraseña</h2>
                <p>Hola <strong>{user.Nombre}</strong>,</p>
                <p>Has solicitado restablecer tu contraseña para ingresar a <strong>Gestor Informático</strong>.</p>
                <p>Haz clic en el siguiente botón para crear una nueva contraseña:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <a href='{resetUrl}' style='background-color: #0d6efd; color: #ffffff; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>Restablecer mi Contraseña</a>
                </div>
                <p style='color: #6c757d; font-size: 0.9em;'>O copia y pega el siguiente enlace en tu navegador:</p>
                <p style='word-break: break-all; color: #0d6efd; font-size: 0.85em;'>{resetUrl}</p>
                <hr style='border: none; border-top: 1px solid #dee2e6; margin: 20px 0;' />
                <p style='color: #888888; font-size: 0.8em;'>Si no realizaste esta solicitud, puedes ignorar este correo de forma segura.</p>
            </div>";

        await _emailService.EnviarCorreoAsync(
            user.Email!,
            "Restablecer contraseña - Gestor Informático",
            cuerpoHtml);

        TempData["Success"] = "Si el correo está registrado, recibirás instrucciones para restablecer tu contraseña.";
        return RedirectToAction(nameof(RecuperarPassword));
    }

    [HttpGet]
    public async Task<IActionResult> RestablecerPassword(string? userId, string? token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            return RedirectToAction(nameof(Login));
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var model = new RestablecerClaveViewModel
        {
            UserId = userId,
            Token = token
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RestablecerPassword(RestablecerClaveViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NuevaContrasena);

        if (result.Succeeded)
        {
            user.DebeCambiarPassword = false;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Contraseña restablecida exitosamente. Ya puedes iniciar sesión.";
            return RedirectToAction(nameof(Login));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }
}
