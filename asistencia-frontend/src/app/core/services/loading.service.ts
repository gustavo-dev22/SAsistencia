import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  // Control de visibilidad y mensaje
  isLoading = signal<boolean>(false);
  mensaje = signal<string>('Procesando solicitud...');

  // Contador de peticiones concurrentes para no apagar el spinner si hay 2 llamadas a la vez
  private peticionesActivas = 0;

  mostrar(mensajePersonalizado = 'Cargando datos...'): void {
    this.peticionesActivas++;
    this.mensaje.set(mensajePersonalizado);
    this.isLoading.set(true);
  }

  ocultar(): void {
    this.peticionesActivas--;
    if (this.peticionesActivas <= 0) {
      this.peticionesActivas = 0;
      this.isLoading.set(false);
      this.mensaje.set('Procesando solicitud...');
    }
  }

  // Para forzar apagado en casos de error crítico
  forzarOcultar(): void {
    this.peticionesActivas = 0;
    this.isLoading.set(false);
  }
}