import { Component, OnInit, OnDestroy, signal, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LiveMonitorService } from '../../../../../core/services/live-monitor.service';
import { Subscription } from 'rxjs';

// PrimeNG
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { MarcacionEnVivo } from '../../../../../core/models/marcacion.model';

@Component({
  selector: 'app-registro-en-vivo',
  standalone: true,
  imports: [CommonModule, FormsModule, TableModule, ToastModule],
  providers: [MessageService],
  templateUrl: './registro-en-vivo.component.html'
})
export class RegistroEnVivoComponent implements OnInit, OnDestroy {
  public monitorService = inject(LiveMonitorService);
  private messageService = inject(MessageService);
  private cdr = inject(ChangeDetectorRef);

  private sub?: Subscription;

  // Lista en memoria
  marcaciones = signal<MarcacionEnVivo[]>([]);
  cargando = signal<boolean>(false);

  // Contadores reactivos
  totalMarcas = signal<number>(0);
  totalPuntuales = signal<number>(0);
  totalTardanzas = signal<number>(0);
  totalExonerados = signal<number>(0);

  // ID de la última marca para animación luminosa
  idReciente = signal<number | null>(null);

  ngOnInit(): void {
    this.cargarDatosIniciales();
    this.iniciarSocket();
  }

  ngOnDestroy(): void {
    if (this.sub) this.sub.unsubscribe();
    this.monitorService.detenerConexion();
  }

  cargarDatosIniciales(): void {
    this.cargando.set(true);
    this.monitorService.obtenerResumenInicial().subscribe({
      next: (res) => {
        this.marcaciones.set(res.ultimasMarcaciones);
        this.totalMarcas.set(res.totalMarcas);
        this.totalPuntuales.set(res.totalPuntuales);
        this.totalTardanzas.set(res.totalTardanzas);
        this.totalExonerados.set(res.totalExonerados);
        this.cargando.set(false);
        this.cdr.markForCheck();
      },
      error: () => {
        this.cargando.set(false);
        this.cdr.markForCheck();
      }
    });
  }

  iniciarSocket(): void {
    this.monitorService.iniciarConexion();

    this.sub = this.monitorService.nuevaMarca$.subscribe((marca) => {
      // 1. Insertar la nueva marcación al principio de la lista
      this.marcaciones.update(lista => [marca, ...lista.slice(0, 99)]);
      
      // 2. Incrementar contadores
      this.totalMarcas.update(c => c + 1);
      if (marca.estadoPuntualidad === 'PUNTUAL' || marca.estadoPuntualidad === 'TOLERANCIA') {
        this.totalPuntuales.update(c => c + 1);
      } else if (marca.estadoPuntualidad === 'TARDANZA') {
        this.totalTardanzas.update(c => c + 1);
      } else if (marca.estadoPuntualidad === 'EXONERADO') {
        this.totalExonerados.update(c => c + 1);
      }

      // 3. Destello visual y sonido sutil opcional
      this.idReciente.set(marca.id);
      setTimeout(() => {
        this.idReciente.set(null);
        this.cdr.markForCheck();
      }, 3000);

      // Notificación flotante discreta
      this.messageService.add({
        severity: marca.estadoPuntualidad === 'TARDANZA' ? 'warn' : 'info',
        summary: `${marca.tipoMarcacion}: ${marca.nombreCompleto.split(' ')[0]}`,
        detail: `${marca.hora} - ${marca.oficinaSigla || 'Sede'} (${marca.estadoPuntualidad})`,
        life: 2500
      });

      this.cdr.markForCheck();
    });
  }
}