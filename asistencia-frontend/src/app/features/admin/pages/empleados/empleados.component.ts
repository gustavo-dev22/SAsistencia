import { Component, OnInit, signal, inject, ChangeDetectorRef, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmpleadosService } from '../../../../core/services/empleado.service';

// PrimeNG
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { SharedModule, MessageService } from 'primeng/api';
import { SelectModule } from 'primeng/select';
import * as QRCode from 'qrcode';
import { EmpleadoItem } from '../../../../core/models/empleado.model';
import { OrganizacionService } from '../../../../core/services/organizacion.service';
import { CargoItem } from '../../../../core/models/organizacion.model';

@Component({
  selector: 'app-empleados',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    DialogModule,
    TagModule,
    ToastModule,
    TooltipModule,
    SharedModule,
    SelectModule,
  ],
  providers: [MessageService],
  templateUrl: './empleados.component.html'
})
export class EmpleadosComponent implements OnInit {
  private empleadosService = inject(EmpleadosService);
  private orgService = inject(OrganizacionService);
  private messageService = inject(MessageService);
  private cdr = inject(ChangeDetectorRef);

  empleados = signal<EmpleadoItem[]>([]);
  cargos = signal<CargoItem[]>([]);
  cargando = signal<boolean>(false);

  // --- CÁLCULO REACTIVO DE ESTADÍSTICAS ---
  totalEmpleados = computed(() => this.empleados().length);
  totalHabilitados = computed(() => this.empleados().filter(e => e.habilitadoParaMarcar).length);
  totalBloqueados = computed(() => this.empleados().filter(e => !e.habilitadoParaMarcar).length);
  totalSinDni = computed(() => this.empleados().filter(e => !e.dni || e.dni.trim() === '').length);

  // Modales
  dialogEdicion = signal<boolean>(false);
  dialogQr = signal<boolean>(false);

  textoFiltro = signal<string>('');

  empleadosFiltrados = computed(() => {
    const query = this.textoFiltro().trim().toLowerCase();
    const lista = this.empleados();

    if (!query) {
      return lista;
    }

    return lista.filter(emp => 
      (emp.nombreCompleto?.toLowerCase().includes(query)) ||
      (emp.dni?.toLowerCase().includes(query)) ||
      (emp.email?.toLowerCase().includes(query)) ||
      (emp.oficinaNombre?.toLowerCase().includes(query)) ||
      (emp.cargoNombre?.toLowerCase().includes(query))
    );
  });

  empleadoSeleccionado: EmpleadoItem | null = null;
  qrDataUrl = signal<string>('');

  ngOnInit(): void {
    this.cargarEmpleados();
    this.cargarCargos();
  }

  onBuscar(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.textoFiltro.set(input.value);
  }

  limpiarBusqueda(): void {
    this.textoFiltro.set('');
    this.cdr.markForCheck();
  }

  cargarEmpleados(): void {
    this.cargando.set(true);
    this.empleadosService.listar().subscribe({
      next: (data) => {
        this.empleados.set([...data]);
        this.cargando.set(false);
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error(err);
        this.cargando.set(false);
        this.messageService.add({ 
          severity: 'error', 
          summary: 'Error', 
          detail: 'No se pudieron cargar los empleados.' 
        });
        this.cdr.markForCheck();
      }
    });
  }

  cargarCargos(): void {
    this.orgService.listarCargos().subscribe({
      next: (data) => {
        // Solo mostramos cargos activos
        this.cargos.set(data.filter(c => c.activo));
        this.cdr.markForCheck();
      }
    });
  }

  sincronizarConSasi(): void {
    this.cargando.set(true);
    this.empleadosService.sincronizar().subscribe({
      next: (res) => {
        this.messageService.add({ severity: 'success', summary: 'SASI Sync', detail: res.mensaje });
        this.cargarEmpleados();
      },
      error: () => {
        this.cargando.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Fallo al conectar con SASI.' });
        this.cdr.markForCheck();
      }
    });
  }

  abrirEdicion(emp: EmpleadoItem): void {
    this.empleadoSeleccionado = { ...emp };
    this.dialogEdicion.set(true);
    this.cdr.markForCheck();
  }

  guardarEdicion(): void {
    if (!this.empleadoSeleccionado) return;

    this.empleadosService.actualizar({
      id: this.empleadoSeleccionado.id,
      dni: this.empleadoSeleccionado.dni || '',
      turnoId: this.empleadoSeleccionado.turnoId,
      cargoId: this.empleadoSeleccionado.cargoId,
      habilitadoParaMarcar: this.empleadoSeleccionado.habilitadoParaMarcar
    }).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Datos actualizados correctamente.' });
        this.dialogEdicion.set(false);
        this.cargarEmpleados();
      }
    });
  }

  async verCarnetQr(emp: EmpleadoItem): Promise<void> {
    this.empleadoSeleccionado = emp;
    try {
      const url = await QRCode.toDataURL(emp.codigoQr, { width: 220, margin: 1 });
      this.qrDataUrl.set(url);
      this.dialogQr.set(true);
      this.cdr.markForCheck();
    } catch {
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo generar el código QR.' });
    }
  }

  regenerarCodigo(): void {
    if (!this.empleadoSeleccionado) return;
    this.empleadosService.regenerarQr(this.empleadoSeleccionado.id).subscribe({
      next: async (res) => {
        this.empleadoSeleccionado!.codigoQr = res.codigoQr;
        const url = await QRCode.toDataURL(res.codigoQr, { width: 220, margin: 1 });
        this.qrDataUrl.set(url);
        this.messageService.add({ severity: 'info', summary: 'Regenerado', detail: res.mensaje });
        this.cargarEmpleados();
      }
    });
  }
}