using GestorInformatico.Models;
using Microsoft.EntityFrameworkCore;

namespace GestorInformatico.Data;

public class GestorDbContext : DbContext
{
    public GestorDbContext(DbContextOptions<GestorDbContext> options) : base(options)
    { }
    
    public DbSet<Clientes> Clientes { get; set; }
    public DbSet<Equipos> Equipos { get; set; }
    public DbSet<Tecnicos> Tecnicos { get; set; }
    public DbSet<OrdenReparacion> OrdenesReparacion { get; set; }
    public DbSet<Repuestos> Repuestos { get; set; }
    public DbSet<DetalleOrdenRepuesto> DetallesOrdenRepuesto { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Regla 1: No se puede borrar un Cliente si tiene Equipos registrados.
        modelBuilder.Entity<Equipos>()
            .HasOne(e => e.Cliente)
            .WithMany(c => c.Equipos)
            .HasForeignKey(e => e.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Regla 2: No se puede borrar un Técnico si ya tiene órdenes de reparación asignadas.
        modelBuilder.Entity<OrdenReparacion>()
            .HasOne(o => o.Tecnico)
            .WithMany(t => t.OrdenesReparacion)
            .HasForeignKey(o => o.TecnicoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Regla 3: No se puede borrar un Repuesto del inventario si ya se usó en una factura/orden.
        modelBuilder.Entity<DetalleOrdenRepuesto>()
            .HasOne(d => d.Repuesto)
            .WithMany(r => r.HistorialUso)
            .HasForeignKey(d => d.RepuestoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}