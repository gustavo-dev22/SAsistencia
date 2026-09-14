import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FeriadoItem, GuardarFeriadoPayload } from '../models/feriado.model';

@Injectable({
  providedIn: 'root'
})
export class CalendarioLaboralService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7051/api/CalendarioLaboral';

  listarPorAnio(anio: number): Observable<FeriadoItem[]> {
    return this.http.get<FeriadoItem[]>(`${this.apiUrl}/${anio}`);
  }

  guardar(payload: GuardarFeriadoPayload): Observable<{ mensaje: string }> {
    return this.http.post<{ mensaje: string }>(`${this.apiUrl}/guardar`, payload);
  }

  eliminar(id: number): Observable<{ mensaje: string }> {
    return this.http.delete<{ mensaje: string }>(`${this.apiUrl}/${id}`);
  }
}