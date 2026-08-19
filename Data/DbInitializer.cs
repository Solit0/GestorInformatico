using GestorInformatico.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestorInformatico.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(GestorDbContext context, UserManager<Usuarios> userManager, RoleManager<IdentityRole> roleManager)
    {
        await context.Database.MigrateAsync();

        // 1. Roles
        string[] roles = { "Administrador", "Tecnico" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Default Administrator
        var adminEmail = "amorales@gestor.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new Usuarios
            {
                UserName = "admin_amorales",
                Email = adminEmail,
                Nombre = "Alejandro Morales",
                DUI = "01234567-8",
                PhoneNumber = "7122-3344",
                Sexo = "Masculino",
                FechaNacimiento = new DateOnly(1990, 4, 15),
                EmailConfirmed = true,
                DebeCambiarPassword = false
            };
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Administrador");
            }
        }

        // 3. Default Technician 1
        var tec1Email = "rflores@gestor.com";
        var tec1User = await userManager.FindByEmailAsync(tec1Email);
        if (tec1User == null)
        {
            tec1User = new Usuarios
            {
                UserName = "tec_rflores",
                Email = tec1Email,
                Nombre = "Roberto Flores",
                DUI = "02345678-9",
                PhoneNumber = "7844-5566",
                Sexo = "Masculino",
                FechaNacimiento = new DateOnly(1995, 9, 22),
                EmailConfirmed = true,
                DebeCambiarPassword = false
            };
            var result = await userManager.CreateAsync(tec1User, "Tecnico123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(tec1User, "Tecnico");
            }
        }

        // 4. Default Technician 2
        var tec2Email = "emartinez@gestor.com";
        var tec2User = await userManager.FindByEmailAsync(tec2Email);
        if (tec2User == null)
        {
            tec2User = new Usuarios
            {
                UserName = "tec_emartinez",
                Email = tec2Email,
                Nombre = "Elena Martínez",
                DUI = "03456789-0",
                PhoneNumber = "7955-6677",
                Sexo = "Femenino",
                FechaNacimiento = new DateOnly(1998, 11, 8),
                EmailConfirmed = true,
                DebeCambiarPassword = false
            };
            var result = await userManager.CreateAsync(tec2User, "Tecnico123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(tec2User, "Tecnico");
            }
        }

        // 5. Default Categories
        if (!await context.Categorias.AnyAsync())
        {
            var categoriasDefault = new List<Categorias>
            {
                new Categorias { Nombre = "Almacenamiento", Descripcion = "Unidades SSD, HDD y almacenamiento externo" },
                new Categorias { Nombre = "Memoria RAM", Descripcion = "Módulos de memoria RAM DDR4, DDR5 para PC y Laptop" },
                new Categorias { Nombre = "Fuentes de Poder", Descripcion = "Fuentes de alimentación y cargadores" },
                new Categorias { Nombre = "Pantallas", Descripcion = "Displays, pantallas LCD, LED y paneles" },
                new Categorias { Nombre = "Baterías", Descripcion = "Baterías para laptops y periféricos" },
                new Categorias { Nombre = "Refrigeración", Descripcion = "Disipadores, ventiladores y pasta térmica" },
                new Categorias { Nombre = "Periféricos", Descripcion = "Teclados, mouse, cables y adaptadores" }
            };

            await context.Categorias.AddRangeAsync(categoriasDefault);
            await context.SaveChangesAsync();
        }
    }
}
