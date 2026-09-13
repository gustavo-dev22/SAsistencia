import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AsignacionTurnosService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7051/api/AsignacionTurnos'; // Ajusta tu puerto

  asignarMasivo(empleadoIds: number[], turnoId: number | null): Observable<{ mensaje: string }> {
    return this.http.post<{ mensaje: string }>(`${this.apiUrl}/masiva`, {
      empleadoIds,
      turnoId
    });
  }

  asignarPorOficina(oficinaId: number, turnoId: number | null): Observable<{ mensaje: string }> {
    return this.http.post<{ mensaje: string }>(`${this.apiUrl}/por-oficina`, {
      oficinaId,
      turnoId
    });
  }
}