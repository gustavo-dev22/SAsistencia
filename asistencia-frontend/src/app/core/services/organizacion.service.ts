import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CargoItem, OficinaItem } from '../models/organizacion.model';

@Injectable({
  providedIn: 'root'
})
export class OrganizacionService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7051/api/Organizacion';

  // Oficinas
  listarOficinas(): Observable<OficinaItem[]> {
    return this.http.get<OficinaItem[]>(`${this.apiUrl}/oficinas`);
  }

  sincronizarOficinas(): Observable<{ mensaje: string }> {
    return this.http.post<{ mensaje: string }>(`${this.apiUrl}/oficinas/sincronizar`, {});
  }

  // Cargos
  listarCargos(): Observable<CargoItem[]> {
    return this.http.get<CargoItem[]>(`${this.apiUrl}/cargos`);
  }

  crearCargo(payload: { nombre: string; descripcion: string | null; exoneradoMarcacion: boolean }): Observable<any> {
    return this.http.post(`${this.apiUrl}/cargos`, payload);
  }

  actualizarCargo(payload: { id: number; nombre: string; descripcion: string | null; exoneradoMarcacion: boolean; activo: boolean }): Observable<any> {
    return this.http.put(`${this.apiUrl}/cargos`, payload);
  }
}