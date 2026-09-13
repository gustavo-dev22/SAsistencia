using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Domain.Entities
{
    public class Marcacion
    {
        public long Id { get; set; }
        public int EmpleadoId { get; set; }
        public int? TurnoId { get; set; }
        public DateTime FechaHoraMarcacion { get; set; } = DateTime.UtcNow;
        public string TipoMarcacion { get; set; } = "ENTRADA"; // ENTRADA, SALIDA
        public string EstadoPuntualidad { get; set; } = "PUNTUAL"; // PUNTUAL, TOLERANCIA, TARDANZA, FUERA_TURNO
        public int MinutosTardanza { get; set; } = 0;
        public string MetodoRegistro { get; set; } = "DNI"; // QR, DNI
        public string? IpTerminal { get; set; }

        public Empleado? Empleado { get; set; }
        public Turno? Turno { get; set; }
    }
}
