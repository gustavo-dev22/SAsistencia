import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { JustificacionItem, PaginatedResult, TipoJustificacion } from '../models/justificacion.model';

@Injectable({
  providedIn: 'root'
})
export class JustificacionesService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7051/api/Justificaciones';

  listarTipos(): Observable<TipoJustificacion[]> {
    return this.http.get<TipoJustificacion[]>(`${this.apiUrl}/tipos`);
  }

  consultarHistorial(filtro: any): Observable<PaginatedResult<JustificacionItem>> {
    return this.http.post<PaginatedResult<JustificacionItem>>(`${this.apiUrl}/historial`, filtro);
  }

  registrar(formData: FormData): Observable<any> {
    return this.http.post(`${this.apiUrl}/registrar`, formData);
  }

  resolver(id: number, estado: 'APROBADA' | 'RECHAZADA', observaciones?: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/resolver`, { id, estado, observaciones });
  }

  descargarArchivoUrl(id: number): string {
    return `${this.apiUrl}/descargar/${id}`;
  }
}