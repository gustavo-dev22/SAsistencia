using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.AuditoriaMarcaciones.DTOs
{
    public class ItemAuditoriaMarcacionDto
    {
        public long Id { get; set; }
        public long? MarcacionId { get; set; }
        public int EmpleadoId { get; set; }
        public string NombreEmpleado { get; set; } = string.Empty;
        public string? DniEmpleado { get; set; }
        public string? OficinaSigla { get; set; }
        public string TipoOperacion { get; set; } = string.Empty;
        public string? HoraAnterior { get; set; }
        public string HoraNueva { get; set; } = string.Empty;
        public string TipoMarcacion { get; set; } = string.Empty;
        public string MotivoJustificacion { get; set; } = string.Empty;
        public string UsuarioResponsable { get; set; } = string.Empty;
        public string IpResponsable { get; set; } = string.Empty;
        public string FechaRegistro { get; set; } = string.Empty;
    }

    public class FiltroAuditoriaRequest
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? TipoOperacion { get; set; }
        public string? Busqueda { get; set; }
        public int Pagina { get; set; } = 1;
        public int RegistrosPorPagina { get; set; } = 15;
    }
}
