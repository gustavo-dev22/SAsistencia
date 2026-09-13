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

    public class MarcacionEnVivoDto
    {
        public long Id { get; set; }
        public int EmpleadoId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Dni { get; set; }
        public string? OficinaNombre { get; set; }
        public string? OficinaSigla { get; set; }
        public string? CargoNombre { get; set; }
        public string Hora { get; set; } = string.Empty; // "08:15:22"
        public string TipoMarcacion { get; set; } = "ENTRADA"; // ENTRADA, SALIDA
        public string EstadoPuntualidad { get; set; } = "PUNTUAL"; // PUNTUAL, TOLERANCIA, TARDANZA, FUERA_TURNO, EXONERADO
        public int MinutosTardanza { get; set; }
        public string MetodoRegistro { get; set; } = "QR";
        public DateTime FechaHora { get; set; }
    }

    public class ResumenMarcacionesHoyDto
    {
        public int TotalMarcas { get; set; }
        public int TotalPuntuales { get; set; }
        public int TotalTardanzas { get; set; }
        public int TotalExonerados { get; set; }
        public List<MarcacionEnVivoDto> UltimasMarcaciones { get; set; } = new();
    }

    public class FiltroHistorialMarcacionesRequest
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int? EmpleadoId { get; set; }
        public int? OficinaId { get; set; }
        public string? TipoMarcacion { get; set; } // 'ENTRADA', 'SALIDA' o null
        public string? EstadoPuntualidad { get; set; } // 'PUNTUAL', 'TARDANZA', etc.
        public string? Busqueda { get; set; } // Texto libre: Nombre o DNI
        public int Pagina { get; set; } = 1;
        public int RegistrosPorPagina { get; set; } = 15;
    }

    public class ItemHistorialMarcacionDto
    {
        public long Id { get; set; }
        public int EmpleadoId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Dni { get; set; }
        public string? OficinaNombre { get; set; }
        public string? OficinaSigla { get; set; }
        public string? CargoNombre { get; set; }
        public string? TurnoNombre { get; set; }
        public DateTime FechaHora { get; set; }
        public string Fecha { get; set; } = string.Empty;
        public string Hora { get; set; } = string.Empty;
        public string TipoMarcacion { get; set; } = string.Empty;
        public string EstadoPuntualidad { get; set; } = string.Empty;
        public int MinutosTardanza { get; set; }
        public string MetodoRegistro { get; set; } = string.Empty;
        public string? IpTerminal { get; set; }
    }

    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
    }
}
