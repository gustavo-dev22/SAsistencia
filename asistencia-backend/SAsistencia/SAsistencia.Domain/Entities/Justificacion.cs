using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Domain.Entities
{
    public class Justificacion
    {
        public long Id { get; set; }
        public int EmpleadoId { get; set; }
        public int TipoJustificacionId { get; set; }
        public long? MarcacionId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string? RutaArchivo { get; set; }
        public string? NombreArchivoOriginal { get; set; }
        public string Estado { get; set; } = "PENDIENTE"; // PENDIENTE, APROBADA, RECHAZADA
        public string? ObservacionesAprobacion { get; set; }
        public string? UsuarioAprobadorId { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public Empleado? Empleado { get; set; }
        public TipoJustificacion? TipoJustificacion { get; set; }
        public Marcacion? Marcacion { get; set; }
    }
}
