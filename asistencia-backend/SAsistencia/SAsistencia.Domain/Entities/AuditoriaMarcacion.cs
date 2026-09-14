using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Domain.Entities
{
    public class AuditoriaMarcacion
    {
        public long Id { get; set; }
        public long? MarcacionId { get; set; }
        public int EmpleadoId { get; set; }
        public string TipoOperacion { get; set; } = "CREACION_MANUAL"; // CREACION_MANUAL, MODIFICACION, ANULACION
        public string? HoraAnterior { get; set; }
        public string HoraNueva { get; set; } = string.Empty;
        public string TipoMarcacion { get; set; } = string.Empty;
        public string MotivoJustificacion { get; set; } = string.Empty;
        public string UsuarioResponsable { get; set; } = string.Empty;
        public string IpResponsable { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public Empleado? Empleado { get; set; }
        public Marcacion? Marcacion { get; set; }
    }
}
