import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EmpleadoItem } from '../models/empleado.model';

@Injectable({
  providedIn: 'root'
})
export class EmpleadosService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7051/api/Empleados';

  listar(): Observable<EmpleadoItem[]> {
    return this.http.get<EmpleadoItem[]>(this.apiUrl);
  }

  sincronizar(): Observable<{ mensaje: string }> {
    return this.http.post<{ mensaje: string }>(`${this.apiUrl}/sincronizar`, {});
  }

  actualizar(dto: { id: number; dni: string; turnoId: number | null; cargoId: number | null; habilitadoParaMarcar: boolean }): Observable<any> {
    return this.http.put(`${this.apiUrl}`, dto);
  }

  regenerarQr(id: number): Observable<{ codigoQr: string; mensaje: string }> {
    return this.http.post<{ codigoQr: string; mensaje: string }>(`${this.apiUrl}/${id}/regenerar-qr`, {});
  }
}