using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Domain.Entities
{
    public class TipoJustificacion
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool RequiereDocumento { get; set; } = true;
        public bool ConGoceHaber { get; set; } = true;
        public bool Activo { get; set; } = true;

        public ICollection<Justificacion> Justificaciones { get; set; } = new List<Justificacion>();
    }
}
