import { Component, OnInit, signal, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { JustificacionesService } from '../../../../../core/services/justificaciones.service';
import { EmpleadosService } from '../../../../../core/services/empleado.service';
import { OrganizacionService } from '../../../../../core/services/organizacion.service';
import { EmpleadoItem } from '../../../../../core/models/empleado.model';

// PrimeNG
import { TableModule, TableLazyLoadEvent } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { DialogModule } from 'primeng/dialog';
import { TooltipModule } from 'primeng/tooltip';
import { MessageService } from 'primeng/api';
import { JustificacionItem, TipoJustificacion } from '../../../../../core/models/justificacion.model';
import { OficinaItem } from '../../../../../core/models/organizacion.model';

@Component({
  selector: 'app-justificaciones',
  standalone: true,
  imports: [CommonModule, FormsModule, TableModule, ToastModule, DialogModule, TooltipModule],
  providers: [MessageService],
  templateUrl: './justificaciones.component.html'
})
export class JustificacionesComponent implements OnInit {
  private justificacionesService = inject(JustificacionesService);
  private empleadosService = inject(EmpleadosService);
  private orgService = inject(OrganizacionService);
  private messageService = inject(MessageService);
  private cdr = inject(ChangeDetectorRef);

  // Datos
  justificaciones = signal<JustificacionItem[]>([]);
  tipos = signal<TipoJustificacion[]>([]);
  oficinas = signal<OficinaItem[]>([]);
  empleados = signal<EmpleadoItem[]>([]);

  totalRegistros = signal<number>(0);
  cargando = signal<boolean>(false);

  // Filtros
  filtroEstado = signal<string>('TODOS');
  filtroTipo = signal<number | null>(null);
  filtroOficina = signal<number | null>(null);
  filtroTexto = signal<string>('');
  pagina = 1;
  filas = 15;

  // Modales
  dialogNueva = signal<boolean>(false);
  dialogResolver = signal<boolean>(false);
  itemParaResolver = signal<JustificacionItem | null>(null);
  observacionesResolucion = '';

  // Formulario Registro
  formRegistro = {
    empleadoId: null as number | null,
    tipoJustificacionId: null as number | null,
    fechaInicio: new Date().toISOString().split('T')[0],
    fechaFin: new Date().toISOString().split('T')[0],
    motivo: '',
    archivo: null as File | null
  };

  ngOnInit(): void {
    this.cargarParametros();
    this.consultar(1);
  }

  cargarParametros(): void {
    this.justificacionesService.listarTipos().subscribe(t => this.tipos.set(t));
    this.orgService.listarOficinas().subscribe(o => this.oficinas.set(o.filter(x => x.activo)));
    this.empleadosService.listar().subscribe(e => this.empleados.set(e));
  }

  consultar(pagina: number = 1): void {
    this.cargando.set(true);
    this.pagina = pagina;

    const payload = {
      fechaInicio: null,
      fechaFin: null,
      estado: this.filtroEstado(),
      tipoJustificacionId: this.filtroTipo(),
      oficinaId: this.filtroOficina(),
      busqueda: this.filtroTexto().trim() || undefined,
      pagina: this.pagina,
      registrosPorPagina: this.filas
    };

    this.justificacionesService.consultarHistorial(payload).subscribe({
      next: (res) => {
        this.justificaciones.set(res.items);
        this.totalRegistros.set(res.totalRegistros);
        this.cargando.set(false);
        this.cdr.markForCheck();
      },
      error: () => {
        this.cargando.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Fallo al consultar justificaciones.' });
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

  limpiarFiltros(): void {
    this.filtroEstado.set('TODOS');
    this.filtroTipo.set(null);
    this.filtroOficina.set(null);
    this.filtroTexto.set('');
    this.consultar(1);
  }

  abrirCrear(): void {
    this.formRegistro = {
      empleadoId: null,
      tipoJustificacionId: this.tipos().length > 0 ? this.tipos()[0].id : null,
      fechaInicio: new Date().toISOString().split('T')[0],
      fechaFin: new Date().toISOString().split('T')[0],
      motivo: '',
      archivo: null
    };
    this.dialogNueva.set(true);
    this.cdr.markForCheck();
  }

  onArchivoSeleccionado(event: any): void {
    const file: File = event.target.files[0];
    if (file) {
      this.formRegistro.archivo = file;
    }
  }

  guardarJustificacion(): void {
    if (!this.formRegistro.empleadoId || !this.formRegistro.tipoJustificacionId || !this.formRegistro.motivo.trim()) {
      this.messageService.add({ severity: 'warn', summary: 'Campos incompletos', detail: 'Complete el colaborador, tipo y motivo.' });
      return;
    }

    const formData = new FormData();
    formData.append('empleadoId', this.formRegistro.empleadoId.toString());
    formData.append('tipoJustificacionId', this.formRegistro.tipoJustificacionId.toString());
    formData.append('fechaInicio', this.formRegistro.fechaInicio);
    formData.append('fechaFin', this.formRegistro.fechaFin);
    formData.append('motivo', this.formRegistro.motivo.trim());

    if (this.formRegistro.archivo) {
      formData.append('archivoSustento', this.formRegistro.archivo);
    }

    this.justificacionesService.registrar(formData).subscribe({
      next: (res) => {
        this.messageService.add({ severity: 'success', summary: 'Registrado', detail: res.mensaje });
        this.dialogNueva.set(false);
        this.consultar(1);
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: err?.error?.mensaje || 'No se pudo radicar.' });
      }
    });
  }

  abrirResolver(item: JustificacionItem): void {
    this.itemParaResolver.set(item);
    this.observacionesResolucion = '';
    this.dialogResolver.set(true);
    this.cdr.markForCheck();
  }

  ejecutarResolucion(estado: 'APROBADA' | 'RECHAZADA'): void {
    const item = this.itemParaResolver();
    if (!item) return;

    this.justificacionesService.resolver(item.id, estado, this.observacionesResolucion).subscribe({
      next: (res) => {
        this.messageService.add({ severity: 'success', summary: 'Éxito', detail: res.mensaje });
        this.dialogResolver.set(false);
        this.consultar(this.pagina);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Fallo al procesar la resolución.' });
      }
    });
  }

  descargarSustento(id: number): void {
    window.open(this.justificacionesService.descargarArchivoUrl(id), '_blank');
  }
}