using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Domain.Entities
{
    public class Turno
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public int ToleranciaEntradaMinutos { get; set; } = 15;
        public int LimiteTardanzaMinutos { get; set; } = 30;
        public int MinutosRefrigerio { get; set; } = 60;
        public bool EsRotativo { get; set; } = false;
        public string DiasSemana { get; set; } = "1,2,3,4,5";
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
    }
}
