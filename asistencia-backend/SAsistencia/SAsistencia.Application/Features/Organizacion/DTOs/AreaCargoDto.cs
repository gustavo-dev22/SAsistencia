using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Organizacion.DTOs
{
    // SASI DTOs
    public class SasiOficinasResponse
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public List<SasiOficinaItemDto> Datos { get; set; } = new();
    }

    public class SasiOficinaItemDto
    {
        public int IdOficina { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Sigla { get; set; } = string.Empty;
        public int? OficinaPadreId { get; set; }
        public bool Activo { get; set; }
    }

    // DTOs para la UI
    public class OficinaListDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Sigla { get; set; } = string.Empty;
        public int? OficinaPadreId { get; set; }
        public string? NombreOficinaPadre { get; set; }
        public bool Activo { get; set; }
        public int TotalEmpleados { get; set; }
    }

    public class CargoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool ExoneradoMarcacion { get; set; }
        public bool Activo { get; set; }
        public int TotalEmpleados { get; set; }
    }

    public record CrearCargoRequest(string Nombre, string? Descripcion, bool ExoneradoMarcacion);
    public record ActualizarCargoRequest(int Id, string Nombre, string? Descripcion, bool ExoneradoMarcacion, bool Activo);
}
