using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Domain.Entities
{
    public class Oficina
    {
        public int Id { get; set; } // IdOficina de SASI
        public string Nombre { get; set; } = string.Empty;
        public string Sigla { get; set; } = string.Empty;
        public int? OficinaPadreId { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaSincronizacion { get; set; } = DateTime.UtcNow;

        public Oficina? OficinaPadre { get; set; }
        public ICollection<Oficina> Suboficinas { get; set; } = new List<Oficina>();
        public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
    }
}
