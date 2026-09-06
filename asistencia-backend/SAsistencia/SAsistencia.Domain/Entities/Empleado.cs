using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Domain.Entities
{
    public class Empleado
    {
        public int Id { get; set; }
        public string UsuarioIdSasi { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Dni { get; set; }
        public string CodigoQr { get; set; } = string.Empty;
        public int? TurnoId { get; set; }
        public bool HabilitadoParaMarcar { get; set; } = true;
        public DateTime FechaSincronizacion { get; set; } = DateTime.UtcNow;

        public int? OficinaId { get; set; }
        public Oficina? Oficina { get; set; }

        public int? CargoId { get; set; }
        public Cargo? Cargo { get; set; }

        public Turno? Turno { get; set; }
    }
}
