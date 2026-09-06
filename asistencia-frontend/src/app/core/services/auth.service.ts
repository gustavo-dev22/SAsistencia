import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { LoginResponse, MenuItem, UsuarioInfo, RolActivo } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_URL = 'https://localhost:7051/api/Auth';

  // Signals reactivos cargando desde cualquiera de los dos storages
  private _currentUser = signal<UsuarioInfo | null>(this.getItem<UsuarioInfo>('user'));
  private _userMenus = signal<MenuItem[]>(this.getItem<MenuItem[]>('menus') || []);
  private _userRole = signal<RolActivo | null>(this.getItem<RolActivo>('role'));
  private _token = signal<string | null>(this.getRawItem('token'));

  public currentUser = computed(() => this._currentUser());
  public userMenus = computed(() => this._userMenus());
  public userRole = computed(() => this._userRole());
  public isAuthenticated = computed(() => !!this._token());

  constructor(private http: HttpClient, private router: Router) {}

  login(usuario: string, password: string, recordar: boolean = false): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.API_URL}/login`, { usuario, password }).pipe(
      tap((res) => {
        if (res.success) {
          // Limpia ambos storages para no mezclar sesiones previas
          this.limpiarSesion();

          // 1. Elegir el almacenamiento según el checkbox
          const storage = recordar ? localStorage : sessionStorage;

          storage.setItem('token', res.token);
          storage.setItem('refreshToken', res.refreshToken);
          storage.setItem('user', JSON.stringify(res.usuario));
          storage.setItem('menus', JSON.stringify(res.menus));
          storage.setItem('role', JSON.stringify(res.rolActivo));

          // 2. Si marcó recordar, persistir el nombre de usuario para futuros logins
          if (recordar) {
            localStorage.setItem('recordar_usuario', usuario);
          } else {
            localStorage.removeItem('recordar_usuario');
          }

          // 3. Actualizar Signals
          this._token.set(res.token);
          this._currentUser.set(res.usuario);
          this._userMenus.set(res.menus);
          this._userRole.set(res.rolActivo);
        }
      })
    );
  }

  logout(): void {
    // Al cerrar sesión se purga la sesión activa, pero se conserva el usuario recordado si existía
    const usuarioRecordado = localStorage.getItem('recordar_usuario');
    this.limpiarSesion();

    if (usuarioRecordado) {
      localStorage.setItem('recordar_usuario', usuarioRecordado);
    }

    this._token.set(null);
    this._currentUser.set(null);
    this._userMenus.set([]);
    this._userRole.set(null);

    this.router.navigate(['/login'], { replaceUrl: true });
  }

  // Devuelve el usuario guardado si se seleccionó "recordar" previamente
  getUsuarioRecordado(): string | null {
    return localStorage.getItem('recordar_usuario');
  }

  private limpiarSesion(): void {
    const keys = ['token', 'refreshToken', 'user', 'menus', 'role'];
    keys.forEach(k => {
      localStorage.removeItem(k);
      sessionStorage.removeItem(k);
    });
  }

  // Busca primero en localStorage, luego en sessionStorage
  private getRawItem(key: string): string | null {
    return localStorage.getItem(key) ?? sessionStorage.getItem(key);
  }

  private getItem<T>(key: string): T | null {
    const raw = this.getRawItem(key);
    return raw ? JSON.parse(raw) : null;
  }
}