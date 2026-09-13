using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Turnos.DTOs
{
    public class EmpleadoTurnoItemDto
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Dni { get; set; }
        public int? OficinaId { get; set; }
        public string? OficinaNombre { get; set; }
        public string? OficinaSigla { get; set; }
        public int? CargoId { get; set; }
        public string? CargoNombre { get; set; }
        public int? TurnoId { get; set; }
        public string? TurnoNombre { get; set; }
        public string? HorarioEntradaSalida { get; set; }
        public bool HabilitadoParaMarcar { get; set; }
    }

    public record AsignarTurnoIndividualRequest(int EmpleadoId, int? TurnoId);

    public record AsignarTurnoMasivoRequest(List<int> EmpleadoIds, int? TurnoId);

    public record AsignarTurnoPorOficinaRequest(int OficinaId, int? TurnoId);
}
