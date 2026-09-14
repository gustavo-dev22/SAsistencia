import { Component, OnInit, signal, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CalendarioLaboralService } from '../../../../../core/services/calendario-laboral.service';

// PrimeNG
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { DialogModule } from 'primeng/dialog';
import { TooltipModule } from 'primeng/tooltip';
import { MessageService } from 'primeng/api';
import { FeriadoItem, GuardarFeriadoPayload } from '../../../../../core/models/feriado.model';

@Component({
  selector: 'app-calendario-laboral',
  standalone: true,
  imports: [CommonModule, FormsModule, TableModule, ToastModule, DialogModule, TooltipModule],
  providers: [MessageService],
  templateUrl: './calendario-laboral.component.html'
})
export class CalendarioLaboralComponent implements OnInit {
  private calendarioService = inject(CalendarioLaboralService);
  private messageService = inject(MessageService);
  private cdr = inject(ChangeDetectorRef);

  feriados = signal<FeriadoItem[]>([]);
  cargando = signal<boolean>(false);
  
  private anioActual = new Date().getFullYear();
  anioSeleccionado = signal<number>(this.anioActual);

  // Ventana móvil automática: [Año Anterior, Año Actual, Año Siguiente]
  aniosDisponibles: number[] = [
    this.anioActual - 1, 
    this.anioActual, 
    this.anioActual + 1
  ];

  // Modal
  dialogFeriado = signal<boolean>(false);
  esEdicion = signal<boolean>(false);
  
  formFeriado: GuardarFeriadoPayload = {
    id: null,
    fecha: new Date().toISOString().split('T')[0],
    nombre: '',
    tipo: 'FERIADO_NACIONAL',
    aplicaSectorPublico: true,
    esCompensable: false,
    normaLegal: '',
    fechaCompensacionLimite: null
  };

  ngOnInit(): void {
    this.cargarFeriados(this.anioSeleccionado());
  }

  cargarFeriados(anio: number): void {
    this.cargando.set(true);
    this.anioSeleccionado.set(anio);

    this.calendarioService.listarPorAnio(anio).subscribe({
      next: (data) => {
        this.feriados.set(data);
        this.cargando.set(false);
        this.cdr.markForCheck();
      },
      error: () => {
        this.cargando.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Fallo al cargar el calendario.' });
        this.cdr.markForCheck();
      }
    });
  }

  abrirCrear(): void {
    this.esEdicion.set(false);
    this.formFeriado = {
      id: null,
      fecha: `${this.anioSeleccionado()}-01-01`,
      nombre: '',
      tipo: 'FERIADO_NACIONAL',
      aplicaSectorPublico: true,
      esCompensable: false,
      normaLegal: '',
      fechaCompensacionLimite: null
    };
    this.dialogFeriado.set(true);
  }

  abrirEditar(item: FeriadoItem): void {
    this.esEdicion.set(true);
    this.formFeriado = {
      id: item.id,
      fecha: item.fecha,
      nombre: item.nombre,
      tipo: item.tipo,
      aplicaSectorPublico: item.aplicaSectorPublico,
      esCompensable: item.esCompensable,
      normaLegal: item.normaLegal || '',
      fechaCompensacionLimite: item.fechaCompensacionLimite || null
    };
    this.dialogFeriado.set(true);
  }

  guardar(): void {
    if (!this.formFeriado.nombre.trim() || !this.formFeriado.fecha) {
      this.messageService.add({ severity: 'warn', summary: 'Atención', detail: 'Complete la fecha y denominación del día festivo.' });
      return;
    }

    this.calendarioService.guardar(this.formFeriado).subscribe({
      next: (res) => {
        this.messageService.add({ severity: 'success', summary: 'Éxito', detail: res.mensaje });
        this.dialogFeriado.set(false);
        this.cargarFeriados(this.anioSeleccionado());
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: err?.error?.mensaje || 'No se pudo guardar.' });
      }
    });
  }

  eliminar(id: number): void {
    if (!confirm('¿Está seguro de eliminar esta fecha del calendario oficial?')) return;

    this.calendarioService.eliminar(id).subscribe({
      next: (res) => {
        this.messageService.add({ severity: 'info', summary: 'Eliminado', detail: res.mensaje });
        this.cargarFeriados(this.anioSeleccionado());
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo eliminar.' });
      }
    });
  }
}