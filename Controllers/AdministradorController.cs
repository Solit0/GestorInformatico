using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GestorInformatico.Data;
using GestorInformatico.Models;
using GestorInformatico.Models.ViewModels.Administrador;
using GestorInformatico.Models.ViewModels.Inventario;
using GestorInformatico.Models.ViewModels.Usuarios;

namespace GestorInformatico.Controllers;

[Authorize]
public class AdministradorController : Controller
{
    private readonly GestorDbContext _context;
    private readonly UserManager<Usuarios> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdministradorController(
        GestorDbContext context,
        UserManager<Usuarios> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var totalUsuarios = await _userManager.Users.CountAsync();

        var tecnicos = await _userManager.GetUsersInRoleAsync("Tecnico");
        var tecnicosActivos = tecnicos.Count(u => u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow);

        var repuestos = await _context.Repuestos.AsNoTracking().ToListAsync();
        var valorTotalAlmacen = repuestos.Sum(r => r.StockDisponible * r.Precio);
        var categoriasActivas = repuestos.Select(r => r.Tipo).Where(t => !string.IsNullOrEmpty(t)).Distinct().Count();
        var alertasStock = repuestos.Count(r => r.StockDisponible <= 5);
        var articulosEnExistencia = repuestos.Sum(r => r.StockDisponible);

        var reparacionesEnCurso = await _context.OrdenesReparacion
            .AsNoTracking()
            .CountAsync(o => o.Estado == "En reparacion");

        var reparacionesPendientes = await _context.OrdenesReparacion
            .AsNoTracking()
            .CountAsync(o => o.Estado == "Pendiente");

        var viewModel = new AdminDashboardViewModel
        {
            TotalUsuarios = totalUsuarios,
            TecnicosActivos = tecnicosActivos,
            ValorTotalAlmacen = valorTotalAlmacen,
            CategoriasActivas = categoriasActivas,
            ReparacionesEnCurso = reparacionesEnCurso,
            ReparacionesPendientes = reparacionesPendientes,
            AlertasStock = alertasStock,
            ArticulosEnExistencia = articulosEnExistencia
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Usuarios(string? terminoBusqueda, string? rolSeleccionado, string? estadoSeleccionado)
    {
        var usuariosDb = await _userManager.Users.AsNoTracking().ToListAsync();
        var listaUsuarios = new List<UsuarioItemViewModel>();

        foreach (var u in usuariosDb)
        {
            var roles = await _userManager.GetRolesAsync(u);
            var rol = roles.FirstOrDefault() ?? "Sin Rol";
            var esActivo = u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow;

            listaUsuarios.Add(new UsuarioItemViewModel
            {
                Id = u.Id,
                Nombre = u.Nombre,
                UserName = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                Telefono = u.PhoneNumber,
                DUI = u.DUI,
                Sexo = u.Sexo,
                FechaNacimiento = u.FechaNacimiento,
                Direccion = u.Direccion,
                Rol = rol,
                EsActivo = esActivo
            });
        }

        var filtrados = listaUsuarios.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(terminoBusqueda))
        {
            var termino = terminoBusqueda.Trim();
            filtrados = filtrados.Where(u =>
                u.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                u.UserName.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                u.DUI.Contains(termino, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(rolSeleccionado))
        {
            filtrados = filtrados.Where(u =>
                string.Equals(u.Rol, rolSeleccionado, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(estadoSeleccionado))
        {
            if (estadoSeleccionado.Equals("Activo", StringComparison.OrdinalIgnoreCase))
            {
                filtrados = filtrados.Where(u => u.EsActivo);
            }
            else if (estadoSeleccionado.Contains("Inactivo", StringComparison.OrdinalIgnoreCase) || estadoSeleccionado.Contains("Bloqueado", StringComparison.OrdinalIgnoreCase))
            {
                filtrados = filtrados.Where(u => !u.EsActivo);
            }
        }

        var viewModel = new GestionUsuariosViewModel
        {
            Usuarios = filtrados.ToList(),
            NuevoUsuario = new CrearUsuarioViewModel(),
            TerminoBusqueda = terminoBusqueda,
            RolSeleccionado = rolSeleccionado,
            EstadoSeleccionado = estadoSeleccionado
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearUsuario(CrearUsuarioViewModel nuevoUsuario)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Por favor, complete todos los campos requeridos correctamente.";
            return await RecargarVistaUsuarios(nuevoUsuario);
        }

        var existingEmail = await _userManager.FindByEmailAsync(nuevoUsuario.Email.Trim());
        if (existingEmail != null)
        {
            ModelState.AddModelError("NuevoUsuario.Email", "El correo electrónico ya se encuentra registrado.");
            TempData["Error"] = "El correo electrónico ya se encuentra registrado.";
            return await RecargarVistaUsuarios(nuevoUsuario);
        }

        var existingDui = await _context.Users.AnyAsync(u => u.DUI == nuevoUsuario.DUI.Trim());
        if (existingDui)
        {
            ModelState.AddModelError("NuevoUsuario.DUI", "El DUI ya se encuentra registrado.");
            TempData["Error"] = "El DUI ya se encuentra registrado.";
            return await RecargarVistaUsuarios(nuevoUsuario);
        }

        var user = new Usuarios
        {
            UserName = nuevoUsuario.Email.Trim(),
            Email = nuevoUsuario.Email.Trim(),
            Nombre = nuevoUsuario.Nombre.Trim(),
            DUI = nuevoUsuario.DUI.Trim(),
            PhoneNumber = nuevoUsuario.Telefono?.Trim(),
            Sexo = nuevoUsuario.Sexo,
            FechaNacimiento = nuevoUsuario.FechaNacimiento,
            Direccion = nuevoUsuario.Direccion?.Trim(),
            EmailConfirmed = true,
            DebeCambiarPassword = false
        };

        var result = await _userManager.CreateAsync(user, nuevoUsuario.Password);
        if (!result.Succeeded)
        {
            var errorMsg = string.Join(" ", result.Errors.Select(e => e.Description));
            TempData["Error"] = errorMsg;
            return await RecargarVistaUsuarios(nuevoUsuario);
        }

        var rol = string.IsNullOrWhiteSpace(nuevoUsuario.Rol) ? "Tecnico" : nuevoUsuario.Rol;
        if (!await _roleManager.RoleExistsAsync(rol))
        {
            await _roleManager.CreateAsync(new IdentityRole(rol));
        }
        await _userManager.AddToRoleAsync(user, rol);

        TempData["Success"] = $"Usuario {user.Nombre} creado exitosamente.";
        return RedirectToAction(nameof(Usuarios));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["Error"] = "Usuario no encontrado.";
            return RedirectToAction(nameof(Usuarios));
        }

        if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow)
        {
            await _userManager.SetLockoutEndDateAsync(user, null);
            TempData["Success"] = $"Usuario {user.Nombre} activado exitosamente.";
        }
        else
        {
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            TempData["Success"] = $"Usuario {user.Nombre} desactivado exitosamente.";
        }

        return RedirectToAction(nameof(Usuarios));
    }

    private async Task<IActionResult> RecargarVistaUsuarios(CrearUsuarioViewModel nuevoUsuario)
    {
        var usuariosDb = await _userManager.Users.AsNoTracking().ToListAsync();
        var listaUsuarios = new List<UsuarioItemViewModel>();

        foreach (var u in usuariosDb)
        {
            var roles = await _userManager.GetRolesAsync(u);
            var rol = roles.FirstOrDefault() ?? "Sin Rol";
            var esActivo = u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow;

            listaUsuarios.Add(new UsuarioItemViewModel
            {
                Id = u.Id,
                Nombre = u.Nombre,
                UserName = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                Telefono = u.PhoneNumber,
                DUI = u.DUI,
                Sexo = u.Sexo,
                FechaNacimiento = u.FechaNacimiento,
                Direccion = u.Direccion,
                Rol = rol,
                EsActivo = esActivo
            });
        }

        var viewModel = new GestionUsuariosViewModel
        {
            Usuarios = listaUsuarios,
            NuevoUsuario = nuevoUsuario
        };

        return View("Usuarios", viewModel);
    }

    public async Task<IActionResult> Inventario(string? terminoBusqueda, string? categoriaSeleccionada, string? estadoSeleccionado)
    {
        // 1. Categorías disponibles
        var categoriasDb = await _context.Categorias.AsNoTracking().Select(c => c.Nombre).ToListAsync();
        var categoriasEnRepuestos = await _context.Repuestos.AsNoTracking().Select(r => r.Tipo).Where(t => !string.IsNullOrEmpty(t)).Distinct().ToListAsync();
        var todasCategorias = categoriasDb.Union(categoriasEnRepuestos).OrderBy(c => c).ToList();

        // 2. Repuestos
        var repuestosDb = await _context.Repuestos
            .Include(r => r.HistorialUso)
            .AsNoTracking()
            .ToListAsync();

        // 3. KPIs
        var totalRepuestos = repuestosDb.Count;
        var totalUnidades = repuestosDb.Sum(r => r.StockDisponible);
        var tiposRegistrados = todasCategorias.Count;
        var valorTotalEstimado = repuestosDb.Sum(r => r.StockDisponible * r.Precio);
        var stockBajo = repuestosDb.Count(r => r.StockDisponible > 0 && r.StockDisponible < 3);
        var agotado = repuestosDb.Count(r => r.StockDisponible == 0);

        // 4. Filtrado
        var filtrados = repuestosDb.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(terminoBusqueda))
        {
            var termino = terminoBusqueda.Trim();
            filtrados = filtrados.Where(r =>
                r.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                r.Tipo.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                r.Id.ToString().Contains(termino) ||
                $"#REP-{r.Id}".Contains(termino, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(categoriaSeleccionada))
        {
            filtrados = filtrados.Where(r =>
                string.Equals(r.Tipo, categoriaSeleccionada, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(estadoSeleccionado))
        {
            if (estadoSeleccionado.Equals("En Stock", StringComparison.OrdinalIgnoreCase))
            {
                filtrados = filtrados.Where(r => r.StockDisponible >= 3);
            }
            else if (estadoSeleccionado.Equals("Stock Bajo", StringComparison.OrdinalIgnoreCase))
            {
                filtrados = filtrados.Where(r => r.StockDisponible > 0 && r.StockDisponible < 3);
            }
            else if (estadoSeleccionado.Equals("Agotado", StringComparison.OrdinalIgnoreCase))
            {
                filtrados = filtrados.Where(r => r.StockDisponible == 0);
            }
        }

        var listaRepuestos = filtrados.Select(r => new RepuestoItemViewModel
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Tipo = r.Tipo,
            Precio = r.Precio,
            StockDisponible = r.StockDisponible,
            PuedeEliminarse = !r.HistorialUso.Any()
        }).ToList();

        var viewModel = new GestionInventarioViewModel
        {
            TotalRepuestos = totalRepuestos,
            TotalUnidades = totalUnidades,
            TiposRegistrados = tiposRegistrados,
            ValorTotalEstimado = valorTotalEstimado,
            StockBajo = stockBajo,
            Agotado = agotado,
            TerminoBusqueda = terminoBusqueda,
            CategoriaSeleccionada = categoriaSeleccionada,
            EstadoSeleccionado = estadoSeleccionado,
            CategoriasDisponibles = todasCategorias,
            Repuestos = listaRepuestos,
            NuevoRepuesto = new CrearRepuestoViewModel(),
            NuevaCategoria = new CrearCategoriaViewModel()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearRepuesto(CrearRepuestoViewModel nuevoRepuesto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Por favor, complete todos los campos requeridos para el repuesto.";
            return RedirectToAction(nameof(Inventario));
        }

        var repuesto = new Repuestos
        {
            Nombre = nuevoRepuesto.Nombre.Trim(),
            Tipo = nuevoRepuesto.Tipo.Trim(),
            Precio = nuevoRepuesto.Precio,
            StockDisponible = nuevoRepuesto.StockDisponible
        };

        _context.Repuestos.Add(repuesto);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Repuesto '{repuesto.Nombre}' registrado exitosamente en el inventario.";
        return RedirectToAction(nameof(Inventario));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarRepuesto(EditarRepuestoViewModel repuestoEditado)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Por favor, verifique los datos del repuesto antes de guardar.";
            return RedirectToAction(nameof(Inventario));
        }

        var repuesto = await _context.Repuestos.FindAsync(repuestoEditado.Id);
        if (repuesto == null)
        {
            TempData["Error"] = "El repuesto no fue encontrado.";
            return RedirectToAction(nameof(Inventario));
        }

        repuesto.Nombre = repuestoEditado.Nombre.Trim();
        repuesto.Tipo = repuestoEditado.Tipo.Trim();
        repuesto.Precio = repuestoEditado.Precio;
        repuesto.StockDisponible = repuestoEditado.StockDisponible;

        await _context.SaveChangesAsync();

        TempData["Success"] = $"Repuesto '{repuesto.Nombre}' actualizado exitosamente.";
        return RedirectToAction(nameof(Inventario));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AjustarStock(int id, int cantidad)
    {
        if (cantidad <= 0)
        {
            TempData["Error"] = "La cantidad a añadir debe ser mayor que cero.";
            return RedirectToAction(nameof(Inventario));
        }

        var repuesto = await _context.Repuestos.FindAsync(id);
        if (repuesto == null)
        {
            TempData["Error"] = "El repuesto no fue encontrado.";
            return RedirectToAction(nameof(Inventario));
        }

        repuesto.StockDisponible += cantidad;
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Se agregaron {cantidad} unidades a '{repuesto.Nombre}'. Stock disponible actual: {repuesto.StockDisponible}.";
        return RedirectToAction(nameof(Inventario));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarRepuesto(int id)
    {
        var repuesto = await _context.Repuestos
            .Include(r => r.HistorialUso)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (repuesto == null)
        {
            TempData["Error"] = "El repuesto no fue encontrado.";
            return RedirectToAction(nameof(Inventario));
        }

        if (repuesto.HistorialUso.Any())
        {
            TempData["Error"] = $"No se puede eliminar '{repuesto.Nombre}' porque ya está registrado en órdenes de reparación.";
            return RedirectToAction(nameof(Inventario));
        }

        _context.Repuestos.Remove(repuesto);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Repuesto '{repuesto.Nombre}' eliminado del inventario exitosamente.";
        return RedirectToAction(nameof(Inventario));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearCategoria(CrearCategoriaViewModel nuevaCategoria)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "El nombre de la categoría es obligatorio.";
            return RedirectToAction(nameof(Inventario));
        }

        var nombreLimpio = nuevaCategoria.Nombre.Trim();
        var existe = await _context.Categorias.AnyAsync(c => c.Nombre.ToLower() == nombreLimpio.ToLower());
        if (existe)
        {
            TempData["Error"] = $"La categoría '{nombreLimpio}' ya se encuentra registrada.";
            return RedirectToAction(nameof(Inventario));
        }

        var categoria = new Categorias
        {
            Nombre = nombreLimpio,
            Descripcion = nuevaCategoria.Descripcion?.Trim()
        };

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Categoría '{categoria.Nombre}' agregada exitosamente.";
        return RedirectToAction(nameof(Inventario));
    }
}
