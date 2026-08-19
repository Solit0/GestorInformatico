namespace GestorInformatico.Models.ViewModels.FlujoTrabajo;

public class EstadosReparacionViewModel
{
    public int Id { get; set; }
    public string IdFormateado => $"#ORD-{Id}";
    
    public string Estado { get; set; } 

    public string NombreCliente { get; set; }
    public string NombreEquipo { get; set; }
    public string NombreTecnico { get; set; }
    
    public string Descripcion { get; set; }
    
    public DateTime FechaActualizacion { get; set; }
    
    public int CantidadRepuestos { get; set; }

    // --- MAGIA VISUAL PARA LA INTERFAZ ---
    
    public string EtiquetaDescripcion => Estado switch
    {
        "Pendiente" => "Falla:",
        "En reparacion" => "Diagnóstico:",
        "Reparado" => "Solución:",
        "No se pudo" => "Motivo:",
        _ => "Detalle:"
    };
    
    public string FechaFormateada 
    {
        get
        {
            var hoy = DateTime.Today;
            var fecha = FechaActualizacion.Date;

            if (fecha == hoy) return $"Hoy {FechaActualizacion:HH:mm}";
            if (fecha == hoy.AddDays(-1)) return $"Ayer {FechaActualizacion:HH:mm}";
            if (fecha.Year == hoy.Year) return FechaActualizacion.ToString("dd/MM");
            
            return FechaActualizacion.ToString("dd/MM/yyyy");
        }
    }
    
    public string TextoBadgeRepuestos => CantidadRepuestos == 1 ? "1 Repuesto asignado" : $"{CantidadRepuestos} Repuestos asignados";
    
}