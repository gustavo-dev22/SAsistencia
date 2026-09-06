using System;
using System.Collections.Generic;
using System.Text;

namespace SAsistencia.Application.Features.Auth.DTOs
{
    // Request que envía el Frontend
    public record LoginRequest(string Usuario, string Password);

    // Respuesta consolidada que devolveremos a Angular
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string Expiration { get; set; } = string.Empty;
        public UsuarioInfoDto Usuario { get; set; } = new();
        public RolAsistenciaDto RolActivo { get; set; } = new();
        public List<MenuItemDto> Menus { get; set; } = new();
    }

    public class UsuarioInfoDto
    {
        public string Id { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Oficina { get; set; } = string.Empty;
    }

    public class RolAsistenciaDto
    {
        public int IdRol { get; set; }
        public string NombreRol { get; set; } = string.Empty;
        public bool EsPrincipal { get; set; }
    }

    public class MenuItemDto
    {
        public int IdObjeto { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;
        public int Orden { get; set; }
        public List<MenuItemDto> Submenus { get; set; } = new();
    }

    // Modelos para deserializar la respuesta completa de SASI
    public class SasiLoginApiResponse
    {
        public bool Success { get; set; }
        public bool Bloqueado { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string Expiration { get; set; } = string.Empty;
        public SasiUsuario? Usuario { get; set; }
    }

    public class SasiUsuario
    {
        public string Id { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public SasiOficina? Oficina { get; set; }
        public List<SasiSistema> Sistemas { get; set; } = new();
    }

    public class SasiOficina
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class SasiSistema
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public List<SasiRol> Roles { get; set; } = new();
    }

    public class SasiRol
    {
        public int IdRol { get; set; }
        public string NombreRol { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public bool EsPrincipal { get; set; }
        public List<SasiObjeto> Objetos { get; set; } = new();
    }

    public class SasiObjeto
    {
        public int IdObjeto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty; // "Menu" o "Submenu"
        public string Url { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public int Orden { get; set; }
        public int? IdPadre { get; set; }
    }
}
