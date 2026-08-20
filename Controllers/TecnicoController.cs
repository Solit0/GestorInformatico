using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GestorInformatico.Data;
using GestorInformatico.Models;
using GestorInformatico.Models.ViewModels;
using GestorInformatico.Models.ViewModels.ClientesEquipos;
using GestorInformatico.Models.ViewModels.FlujoTrabajo;
using GestorInformatico.Models.ViewModels.Inventario;
using GestorInformatico.Models.ViewModels.Ordenes;
using GestorInformatico.Models.ViewModels.LoginViewModels;

namespace GestorInformatico.Controllers;

[Authorize]
public class TecnicoController : Controller
{
    private readonly GestorDbContext _context;
    private readonly UserManager<Usuarios> _userManager;

    public TecnicoController(GestorDbContext context, UserManager<Usuarios> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ==================== DASHBOARD ====================
    public async Task<IActionResult> Index()
    {
        var ordenes = await _context.OrdenesReparacion.AsNoTracking().ToListAsync();

        var viewModel = new TecnicoDashboardViewModel
        {
            OrdenesPendientes = ordenes.Count(o => o.Estado == "Pendiente"),
            OrdenesEnReparacion = ordenes.Count(o => o.Estado == "En reparacion"),
            ListasParaEntrega = ordenes.Count(o => o.Estado == "Reparado"),
            StockCritico = await _context.Repuestos.AsNoTracking()
                .CountAsync(r => r.StockDisponible > 0 && r.StockDisponible < 3)
        };

        return View(viewModel);
    }

    // ==================== CLIENTES Y EQUIPOS ====================
    public async Task<IActionResult> ClientesEquipos(string? terminoBusqueda, string? marcaSeleccionada)
    {
        var clientesDb = await _context.Clientes
            .Include(c => c.Equipos)
            .AsNoTracking()
            .ToListAsync();

        var equiposDb = await _context.Equipos
            .Include(e => e.Cliente)
            .Include(e => e.HistorialReparaciones)
            .AsNoTracking()
            .ToListAsync();

        // Filtrar clientes
        var clientesFiltrados = clientesDb.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(terminoBusqueda))
        {
            var termino = terminoBusqueda.Trim();
            clientesFiltrados = clientesFiltrados.Where(c =>
                c.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                c.DUI.Contains(termino) ||
                c.Telefono.Contains(termino) ||
                c.Email.Contains(termino, StringComparison.OrdinalIgnoreCase));
        }

        var clientes = clientesFiltrados.Select(c => new ClientesViewModel
        {
            Id = c.Id,
            Nombre = c.Nombre,
            DUI = c.DUI,
            Telefono = c.Telefono,
            Email = c.Email,
            Direccion = c.Direccion,
            CantidadEquipos = c.Equipos.Count
        }).ToList();

        // Filtrar equipos
        var equiposFiltrados = equiposDb.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(marcaSeleccionada))
        {
            equiposFiltrados = equiposFiltrados.Where(e =>
                string.Equals(e.Marca, marcaSeleccionada, StringComparison.OrdinalIgnoreCase));
        }

        var equipos = equiposFiltrados.Select(e => new EquiposViewModel
        {
            Id = e.Id,
            Nombre = e.Nombre,
            Marca = e.Marca,
            Modelo = e.Modelo,
            NumeroSerie = e.NumeroSerie,
            NombreCliente = e.Cliente?.Nombre ?? "Sin cliente",
            CantidadReparaciones = e.HistorialReparaciones.Count
        }).ToList();

        var marcas = equiposDb.Select(e => e.Marca).Distinct().OrderBy(m => m);

        var viewModel = new ClientesEquiposVM
        {
            TotalClientes = clientes.Count,
            TotalEquipos = equipos.Count,
            TerminoBusqueda = terminoBusqueda,
            MarcaSeleccionada = marcaSeleccionada,
            ListaMarcas = marcas.Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = m,
                Value = m,
                Selected = string.Equals(m, marcaSeleccionada, StringComparison.OrdinalIgnoreCase)
            }),
            Clientes = clientes,
            Equipos = equipos,
            NuevoCliente = new CrearClienteFormViewModel(),
            NuevoEquipo = new CrearEquipoFormViewModel
            {
                ListaClientes = clientesDb.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                })
            }
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearCliente(CrearClienteFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return await ReconstruirClientesEquipos(formClienteFallido: model);
        }

        var cliente = new Clientes
        {
            Nombre = model.Nombre.Trim(),
            DUI = model.DUI.Trim(),
            Telefono = model.Telefono.Trim(),
            Email = model.Email.Trim(),
            Direccion = model.Direccion.Trim(),
            RecibeNotificacionesCorreo = model.RecibeNotificacionesCorreo
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Cliente '{cliente.Nombre}' registrado exitosamente.";
        return RedirectToAction(nameof(ClientesEquipos));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearEquipo(ClientesEquiposVM model)
    {
        var equipoModel = model.NuevoEquipo;
        if (equipoModel == null || !ModelState.IsValid)
        {
            return await ReconstruirClientesEquipos(formEquipoFallido: equipoModel);
        }

        var equipo = new Equipos
        {
            ClienteId = equipoModel.ClienteId,
            Nombre = equipoModel.Nombre.Trim(),
            Marca = equipoModel.Marca.Trim(),
            Modelo = equipoModel.Modelo.Trim(),
            NumeroSerie = equipoModel.NumeroSerie.Trim()
        };

        _context.Equipos.Add(equipo);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Equipo '{equipo.Nombre}' registrado exitosamente.";
        return RedirectToAction(nameof(ClientesEquipos));
    }

    private async Task<IActionResult> ReconstruirClientesEquipos(CrearClienteFormViewModel? formClienteFallido = null, CrearEquipoFormViewModel? formEquipoFallido = null)
    {
        var clientesDb = await _context.Clientes
            .Include(c => c.Equipos)
            .AsNoTracking().ToListAsync();
        var equiposDb = await _context.Equipos
            .Include(e => e.Cliente)
            .Include(e => e.HistorialReparaciones)
            .AsNoTracking().ToListAsync();

        var clientes = clientesDb.Select(c => new ClientesViewModel
        {
            Id = c.Id,
            Nombre = c.Nombre,
            DUI = c.DUI,
            Telefono = c.Telefono,
            Email = c.Email,
            Direccion = c.Direccion,
            CantidadEquipos = c.Equipos.Count
        }).ToList();

        var equipos = equiposDb.Select(e => new EquiposViewModel
        {
            Id = e.Id,
            Nombre = e.Nombre,
            Marca = e.Marca,
            Modelo = e.Modelo,
            NumeroSerie = e.NumeroSerie,
            NombreCliente = e.Cliente?.Nombre ?? "Sin cliente",
            CantidadReparaciones = e.HistorialReparaciones.Count
        }).ToList();

        var viewModel = new ClientesEquiposVM
        {
            TotalClientes = clientesDb.Count,
            TotalEquipos = equiposDb.Count,
            Clientes = clientes,
            Equipos = equipos,
            NuevoCliente = formClienteFallido ?? new CrearClienteFormViewModel(),
            NuevoEquipo = formEquipoFallido ?? new CrearEquipoFormViewModel
            {
                ListaClientes = clientesDb.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                })
            },
            ModalAbierto = formClienteFallido != null ? "modalCliente" : (formEquipoFallido != null ? "modalEquipo" : null)
        };

        return View("ClientesEquipos", viewModel);
    }

    // ==================== RECEPCION DE ORDENES ====================
    public async Task<IActionResult> RecepcionOrdenes(string? terminoBusqueda = null)
    {
        var query = _context.OrdenesReparacion
            .Include(o => o.Equipo).ThenInclude(e => e.Cliente)
            .Include(o => o.Tecnico)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(terminoBusqueda))
        {
            var busqueda = terminoBusqueda.Trim().ToLower();
            query = query.Where(o =>
                o.Id.ToString().Contains(busqueda) ||
                (o.Equipo != null && o.Equipo.Cliente != null && o.Equipo.Cliente.Nombre.ToLower().Contains(busqueda)) ||
                (o.Equipo != null && o.Equipo.Nombre.ToLower().Contains(busqueda)) ||
                (o.Equipo != null && o.Equipo.Modelo.ToLower().Contains(busqueda)));
        }

        var ordenesRecientes = await query
            .OrderByDescending(o => o.FechaIngreso)
            .Take(20)
            .Select(o => new OrdenRecienteViewModel
            {
                Id = o.Id,
                FechaIngreso = o.FechaIngreso,
                NombreCliente = o.Equipo != null && o.Equipo.Cliente != null ? o.Equipo.Cliente.Nombre : "N/A",
                NombreEquipo = o.Equipo != null ? $"{o.Equipo.Nombre} - {o.Equipo.Modelo}" : "N/A",
                NombreTecnico = o.Tecnico != null ? o.Tecnico.Nombre : "Sin asignar",
                Estado = o.Estado
            })
            .ToListAsync();

        var clientes = await _context.Clientes.AsNoTracking().ToListAsync();
        var equipos = await _context.Equipos.Include(e => e.Cliente).AsNoTracking().ToListAsync();
        var tecnicos = await _userManager.GetUsersInRoleAsync("Tecnico");
        var adminUsers = await _userManager.GetUsersInRoleAsync("Administrador");
        var todosTecnicos = tecnicos.Concat(adminUsers).ToList();

        var viewModel = new RecepcionOrdenesViewModel
        {
            ListaClientes = clientes.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = $"{c.Nombre} ({c.DUI})",
                Value = c.Id.ToString()
            }),
            ListaEquipos = equipos.Select(e => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = $"{e.Nombre} ({e.Marca} {e.Modelo}) - {e.NumeroSerie}",
                Value = e.Id.ToString()
            }),
            ListaTecnicos = todosTecnicos.Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = t.Nombre,
                Value = t.Id
            }),
            ListaEstados = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
            {
                new("Pendiente", "Pendiente", true)
            },
            FormularioNuevaOrden = new FormNuevaOrdenViewModel
            {
                FechaIngreso = DateTime.Now
            },
            OrdenesRecientes = ordenesRecientes
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearOrden(FormNuevaOrdenViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return await ReconstruirRecepcionOrdenes(model);
        }

        var orden = new OrdenReparacion
        {
            EquipoId = model.EquipoId,
            TecnicoId = model.TecnicoId,
            FechaIngreso = model.FechaIngreso,
            Descripcion = model.Descripcion.Trim(),
            Observaciones = model.Observaciones?.Trim(),
            Estado = model.Estado,
            NotificacionEnviada = false
        };

        _context.OrdenesReparacion.Add(orden);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Orden de reparación #{orden.Id} creada exitosamente.";
        return RedirectToAction(nameof(RecepcionOrdenes));
    }

    private async Task<IActionResult> ReconstruirRecepcionOrdenes(FormNuevaOrdenViewModel? formFallido = null)
    {
        var clientes = await _context.Clientes.AsNoTracking().ToListAsync();
        var equipos = await _context.Equipos.Include(e => e.Cliente).AsNoTracking().ToListAsync();
        var tecnicos = await _userManager.GetUsersInRoleAsync("Tecnico");
        var adminUsers = await _userManager.GetUsersInRoleAsync("Administrador");
        var todosTecnicos = tecnicos.Concat(adminUsers).ToList();

        var query = _context.OrdenesReparacion
            .Include(o => o.Equipo).ThenInclude(e => e.Cliente)
            .Include(o => o.Tecnico)
            .AsQueryable();

        var ordenesRecientes = await query
            .OrderByDescending(o => o.FechaIngreso)
            .Take(20)
            .Select(o => new OrdenRecienteViewModel
            {
                Id = o.Id,
                FechaIngreso = o.FechaIngreso,
                NombreCliente = o.Equipo != null && o.Equipo.Cliente != null ? o.Equipo.Cliente.Nombre : "N/A",
                NombreEquipo = o.Equipo != null ? $"{o.Equipo.Nombre} - {o.Equipo.Modelo}" : "N/A",
                NombreTecnico = o.Tecnico != null ? o.Tecnico.Nombre : "Sin asignar",
                Estado = o.Estado
            })
            .ToListAsync();

        var viewModel = new RecepcionOrdenesViewModel
        {
            ListaClientes = clientes.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = $"{c.Nombre} ({c.DUI})",
                Value = c.Id.ToString()
            }),
            ListaEquipos = equipos.Select(e => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = $"{e.Nombre} ({e.Marca} {e.Modelo}) - {e.NumeroSerie}",
                Value = e.Id.ToString()
            }),
            ListaTecnicos = todosTecnicos.Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = t.Nombre,
                Value = t.Id
            }),
            ListaEstados = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
            {
                new("Pendiente", "Pendiente", true)
            },
            FormularioNuevaOrden = formFallido ?? new FormNuevaOrdenViewModel
            {
                FechaIngreso = DateTime.Now
            },
            OrdenesRecientes = ordenesRecientes
        };

        return View("RecepcionOrdenes", viewModel);
    }

    // ==================== FLUJO DE TRABAJO (KANBAN) ====================
    public async Task<IActionResult> FlujoTrabajo(string? terminoBusqueda, string? tecnicoSeleccionado)
    {
        var ordenesQuery = _context.OrdenesReparacion
            .Include(o => o.Equipo).ThenInclude(e => e.Cliente)
            .Include(o => o.Tecnico)
            .Include(o => o.RepuestosUsados)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(terminoBusqueda))
        {
            var termino = terminoBusqueda.Trim();
            ordenesQuery = ordenesQuery.Where(o =>
                o.Id.ToString().Contains(termino) ||
                (o.Equipo != null && o.Equipo.Nombre.Contains(termino)) ||
                (o.Equipo != null && o.Equipo.Cliente != null && o.Equipo.Cliente.Nombre.Contains(termino)));
        }

        if (!string.IsNullOrWhiteSpace(tecnicoSeleccionado))
        {
            ordenesQuery = ordenesQuery.Where(o => o.TecnicoId == tecnicoSeleccionado);
        }

        var ordenes = await ordenesQuery.ToListAsync();

        var ordenesVm = ordenes.Select(o => new EstadosReparacionViewModel
        {
            Id = o.Id,
            Estado = o.Estado,
            NombreCliente = o.Equipo?.Cliente?.Nombre ?? "Sin cliente",
            NombreEquipo = o.Equipo?.Nombre ?? "Sin equipo",
            NombreTecnico = o.Tecnico?.Nombre ?? "Sin técnico",
            Descripcion = o.Descripcion ?? "",
            FechaActualizacion = o.FechaSalida ?? o.FechaIngreso,
            CantidadRepuestos = o.RepuestosUsados.Count
        }).ToList();

        var tecnicos = await _userManager.GetUsersInRoleAsync("Tecnico");
        var adminUsers = await _userManager.GetUsersInRoleAsync("Administrador");
        var todosTecnicos = tecnicos.Concat(adminUsers).ToList();

        var viewModel = new FlujoVM
        {
            TerminoBusqueda = terminoBusqueda,
            TecnicoSeleccionado = tecnicoSeleccionado,
            ListaTecnicos = todosTecnicos.Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = t.Nombre,
                Value = t.Id,
                Selected = t.Id == tecnicoSeleccionado
            }),
            OrdenesPendientes = ordenesVm.Where(o => o.Estado == "Pendiente").ToList(),
            OrdenesEnReparacion = ordenesVm.Where(o => o.Estado == "En reparacion").ToList(),
            OrdenesReparadas = ordenesVm.Where(o => o.Estado == "Reparado").ToList(),
            OrdenesNoSePudo = ordenesVm.Where(o => o.Estado == "No se pudo").ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstadoOrden(int id, string nuevoEstado)
    {
        var orden = await _context.OrdenesReparacion.FindAsync(id);
        if (orden == null)
        {
            TempData["Error"] = "Orden no encontrada.";
            return RedirectToAction(nameof(FlujoTrabajo));
        }

        var estadosValidos = new[] { "Pendiente", "En reparacion", "Reparado", "No se pudo" };
        if (!estadosValidos.Contains(nuevoEstado))
        {
            TempData["Error"] = "Estado no válido.";
            return RedirectToAction(nameof(FlujoTrabajo));
        }

        orden.Estado = nuevoEstado;
        if (nuevoEstado == "Reparado" || nuevoEstado == "No se pudo")
        {
            orden.FechaSalida = DateTime.Now;
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = $"Orden #{orden.Id} cambiada a '{nuevoEstado}'.";
        return RedirectToAction(nameof(FlujoTrabajo));
    }

    // ==================== CAMBIAR CONTRASEÑA ====================
    public IActionResult CambiarPassword()
    {
        return View(new CambiarClaveViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarPassword(CambiarClaveViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["Error"] = "Usuario no encontrado.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _userManager.ChangePasswordAsync(user, model.ContrasenaActual, model.NuevaContrasena);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        if (user.DebeCambiarPassword)
        {
            user.DebeCambiarPassword = false;
            await _userManager.UpdateAsync(user);
        }

        TempData["Success"] = "Contraseña actualizada exitosamente.";
        return RedirectToAction(nameof(Index));
    }

    // ==================== ASIGNAR REPUESTO A ORDEN ====================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AsignarRepuesto(int ordenReparacionId, int repuestoId, int cantidad, string? nota)
    {
        if (ordenReparacionId <= 0 || repuestoId <= 0 || cantidad <= 0)
        {
            return await ReconstruirInventario(formFallido: new AsignarRepuestoViewModel
            {
                OrdenReparacionId = ordenReparacionId,
                RepuestoId = repuestoId,
                Cantidad = cantidad,
                Nota = nota
            });
        }

        var orden = await _context.OrdenesReparacion.FindAsync(ordenReparacionId);
        if (orden == null)
        {
            TempData["Error"] = "Orden no encontrada.";
            return RedirectToAction(nameof(Inventario));
        }

        var repuesto = await _context.Repuestos.FindAsync(repuestoId);
        if (repuesto == null)
        {
            TempData["Error"] = "Repuesto no encontrado.";
            return RedirectToAction(nameof(Inventario));
        }

        if (repuesto.StockDisponible < cantidad)
        {
            return await ReconstruirInventario(formFallido: new AsignarRepuestoViewModel
            {
                OrdenReparacionId = ordenReparacionId,
                RepuestoId = repuestoId,
                Cantidad = cantidad,
                Nota = nota
            }, errorStock: $"Stock insuficiente. Disponible: {repuesto.StockDisponible}.");
        }

        var detalle = new DetalleOrdenRepuesto
        {
            OrdenReparacionId = ordenReparacionId,
            RepuestoId = repuestoId,
            Cantidad = cantidad,
            PrecioUnitario = repuesto.Precio
        };

        repuesto.StockDisponible -= cantidad;
        _context.DetallesOrdenRepuesto.Add(detalle);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Repuesto '{repuesto.Nombre}' (x{cantidad}) asignado a la orden #{orden.Id}.";
        return RedirectToAction(nameof(Inventario));
    }

    private async Task<IActionResult> ReconstruirInventario(AsignarRepuestoViewModel? formFallido = null, string? errorStock = null)
    {
        var todosRepuestos = await _context.Repuestos.AsNoTracking().ToListAsync();

        var totalRepuestos = todosRepuestos.Sum(r => r.StockDisponible);
        var stockBajo = todosRepuestos.Count(r => r.StockDisponible > 0 && r.StockDisponible < 3);
        var agotado = todosRepuestos.Count(r => r.StockDisponible == 0);

        var ordenesActivas = await _context.OrdenesReparacion
            .Where(o => o.Estado == "Pendiente" || o.Estado == "En reparacion")
            .Include(o => o.Equipo).ThenInclude(e => e.Cliente)
            .AsNoTracking()
            .ToListAsync();

        if (errorStock != null)
        {
            ModelState.AddModelError(string.Empty, errorStock);
        }

        var formulario = formFallido ?? new AsignarRepuestoViewModel();
        formulario.ListaOrdenes = ordenesActivas.Select(o => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Text = $"#ORD-{o.Id} - {o.Equipo?.Cliente?.Nombre ?? "?"} ({o.Equipo?.Nombre ?? "?"})",
            Value = o.Id.ToString(),
            Selected = o.Id == formulario.OrdenReparacionId
        });
        formulario.ListaRepuestos = todosRepuestos.Where(r => r.StockDisponible > 0).Select(r => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Text = $"{r.Nombre} (${r.Precio:F2})",
            Value = r.Id.ToString(),
            Selected = r.Id == formulario.RepuestoId
        });

        var viewModel = new InventarioVM
        {
            TotalRepuestos = totalRepuestos,
            StockBajo = stockBajo,
            Agotado = agotado,
            Repuestos = todosRepuestos.Select(r => new VerInventarioViewModel
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Descripcion = r.Tipo,
                Tipo = r.Tipo,
                Precio = r.Precio,
                StockDisponible = r.StockDisponible
            }).ToList(),
            FormularioAsignar = formulario,
            ModalAbierto = "modalAsignarRepuesto"
        };

        return View("Inventario", viewModel);
    }

    // ==================== INVENTARIO (con modal de asignar) ====================
    public async Task<IActionResult> Inventario(string? terminoBusqueda, string? categoriaSeleccionada)
    {
        var todosRepuestos = await _context.Repuestos.AsNoTracking().ToListAsync();

        var totalRepuestos = todosRepuestos.Sum(r => r.StockDisponible);
        var stockBajo = todosRepuestos.Count(r => r.StockDisponible > 0 && r.StockDisponible < 3);
        var agotado = todosRepuestos.Count(r => r.StockDisponible == 0);

        var repuestosFiltrados = todosRepuestos.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(terminoBusqueda))
        {
            var termino = terminoBusqueda.Trim();
            repuestosFiltrados = repuestosFiltrados.Where(r =>
                r.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                r.Tipo.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                r.Id.ToString().Contains(termino) ||
                $"#REP-{r.Id}".Contains(termino, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(categoriaSeleccionada))
        {
            repuestosFiltrados = repuestosFiltrados.Where(r =>
                string.Equals(r.Tipo, categoriaSeleccionada, StringComparison.OrdinalIgnoreCase));
        }

        var ordenesActivas = await _context.OrdenesReparacion
            .Where(o => o.Estado == "Pendiente" || o.Estado == "En reparacion")
            .Include(o => o.Equipo).ThenInclude(e => e.Cliente)
            .AsNoTracking()
            .ToListAsync();

        var viewModel = new InventarioVM
        {
            TotalRepuestos = totalRepuestos,
            StockBajo = stockBajo,
            Agotado = agotado,
            TerminoBusqueda = terminoBusqueda,
            CategoriaSeleccionada = categoriaSeleccionada,
            Repuestos = repuestosFiltrados.Select(r => new VerInventarioViewModel
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Descripcion = r.Tipo,
                Tipo = r.Tipo,
                Precio = r.Precio,
                StockDisponible = r.StockDisponible
            }).ToList(),
            FormularioAsignar = new AsignarRepuestoViewModel
            {
                ListaOrdenes = ordenesActivas.Select(o => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = $"#ORD-{o.Id} - {o.Equipo?.Cliente?.Nombre ?? "?"} ({o.Equipo?.Nombre ?? "?"})",
                    Value = o.Id.ToString()
                }),
                ListaRepuestos = todosRepuestos.Where(r => r.StockDisponible > 0).Select(r => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = $"{r.Nombre} (${r.Precio:F2})",
                    Value = r.Id.ToString()
                })
            }
        };

        return View(viewModel);
    }
}
