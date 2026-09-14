import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ParametroGlobal } from '../models/parametro.model';

@Injectable({
  providedIn: 'root'
})
export class ParametrosService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7051/api/Parametros';

  listar(): Observable<ParametroGlobal[]> {
    return this.http.get<ParametroGlobal[]>(this.apiUrl);
  }

  guardarBatch(parametros: { clave: string; valor: string }[]): Observable<{ mensaje: string }> {
    return this.http.post<{ mensaje: string }>(`${this.apiUrl}/guardar`, { parametros });
  }
}