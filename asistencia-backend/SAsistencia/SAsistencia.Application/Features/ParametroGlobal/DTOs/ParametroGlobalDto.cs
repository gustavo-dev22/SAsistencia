using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.ParametroGlobal.DTOs
{
    public class ParametroGlobalDto
    {
        public int Id { get; set; }
        public string Clave { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
        public string TipoDato { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Etiqueta { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string FechaModificacion { get; set; } = string.Empty;
        public string? UsuarioModificador { get; set; }
    }

    public record ActualizarParametroItemRequest(string Clave, string Valor);

    public record GuardarParametrosBatchRequest(List<ActualizarParametroItemRequest> Parametros);
}
