import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';
import { MarcacionEnVivo, ResumenHoy } from '../models/marcacion.model';

@Injectable({
  providedIn: 'root'
})
export class LiveMonitorService {
  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:7051'; // Ajusta el puerto
  private hubConnection?: signalR.HubConnection;

  // Estados reactivos
  conectado = signal<boolean>(false);
  nuevaMarca$ = new Subject<MarcacionEnVivo>();

  obtenerResumenInicial(): Observable<ResumenHoy> {
    return this.http.get<ResumenHoy>(`${this.baseUrl}/api/Marcaciones/hoy`);
  }

  iniciarConexion(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${this.baseUrl}/hubs/marcaciones`, {
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        this.conectado.set(true);
        console.log('[SignalR] Conectado al Hub de Marcaciones en Vivo');
      })
      .catch(err => {
        this.conectado.set(false);
        console.error('[SignalR] Error al conectar:', err);
      });

    this.hubConnection.onreconnecting(() => this.conectado.set(false));
    this.hubConnection.onreconnected(() => this.conectado.set(true));
    this.hubConnection.onclose(() => this.conectado.set(false));

    // Escuchar el evento emitido por el backend
    this.hubConnection.on('RecibirMarcacionEnVivo', (data: MarcacionEnVivo) => {
      this.nuevaMarca$.next(data);
    });
  }

  detenerConexion(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
      this.conectado.set(false);
    }
  }
}