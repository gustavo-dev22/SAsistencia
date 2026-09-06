using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Domain.Entities
{
    public class Turno
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public int ToleranciaMinutos { get; set; }
        public bool Activo { get; set; } = true;
    }
}
