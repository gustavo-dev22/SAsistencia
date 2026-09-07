import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TurnoItem } from '../models/turno.model';

@Injectable({
  providedIn: 'root'
})
export class TurnoService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7051/api/Turnos'; // Ajusta a tu puerto

  listar(): Observable<TurnoItem[]> {
    return this.http.get<TurnoItem[]>(this.apiUrl);
  }

  crear(payload: any): Observable<any> {
    return this.http.post(this.apiUrl, payload);
  }

  actualizar(payload: any): Observable<any> {
    return this.http.put(this.apiUrl, payload);
  }
}