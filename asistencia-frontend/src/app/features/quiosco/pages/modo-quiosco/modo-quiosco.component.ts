import { Component, OnInit, OnDestroy, signal, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { QuioscoService } from '../../../../core/services/quiosco.service';
import { ZXingScannerModule } from '@zxing/ngx-scanner';
import { BarcodeFormat } from '@zxing/library';
import { ResultadoMarcacion } from '../../../../core/models/marcacion.model';

@Component({
  selector: 'app-modo-quiosco',
  standalone: true,
  imports: [CommonModule, ZXingScannerModule],
  templateUrl: './modo-quiosco.component.html'
})
export class ModoQuioscoComponent implements OnInit, OnDestroy {
  private quioscoService = inject(QuioscoService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  // Reloj
  horaActual = signal<string>('');
  fechaActual = signal<string>('');
  private relojTimer: any;

  // Entrada por DNI
  dniInput = signal<string>('');

  // Configuración Escáner QR
  formatosPermitidos = [BarcodeFormat.QR_CODE];
  tienePermisoCamara = signal<boolean>(true);
  modoActivo = signal<'DNI' | 'QR'>('DNI');
  tryHarder = true;

  // Control de escaneo (Para no apagar el hardware de la cámara)
  ignorarLecturas = false;

  // Feedback y Resultados
  procesando = signal<boolean>(false);
  resultado = signal<ResultadoMarcacion | null>(null);
  mostrarResultado = signal<boolean>(false);
  private resetTimer: any;

  ngOnInit(): void {
    this.iniciarReloj();
  }

  ngOnDestroy(): void {
    if (this.relojTimer) clearInterval(this.relojTimer);
    if (this.resetTimer) clearTimeout(this.resetTimer);
  }

  onScanError(error: any): void {
    // Ignorar deliberadamente NotFoundException ya que ocurre en cada frame vacío
    if (error?.name === 'NotFoundException' || error?.message?.includes('NotFoundException')) {
        return;
    }
    // Solo loguear si es un error crítico distinto
    console.error('Error del lector QR:', error);
  }

  iniciarReloj(): void {
    const actualizar = () => {
      const ahora = new Date();
      this.horaActual.set(ahora.toLocaleTimeString('es-PE', { hour: '2-digit', minute: '2-digit', second: '2-digit' }));
      this.fechaActual.set(ahora.toLocaleDateString('es-PE', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' }));
    };
    actualizar();
    this.relojTimer = setInterval(actualizar, 1000);
  }

  // Teclado Numérico
  presionarTecla(tecla: string): void {
    if (this.procesando()) return;
    if (this.dniInput().length < 12) {
      this.dniInput.update(v => v + tecla);
    }
  }

  borrarTecla(): void {
    if (this.procesando()) return;
    this.dniInput.update(v => v.slice(0, -1));
  }

  limpiarDni(): void {
    this.dniInput.set('');
  }

  marcarPorDni(): void {
    const dni = this.dniInput().trim();
    if (!dni || this.procesando() || this.ignorarLecturas) return;
    this.ejecutarProceso(dni, 'DNI');
  }

  // Callback de éxito de la cámara QR
  alEscanearQr(codigo: string): void {
    // Si estamos mostrando el resultado o procesando, IGNORAMOS la lectura sin apagar la cámara
    if (!codigo || this.procesando() || this.ignorarLecturas) return;
    this.ejecutarProceso(codigo, 'QR');
  }

  // Manejador del permiso de cámara
  onHasPermission(hasPermission: boolean): void {
    this.tienePermisoCamara.set(hasPermission);
    this.cdr.markForCheck();
  }

  private ejecutarProceso(identificador: string, metodo: 'QR' | 'DNI'): void {
    this.procesando.set(true);
    this.ignorarLecturas = true; // Bloquear lecturas repetidas de la cámara

    this.quioscoService.registrarMarca(identificador, metodo).subscribe({
      next: (res) => {
        this.resultado.set(res);
        this.mostrarResultado.set(true);
        this.procesando.set(false);
        this.limpiarDni();
        this.reproducirFeedbackVoz(res);
        this.cdr.markForCheck();

        // Auto-reset después de 3.5 segundos: vuelve a habilitar la lectura
        this.resetTimer = setTimeout(() => {
          this.mostrarResultado.set(false);
          this.resultado.set(null);
          this.ignorarLecturas = false; // Vuelve a permitir lecturas limpiamente
          this.cdr.markForCheck();
        }, 3500);
      },
      error: () => {
        this.resultado.set({
          exito: false,
          mensaje: 'Error de comunicación con el servidor institucional.',
          minutosTardanza: 0
        });
        this.mostrarResultado.set(true);
        this.procesando.set(false);
        this.limpiarDni();
        this.cdr.markForCheck();

        this.resetTimer = setTimeout(() => {
          this.mostrarResultado.set(false);
          this.resultado.set(null);
          this.ignorarLecturas = false;
          this.cdr.markForCheck();
        }, 3500);
      }
    });
  }

  private obtenerSaludo(): string {
    const hora = new Date().getHours();
    if (hora >= 5 && hora < 12) return 'Buenos días';
    if (hora >= 12 && hora < 19) return 'Buenas tardes';
    return 'Buenas noches';
    }

  private reproducirFeedbackVoz(res: ResultadoMarcacion): void {
    if ('speechSynthesis' in window) {
        try {
        window.speechSynthesis.cancel(); // Detener cualquier audio en cola

        const primerNombre = res.nombreEmpleado?.trim().split(' ')[0] || '';
        const saludo = this.obtenerSaludo();

        let mensajeVoz = '';
        if (res.exito) {
            mensajeVoz = `${res.tipoMarcacion} registrada. ${saludo}, ${primerNombre}`;
        } else {
            mensajeVoz = res.mensaje || 'Identificación no autorizada';
        }

        const utterance = new SpeechSynthesisUtterance(mensajeVoz);
        utterance.lang = 'es-PE';
        utterance.rate = 1.05; // Ritmo ágil para quiosco
        window.speechSynthesis.speak(utterance);
        } catch {
        // Manejo silencioso en navegadores sin soporte de audio
        }
    }
  }

  volverAlPanel(): void {
    this.router.navigate(['/admin/dashboard']);
  }
}