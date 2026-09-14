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

  // Catálogo de días (1: Lun a 0: Dom)
  diasDisponibles = [
    { id: '1', nombre: 'Lun' },
    { id: '2', nombre: 'Mar' },
    { id: '3', nombre: 'Mié' },
    { id: '4', nombre: 'Jue' },
    { id: '5', nombre: 'Vie' },
    { id: '6', nombre: 'Sáb' },
    { id: '0', nombre: 'Dom' }
  ];

  // Días seleccionados en el modal (por defecto Lunes a Viernes)
  diasSeleccionados: string[] = ['1', '2', '3', '4', '5'];

  // Formulario de Turno
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
    activo: true,
    diasSemana: '1,2,3,4,5'
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
    this.diasSeleccionados = ['1', '2', '3', '4', '5'];
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
      activo: true,
      diasSemana: '1,2,3,4,5'
    };
    this.dialogTurno.set(true);
    this.cdr.markForCheck();
  }

  abrirEditar(t: TurnoItem): void {
    this.esNuevo.set(false);
    
    // Parsear la cadena de días ("1,2,3,4,5") al arreglo de checkboxes/botones
    const diasCadena = t.diasSemana || '1,2,3,4,5';
    this.diasSeleccionados = diasCadena.split(',').map(d => d.trim()).filter(d => d.length > 0);

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
      activo: t.activo,
      diasSemana: diasCadena
    };
    this.dialogTurno.set(true);
    this.cdr.markForCheck();
  }

  toggleDia(diaId: string): void {
    if (this.diasSeleccionados.includes(diaId)) {
      if (this.diasSeleccionados.length > 1) {
        this.diasSeleccionados = this.diasSeleccionados.filter(d => d !== diaId);
      }
    } else {
      this.diasSeleccionados.push(diaId);
    }
    // Mantener sincronizado el campo diasSemana ordenado
    this.turnoForm.diasSemana = [...this.diasSeleccionados].sort().join(',');
  }

  esDiaActivo(diaId: string): boolean {
    return this.diasSeleccionados.includes(diaId);
  }

  // Método auxiliar para pintar las etiquetas de días en la tabla principal
  obtenerTextoDias(diasCadena?: string): string {
    if (!diasCadena) return 'Lun - Vie';
    if (diasCadena === '1,2,3,4,5') return 'Lun - Vie';
    if (diasCadena === '1,2,3,4,5,6') return 'Lun - Sáb';
    if (diasCadena === '0,1,2,3,4,5,6') return 'Toda la semana';

    const mapa: Record<string, string> = {
      '1': 'Lun', '2': 'Mar', '3': 'Mié', '4': 'Jue', '5': 'Vie', '6': 'Sáb', '0': 'Dom'
    };

    return diasCadena
      .split(',')
      .map(d => mapa[d.trim()] || d)
      .join(', ');
  }

  guardar(): void {
    if (!this.turnoForm.nombre.trim()) {
      this.messageService.add({ severity: 'warn', summary: 'Atención', detail: 'El nombre del turno es requerido.' });
      return;
    }

    if (this.diasSeleccionados.length === 0) {
      this.messageService.add({ severity: 'warn', summary: 'Atención', detail: 'Debe seleccionar al menos un día laborable.' });
      return;
    }

    // Asegurar valor de diasSemana
    this.turnoForm.diasSemana = [...this.diasSeleccionados].sort().join(',');

    if (this.esNuevo()) {
      this.turnoService.crear(this.turnoForm).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Turno registrado correctamente.' });
          this.dialogTurno.set(false);
          this.cargarTurnos();
        },
        error: () => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Fallo al registrar el turno.' });
        }
      });
    } else {
      this.turnoService.actualizar(this.turnoForm).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Turno actualizado correctamente.' });
          this.dialogTurno.set(false);
          this.cargarTurnos();
        },
        error: () => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Fallo al actualizar el turno.' });
        }
      });
    }
  }
}