using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Common.Services
{
    public interface ICurrentUserService
    {
        string? UsuarioId { get; }
        string NombreCompleto { get; }
        string? Email { get; }
        bool EstaAutenticado { get; }
        string ObtenerIpCliente();
    }
}
