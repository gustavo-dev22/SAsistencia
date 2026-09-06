import { Component, OnInit, signal, computed, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrganizacionService } from '../../../../core/services/organizacion.service';

// PrimeNG
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { SharedModule, MessageService } from 'primeng/api';
import { CargoItem, OficinaItem } from '../../../../core/models/organizacion.model';

@Component({
  selector: 'app-areas-cargos',
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
    SharedModule
  ],
  providers: [MessageService],
  templateUrl: './areas-cargos.component.html'
})
export class AreasCargosComponent implements OnInit {
  private orgService = inject(OrganizacionService);
  private messageService = inject(MessageService);
  private cdr = inject(ChangeDetectorRef);

  // Tab activo: 'oficinas' | 'cargos'
  tabActiva = signal<'oficinas' | 'cargos'>('oficinas');

  cargando = signal<boolean>(false);
  oficinas = signal<OficinaItem[]>([]);
  cargos = signal<CargoItem[]>([]);

  // Modales Cargos
  dialogCargo = signal<boolean>(false);
  esNuevoCargo = signal<boolean>(true);
  cargoForm: { id: number; nombre: string; descripcion: string; exoneradoMarcacion: boolean; activo: boolean } = {
    id: 0,
    nombre: '',
    descripcion: '',
    exoneradoMarcacion: false,
    activo: true
  };

  // Métricas Computadas
  totalOficinas = computed(() => this.oficinas().length);
  totalCargos = computed(() => this.cargos().length);
  cargosExonerados = computed(() => this.cargos().filter(c => c.exoneradoMarcacion).length);

  // Señales para los textos de búsqueda
  filtroOficinasTexto = signal<string>('');
  filtroCargosTexto = signal<string>('');

  // Listas filtradas reactivas para alimentar las tablas
  oficinasFiltradas = computed(() => {
    const q = this.filtroOficinasTexto().trim().toLowerCase();
    if (!q) return this.oficinas();
    return this.oficinas().filter(o => 
      o.nombre?.toLowerCase().includes(q) || 
      o.sigla?.toLowerCase().includes(q) ||
      o.nombreOficinaPadre?.toLowerCase().includes(q)
    );
  });

  cargosFiltrados = computed(() => {
    const q = this.filtroCargosTexto().trim().toLowerCase();
    if (!q) return this.cargos();
    return this.cargos().filter(c => 
      c.nombre?.toLowerCase().includes(q) || 
      c.descripcion?.toLowerCase().includes(q)
    );
  });

  ngOnInit(): void {
    this.cargarOficinas();
    this.cargarCargos();
  }

  cargarOficinas(): void {
    this.cargando.set(true);
    this.orgService.listarOficinas().subscribe({
      next: (data) => {
        this.oficinas.set([...data]);
        this.cargando.set(false);
        this.cdr.markForCheck();
      },
      error: () => {
        this.cargando.set(false);
        this.cdr.markForCheck();
      }
    });
  }

  cargarCargos(): void {
    this.orgService.listarCargos().subscribe({
      next: (data) => {
        this.cargos.set([...data]);
        this.cdr.markForCheck();
      }
    });
  }

  sincronizarOficinas(): void {
    this.cargando.set(true);
    this.orgService.sincronizarOficinas().subscribe({
      next: (res) => {
        this.messageService.add({ severity: 'success', summary: 'SASI Sync', detail: res.mensaje });
        this.cargarOficinas();
      },
      error: (err) => {
        this.cargando.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'Error al conectar con SASI.' });
        this.cdr.markForCheck();
      }
    });
  }

  // --- MÉTODOS DE BÚSQUEDA Y LIMPIEZA ---
  onBuscarOficinas(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.filtroOficinasTexto.set(input.value);
  }

  limpiarFiltroOficinas(): void {
    this.filtroOficinasTexto.set('');
    this.cdr.markForCheck();
  }

  onBuscarCargos(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.filtroCargosTexto.set(input.value);
  }

  limpiarFiltroCargos(): void {
    this.filtroCargosTexto.set('');
    this.cdr.markForCheck();
  }

  // --- CRUD CARGOS ---
  abrirCrearCargo(): void {
    this.esNuevoCargo.set(true);
    this.cargoForm = { id: 0, nombre: '', descripcion: '', exoneradoMarcacion: false, activo: true };
    this.dialogCargo.set(true);
    this.cdr.markForCheck();
  }

  abrirEditarCargo(c: CargoItem): void {
    this.esNuevoCargo.set(false);
    this.cargoForm = {
      id: c.id,
      nombre: c.nombre,
      descripcion: c.descripcion || '',
      exoneradoMarcacion: c.exoneradoMarcacion,
      activo: c.activo
    };
    this.dialogCargo.set(true);
    this.cdr.markForCheck();
  }

  guardarCargo(): void {
    if (!this.cargoForm.nombre.trim()) {
      this.messageService.add({ severity: 'warn', summary: 'Atención', detail: 'El nombre del cargo es obligatorio.' });
      return;
    }

    if (this.esNuevoCargo()) {
      this.orgService.crearCargo({
        nombre: this.cargoForm.nombre,
        descripcion: this.cargoForm.descripcion,
        exoneradoMarcacion: this.cargoForm.exoneradoMarcacion
      }).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Cargo registrado correctamente.' });
          this.dialogCargo.set(false);
          this.cargarCargos();
        }
      });
    } else {
      this.orgService.actualizarCargo(this.cargoForm).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Cargo actualizado correctamente.' });
          this.dialogCargo.set(false);
          this.cargarCargos();
        }
      });
    }
  }
}