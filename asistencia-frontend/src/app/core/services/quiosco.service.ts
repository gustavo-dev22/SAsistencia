import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ResultadoMarcacion } from '../models/marcacion.model';

@Injectable({
  providedIn: 'root'
})
export class QuioscoService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7051/api/Marcaciones/registrar'; // Ajusta el puerto a tu API

  registrarMarca(identificador: string, metodo: 'QR' | 'DNI'): Observable<ResultadoMarcacion> {
    return this.http.post<ResultadoMarcacion>(this.apiUrl, {
      identificador,
      metodo
    });
  }
}