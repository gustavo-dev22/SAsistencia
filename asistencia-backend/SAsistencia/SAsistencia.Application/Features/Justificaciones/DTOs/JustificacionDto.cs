using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Justificaciones.DTOs
{
    public class TipoJustificacionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool RequiereDocumento { get; set; }
        public bool ConGoceHaber { get; set; }
    }

    public class JustificacionItemDto
    {
        public long Id { get; set; }
        public int EmpleadoId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Dni { get; set; }
        public string? OficinaNombre { get; set; }
        public string? OficinaSigla { get; set; }
        public int TipoJustificacionId { get; set; }
        public string TipoJustificacionNombre { get; set; } = string.Empty;
        public bool ConGoceHaber { get; set; }
        public long? MarcacionId { get; set; }
        public string FechaInicio { get; set; } = string.Empty;
        public string FechaFin { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public string? RutaArchivo { get; set; }
        public string? NombreArchivoOriginal { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? ObservacionesAprobacion { get; set; }
        public string FechaRegistro { get; set; } = string.Empty;
    }

    public class FiltroJustificacionesRequest
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? Estado { get; set; } // 'TODOS', 'PENDIENTE', 'APROBADA', 'RECHAZADA'
        public int? TipoJustificacionId { get; set; }
        public int? OficinaId { get; set; }
        public string? Busqueda { get; set; }
        public int Pagina { get; set; } = 1;
        public int RegistrosPorPagina { get; set; } = 15;
    }

    public record ResolverJustificacionRequest(
        long Id,
        string Estado, // 'APROBADA' o 'RECHAZADA'
        string? Observaciones
    );
}
