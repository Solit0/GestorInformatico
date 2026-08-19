using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestorInformatico.Data;
using GestorInformatico.Models.ViewModels.Inventario;

namespace GestorInformatico.Controllers;

public class TecnicoController : Controller
{
    private readonly GestorDbContext _context;

    public TecnicoController(GestorDbContext context)
    {
        _context = context;
    }

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
                Tipo = r.Tipo,
                Precio = r.Precio,
                StockDisponible = r.StockDisponible
            }).ToList()
        };

        return View(viewModel);
    }

    public IActionResult CambiarPassword()
    {
        return View();
    }
}
