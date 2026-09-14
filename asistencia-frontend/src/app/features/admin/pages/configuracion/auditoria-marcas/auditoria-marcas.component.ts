import { Component, OnInit, signal, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuditoriaMarcasService } from '../../../../../core/services/auditoria-marcas.service';
import { EmpleadosService } from '../../../../../core/services/empleado.service';
import { EmpleadoItem } from '../../../../../core/models/empleado.model';

// PrimeNG
import { TableModule, TableLazyLoadEvent } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { DialogModule } from 'primeng/dialog';
import { MessageService } from 'primeng/api';
import { ItemAuditoria } from '../../../../../core/models/auditoria-marcas.model';

@Component({
  selector: 'app-auditoria-marcas',
  standalone: true,
  imports: [CommonModule, FormsModule, TableModule, ToastModule, DialogModule],
  providers: [MessageService],
  templateUrl: './auditoria-marcas.component.html'
})
export class AuditoriaMarcasComponent implements OnInit {
  private auditoriaService = inject(AuditoriaMarcasService);
  private empleadosService = inject(EmpleadosService);
  private messageService = inject(MessageService);
  private cdr = inject(ChangeDetectorRef);

  registros = signal<ItemAuditoria[]>([]);
  empleados = signal<EmpleadoItem[]>([]);
  totalRegistros = signal<number>(0);
  cargando = signal<boolean>(false);

  // Filtros
  busqueda = signal<string>('');
  tipoOperacion = signal<string>('TODOS');
  pagina = 1;
  filas = 15;

  // Modal Regularización
  dialogManual = signal<boolean>(false);
  formManual = {
    empleadoId: null as number | null,
    fecha: new Date().toISOString().split('T')[0],
    hora: '08:00:00',
    tipoMarcacion: 'ENTRADA',
    motivo: ''
  };

  ngOnInit(): void {
    this.cargarEmpleados();
    this.consultar(1);
  }

  cargarEmpleados(): void {
    this.empleadosService.listar().subscribe(e => this.empleados.set(e));
  }

  consultar(pagina: number = 1): void {
    this.cargando.set(true);
    this.pagina = pagina;

    const payload = {
      tipoOperacion: this.tipoOperacion(),
      busqueda: this.busqueda().trim() || undefined,
      pagina: this.pagina,
      registrosPorPagina: this.filas
    };

    this.auditoriaService.consultar(payload).subscribe({
      next: (res) => {
        this.registros.set(res.items);
        this.totalRegistros.set(res.totalRegistros);
        this.cargando.set(false);
        this.cdr.markForCheck();
      },
      error: () => {
        this.cargando.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Fallo al consultar auditoría.' });
        this.cdr.markForCheck();
      }
    });
  }

  onLazyLoad(event: TableLazyLoadEvent): void {
    const primeraFila = event.first ?? 0;
    const filas = event.rows ?? this.filas;
    this.filas = filas;
    const nuevaPagina = Math.floor(primeraFila / filas) + 1;
    this.consultar(nuevaPagina);
  }

  abrirRegularizacion(): void {
    this.formManual = {
      empleadoId: null,
      fecha: new Date().toISOString().split('T')[0],
      hora: '08:00:00',
      tipoMarcacion: 'ENTRADA',
      motivo: ''
    };
    this.dialogManual.set(true);
  }

  guardarRegularizacion(): void {
    if (!this.formManual.empleadoId || !this.formManual.motivo.trim()) {
      this.messageService.add({ severity: 'warn', summary: 'Atención', detail: 'Seleccione el colaborador y especifique el motivo institucional.' });
      return;
    }

    const payload = {
      empleadoId: this.formManual.empleadoId,
      fecha: this.formManual.fecha,
      hora: this.formManual.hora,
      tipoMarcacion: this.formManual.tipoMarcacion,
      motivo: this.formManual.motivo.trim()
    };

    this.auditoriaService.regularizar(payload).subscribe({
      next: (res) => {
        this.messageService.add({ severity: 'success', summary: 'Auditado', detail: res.mensaje });
        this.dialogManual.set(false);
        this.consultar(1);
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: err?.error?.mensaje || 'No se pudo regularizar.' });
      }
    });
  }
}