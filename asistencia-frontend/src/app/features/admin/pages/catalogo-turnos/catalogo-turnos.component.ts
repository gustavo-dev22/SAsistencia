import { Component, OnInit, signal, computed, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TurnoService } from '../../../../core/services/turno.service';

// PrimeNG
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { SharedModule, MessageService } from 'primeng/api';
import { TurnoItem } from '../../../../core/models/turno.model';

@Component({
  selector: 'app-catalogo-turnos',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    DialogModule,
    ToastModule,
    TooltipModule,
    SharedModule
  ],
  providers: [MessageService],
  templateUrl: './catalogo-turnos.component.html'
})
export class CatalogoTurnosComponent implements OnInit {
  private turnoService = inject(TurnoService);
  private messageService = inject(MessageService);
  private cdr = inject(ChangeDetectorRef);

  turnos = signal<TurnoItem[]>([]);
  cargando = signal<boolean>(false);
  filtroTexto = signal<string>('');

  // Modales
  dialogTurno = signal<boolean>(false);
  esNuevo = signal<boolean>(true);

  // Formulario
  turnoForm = {
    id: 0,
    nombre: '',
    descripcion: '',
    horaEntrada: '08:00',
    horaSalida: '17:00',
    toleranciaEntradaMinutos: 10,
    limiteTardanzaMinutos: 30,
    minutosRefrigerio: 60,
    esRotativo: false,
    activo: true
  };

  // Turnos Filtrados reactivamente
  turnosFiltrados = computed(() => {
    const q = this.filtroTexto().trim().toLowerCase();
    if (!q) return this.turnos();
    return this.turnos().filter(t => 
      t.nombre.toLowerCase().includes(q) || 
      (t.descripcion && t.descripcion.toLowerCase().includes(q))
    );
  });

  // Métricas reactivas
  totalTurnos = computed(() => this.turnos().length);
  totalActivos = computed(() => this.turnos().filter(t => t.activo).length);

  ngOnInit(): void {
    this.cargarTurnos();
  }

  cargarTurnos(): void {
    this.cargando.set(true);
    this.turnoService.listar().subscribe({
      next: (data) => {
        this.turnos.set([...data]);
        this.cargando.set(false);
        this.cdr.markForCheck();
      },
      error: () => {
        this.cargando.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar los turnos.' });
        this.cdr.markForCheck();
      }
    });
  }

  onBuscar(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.filtroTexto.set(input.value);
  }

  limpiarBusqueda(): void {
    this.filtroTexto.set('');
    this.cdr.markForCheck();
  }

  abrirCrear(): void {
    this.esNuevo.set(true);
    this.turnoForm = {
      id: 0,
      nombre: '',
      descripcion: '',
      horaEntrada: '08:30',
      horaSalida: '17:30',
      toleranciaEntradaMinutos: 15,
      limiteTardanzaMinutos: 30,
      minutosRefrigerio: 60,
      esRotativo: false,
      activo: true
    };
    this.dialogTurno.set(true);
    this.cdr.markForCheck();
  }

  abrirEditar(t: TurnoItem): void {
    this.esNuevo.set(false);
    this.turnoForm = {
      id: t.id,
      nombre: t.nombre,
      descripcion: t.descripcion || '',
      horaEntrada: t.horaEntrada,
      horaSalida: t.horaSalida,
      toleranciaEntradaMinutos: t.toleranciaEntradaMinutos,
      limiteTardanzaMinutos: t.limiteTardanzaMinutos,
      minutosRefrigerio: t.minutosRefrigerio,
      esRotativo: t.esRotativo,
      activo: t.activo
    };
    this.dialogTurno.set(true);
    this.cdr.markForCheck();
  }

  guardar(): void {
    if (!this.turnoForm.nombre.trim()) {
      this.messageService.add({ severity: 'warn', summary: 'Atención', detail: 'El nombre del turno es requerido.' });
      return;
    }

    if (this.esNuevo()) {
      this.turnoService.crear(this.turnoForm).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Turno registrado correctamente.' });
          this.dialogTurno.set(false);
          this.cargarTurnos();
        }
      });
    } else {
      this.turnoService.actualizar(this.turnoForm).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Turno actualizado correctamente.' });
          this.dialogTurno.set(false);
          this.cargarTurnos();
        }
      });
    }
  }
}