using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Empleados.DTOs
{
    // Respuesta de SASI para el endpoint de usuarios del sistema
    public class SasiUsuariosResponse
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public List<SasiUsuarioItemDto> Datos { get; set; } = new();
    }

    public class SasiUsuarioItemDto
    {
        public string UsuarioId { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? IdOficina { get; set; }
        public string? NombreOficina { get; set; }
        public string? SiglaOficina { get; set; }
    }

    // DTO para la tabla en Angular
    public class EmpleadoListDto
    {
        public int Id { get; set; }
        public string UsuarioIdSasi { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Dni { get; set; }
        public string CodigoQr { get; set; } = string.Empty;
        public int? TurnoId { get; set; }
        public string? TurnoNombre { get; set; }
        public bool HabilitadoParaMarcar { get; set; }

        // Datos de Oficina (procedentes de SASI y persistidos localmente)
        public int? OficinaId { get; set; }
        public string? OficinaNombre { get; set; }
        public string? OficinaSigla { get; set; }

        // Cargo (Administrado localmente)
        public int? CargoId { get; set; }
        public string? CargoNombre { get; set; }
    }

    public class ActualizarEmpleadoRequest
    {
        public int Id { get; set; }
        public string? Dni { get; set; }
        public int? TurnoId { get; set; }
        public int? CargoId { get; set; } // <-- AGREGAR CARGO ID
        public bool HabilitadoParaMarcar { get; set; }
    }
}
