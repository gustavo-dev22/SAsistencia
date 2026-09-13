import { Component, OnInit, signal, computed, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmpleadosService } from '../../../../core/services/empleado.service';
import { TurnoService } from '../../../../core/services/turno.service';
import { OrganizacionService } from '../../../../core/services/organizacion.service';
import { AsignacionTurnosService } from '../../../../core/services/asignacion-turnos.service';
import { EmpleadoItem } from '../../../../core/models/empleado.model';

// PrimeNG
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { MessageService } from 'primeng/api';
import { TurnoItem } from '../../../../core/models/turno.model';
import { OficinaItem } from '../../../../core/models/organizacion.model';

@Component({
  selector: 'app-asignacion-turnos',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    DialogModule,
    ToastModule,
    TooltipModule
  ],
  providers: [MessageService],
  templateUrl: './asignacion-turnos.component.html'
})
export class AsignacionTurnosComponent implements OnInit {
  private empleadosService = inject(EmpleadosService);
  private turnoService = inject(TurnoService);
  private orgService = inject(OrganizacionService);
  private asignacionService = inject(AsignacionTurnosService);
  private messageService = inject(MessageService);
  private cdr = inject(ChangeDetectorRef);

  // Estados
  empleados = signal<EmpleadoItem[]>([]);
  turnos = signal<TurnoItem[]>([]);
  oficinas = signal<OficinaItem[]>([]);
  cargando = signal<boolean>(false);

  // Selección en tabla
  empleadosSeleccionados: EmpleadoItem[] = [];

  // Filtros
  filtroTexto = signal<string>('');
  filtroOficina = signal<number | null>(null);
  filtroTurno = signal<string>('TODOS'); // 'TODOS' | 'CON_TURNO' | 'SIN_TURNO'

  // Modales
  dialogAsignacionMasiva = signal<boolean>(false);
  dialogAsignacionOficina = signal<boolean>(false);
  turnoSeleccionadoParaAsignar: number | null = null;
  oficinaSeleccionadaParaAsignar: number | null = null;

  // Filtrado reactivo en memoria
  empleadosFiltrados = computed(() => {
    const q = this.filtroTexto().trim().toLowerCase();
    const ofiId = this.filtroOficina();
    const estadoTurno = this.filtroTurno();

    return this.empleados().filter(emp => {
      // 1. Filtro texto
      const matchTexto = !q || 
        emp.nombreCompleto.toLowerCase().includes(q) ||
        (emp.dni && emp.dni.includes(q)) ||
        (emp.oficinaNombre && emp.oficinaNombre.toLowerCase().includes(q));

      // 2. Filtro oficina
      const matchOficina = ofiId === null || emp.oficinaId === ofiId;

      // 3. Filtro estado de turno
      let matchTurno = true;
      if (estadoTurno === 'CON_TURNO') matchTurno = emp.turnoId !== null;
      if (estadoTurno === 'SIN_TURNO') matchTurno = emp.turnoId === null;

      return matchTexto && matchOficina && matchTurno;
    });
  });

  // Métricas
  totalEmpleados = computed(() => this.empleados().length);
  totalConTurno = computed(() => this.empleados().filter(e => e.turnoId !== null).length);
  totalSinTurno = computed(() => this.empleados().filter(e => e.turnoId === null).length);

  ngOnInit(): void {
    this.cargarDatos();
  }

  cargarDatos(): void {
    this.cargando.set(true);

    this.empleadosService.listar().subscribe({
      next: (emps) => {
        this.empleados.set(emps);
        this.cargando.set(false);
        this.cdr.markForCheck();
      },
      error: () => {
        this.cargando.set(false);
        this.cdr.markForCheck();
      }
    });

    this.turnoService.listar().subscribe({
      next: (ts) => this.turnos.set(ts.filter(t => t.activo))
    });

    this.orgService.listarOficinas().subscribe({
      next: (ofis) => this.oficinas.set(ofis.filter(o => o.activo))
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

  // --- ASIGNACIÓN MASIVA POR CHECKBOX ---
  abrirModalAsignacionMasiva(): void {
    if (this.empleadosSeleccionados.length === 0) {
      this.messageService.add({ 
        severity: 'warn', 
        summary: 'Sin selección', 
        detail: 'Seleccione al menos un colaborador de la lista.' 
      });
      return;
    }
    this.turnoSeleccionadoParaAsignar = null;
    this.dialogAsignacionMasiva.set(true);
    this.cdr.markForCheck();
  }

  ejecutarAsignacionMasiva(): void {
    const ids = this.empleadosSeleccionados.map(e => e.id);

    this.asignacionService.asignarMasivo(ids, this.turnoSeleccionadoParaAsignar).subscribe({
      next: (res) => {
        this.messageService.add({ severity: 'success', summary: 'Éxito', detail: res.mensaje });
        this.dialogAsignacionMasiva.set(false);
        this.empleadosSeleccionados = [];
        this.cargarDatos();
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Fallo al procesar la asignación.' });
      }
    });
  }

  // --- ASIGNACIÓN POR ÁREA / OFICINA COMPLETA ---
  abrirModalAsignacionOficina(): void {
    this.oficinaSeleccionadaParaAsignar = null;
    this.turnoSeleccionadoParaAsignar = null;
    this.dialogAsignacionOficina.set(true);
    this.cdr.markForCheck();
  }

  ejecutarAsignacionOficina(): void {
    if (!this.oficinaSeleccionadaParaAsignar) {
      this.messageService.add({ severity: 'warn', summary: 'Atención', detail: 'Seleccione una oficina.' });
      return;
    }

    this.asignacionService.asignarPorOficina(
      this.oficinaSeleccionadaParaAsignar, 
      this.turnoSeleccionadoParaAsignar
    ).subscribe({
      next: (res) => {
        this.messageService.add({ severity: 'success', summary: 'Éxito', detail: res.mensaje });
        this.dialogAsignacionOficina.set(false);
        this.cargarDatos();
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Fallo al asignar el turno al área.' });
      }
    });
  }
}