using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.CalendarioLaboral.DTOs
{
    public class FeriadoItemDto
    {
        public int Id { get; set; }
        public int Anio { get; set; }
        public string Fecha { get; set; } = string.Empty; // YYYY-MM-DD
        public string FechaLegible { get; set; } = string.Empty; // dd/MM/yyyy
        public string DiaSemana { get; set; } = string.Empty; // Lunes, Martes...
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public bool AplicaSectorPublico { get; set; }
        public bool EsCompensable { get; set; }
        public string? NormaLegal { get; set; }
        public string? FechaCompensacionLimite { get; set; }
        public bool Activo { get; set; }
    }

    public record GuardarFeriadoRequest(
        int? Id,
        DateTime Fecha,
        string Nombre,
        string Tipo,
        bool AplicaSectorPublico,
        bool EsCompensable,
        string? NormaLegal,
        DateTime? FechaCompensacionLimite
    );
}
