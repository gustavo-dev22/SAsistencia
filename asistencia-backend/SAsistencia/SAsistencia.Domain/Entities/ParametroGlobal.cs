using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Domain.Entities
{
    public class ParametroGlobal
    {
        public int Id { get; set; }
        public string Clave { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
        public string TipoDato { get; set; } = "STRING"; // INT, STRING, BOOL, DECIMAL, TIME
        public string Categoria { get; set; } = "GENERAL"; // QUIOSCO, POLITICAS, SEGURIDAD, SASI
        public string Etiqueta { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaModificacion { get; set; } = DateTime.UtcNow;
        public string? UsuarioModificador { get; set; }
    }
}
