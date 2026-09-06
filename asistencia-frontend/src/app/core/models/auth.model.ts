export interface MenuItem {
  idObjeto: number;
  titulo: string;
  nombre: string;
  url: string;
  icono: string;
  orden: number;
  submenus: MenuItem[];
}

export interface UsuarioInfo {
  id: string;
  nombreCompleto: string;
  userName: string;
  email: string;
  oficina: string;
}

export interface RolActivo {
  idRol: number;
  nombreRol: string;
  esPrincipal: boolean;
}

export interface LoginResponse {
  success: boolean;
  token: string;
  refreshToken: string;
  expiration: string;
  usuario: UsuarioInfo;
  rolActivo: RolActivo;
  menus: MenuItem[];
}