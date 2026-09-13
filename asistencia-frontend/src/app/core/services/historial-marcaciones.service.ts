import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FiltroHistorial, ItemHistorial, PaginatedResult } from '../models/marcacion.model';

@Injectable({
  providedIn: 'root'
})
export class HistorialMarcacionesService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7051/api/Marcaciones/historial';

  consultarHistorial(filtro: FiltroHistorial): Observable<PaginatedResult<ItemHistorial>> {
    return this.http.post<PaginatedResult<ItemHistorial>>(this.apiUrl, filtro);
  }
}