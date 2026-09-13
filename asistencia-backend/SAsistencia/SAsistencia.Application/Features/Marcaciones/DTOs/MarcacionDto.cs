using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Marcaciones.DTOs
{
    public record RegistrarMarcaRequest(
        string Identificador, // Puede ser el DNI o el Token QR
        string Metodo // 'QR' o 'DNI'
    );

    public class ResultadoMarcacionDto
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string? NombreEmpleado { get; set; }
        public string? AreaEmpleado { get; set; }
        public string? CargoEmpleado { get; set; }
        public string? Hora { get; set; }
        public string? TipoMarcacion { get; set; } // 'ENTRADA' | 'SALIDA'
        public string? EstadoPuntualidad { get; set; } // 'PUNTUAL' | 'TOLERANCIA' | 'TARDANZA' | 'EXONERADO'
        public int MinutosTardanza { get; set; }
    }
}
