import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ItemAuditoria, PaginatedResult } from '../models/auditoria-marcas.model';

@Injectable({
  providedIn: 'root'
})
export class AuditoriaMarcasService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7051/api/AuditoriaMarcaciones';

  consultar(filtro: any): Observable<PaginatedResult<ItemAuditoria>> {
    return this.http.post<PaginatedResult<ItemAuditoria>>(`${this.apiUrl}/historial`, filtro);
  }

  regularizar(payload: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/regularizar`, payload);
  }
}