using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Domain.Entities
{
    public class FeriadoCalendario
    {
        public int Id { get; set; }
        public int Anio { get; set; }
        public DateTime Fecha { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = "FERIADO_NACIONAL"; // FERIADO_NACIONAL, NO_LABORABLE_COMPENSABLE, FIESTA_INSTITUCIONAL
        public bool AplicaSectorPublico { get; set; } = true;
        public bool EsCompensable { get; set; } = false;
        public string? NormaLegal { get; set; }
        public DateTime? FechaCompensacionLimite { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public string? UsuarioCreacion { get; set; }
    }
}
