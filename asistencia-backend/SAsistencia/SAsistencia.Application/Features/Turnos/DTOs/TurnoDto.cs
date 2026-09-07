using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Turnos.DTOs
{
    public class TurnoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string HoraEntrada { get; set; } = string.Empty; // Formato "HH:mm"
        public string HoraSalida { get; set; } = string.Empty;  // Formato "HH:mm"
        public int ToleranciaEntradaMinutos { get; set; }
        public int LimiteTardanzaMinutos { get; set; }
        public int MinutosRefrigerio { get; set; }
        public bool EsRotativo { get; set; }
        public bool Activo { get; set; }
        public int TotalEmpleadosAsignados { get; set; }
    }

    public record CrearTurnoRequest(
        string Nombre,
        string? Descripcion,
        string HoraEntrada, // "08:30"
        string HoraSalida,  // "17:30"
        int ToleranciaEntradaMinutos,
        int LimiteTardanzaMinutos,
        int MinutosRefrigerio,
        bool EsRotativo
    );

    public record ActualizarTurnoRequest(
        int Id,
        string Nombre,
        string? Descripcion,
        string HoraEntrada,
        string HoraSalida,
        int ToleranciaEntradaMinutos,
        int LimiteTardanzaMinutos,
        int MinutosRefrigerio,
        bool EsRotativo,
        bool Activo
    );
}
