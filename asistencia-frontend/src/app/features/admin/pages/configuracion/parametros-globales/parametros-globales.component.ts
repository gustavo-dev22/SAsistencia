import { Component, OnInit, signal, computed, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ParametrosService } from '../../../../../core/services/parametros.service';

// PrimeNG
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { ParametroGlobal } from '../../../../../core/models/parametro.model';

@Component({
  selector: 'app-parametros-globales',
  standalone: true,
  imports: [CommonModule, FormsModule, ToastModule],
  providers: [MessageService],
  templateUrl: './parametros-globales.component.html'
})
export class ParametrosGlobalesComponent implements OnInit {
  private parametrosService = inject(ParametrosService);
  private messageService = inject(MessageService);
  private cdr = inject(ChangeDetectorRef);

  parametros = signal<ParametroGlobal[]>([]);
  cargando = signal<boolean>(false);
  guardando = signal<boolean>(false);

  // Mapeo mutable en memoria para los inputs
  valoresForm: Record<string, string> = {};
  valoresOriginales: Record<string, string> = {};

  diasCatalogo = [
    { id: '1', label: 'Lun' },
    { id: '2', label: 'Mar' },
    { id: '3', label: 'Mié' },
    { id: '4', label: 'Jue' },
    { id: '5', label: 'Vie' },
    { id: '6', label: 'Sáb' },
    { id: '0', label: 'Dom' }
  ];

  esDiaSeleccionado(claveParam: string, diaId: string): boolean {
    const valor = this.valoresForm[claveParam] || '';
    const seleccionados = valor.split(',').map(d => d.trim());
    return seleccionados.includes(diaId);
  }

  toggleDiaParametro(claveParam: string, diaId: string): void {
    const valorActual = this.valoresForm[claveParam] || '';
    let seleccionados = valorActual.split(',').map(d => d.trim()).filter(d => d.length > 0);

    if (seleccionados.includes(diaId)) {
      if (seleccionados.length > 1) { // Evitar dejar sin ningún día
        seleccionados = seleccionados.filter(d => d !== diaId);
      }
    } else {
      seleccionados.push(diaId);
    }

    // Ordenar numéricamente (1 a 6 y 0)
    seleccionados.sort((a, b) => {
      const ordenA = a === '0' ? 7 : Number(a);
      const ordenB = b === '0' ? 7 : Number(b);
      return ordenA - ordenB;
    });

    this.valoresForm[claveParam] = seleccionados.join(',');
    this.cdr.markForCheck();
  }

  // Categorías agrupadas
  categorias = computed(() => {
    const cats = new Set(this.parametros().map(p => p.categoria));
    return Array.from(cats);
  });

  ngOnInit(): void {
    this.cargarParametros();
  }

  cargarParametros(): void {
    this.cargando.set(true);
    this.parametrosService.listar().subscribe({
        next: (data) => {
        this.parametros.set(data);
        this.valoresForm = {};
        this.valoresOriginales = {};
        for (const p of data) {
            this.valoresForm[p.clave] = p.valor;
            this.valoresOriginales[p.clave] = p.valor; // Respaldo para comparar
        }
        this.cargando.set(false);
        this.cdr.markForCheck();
        }
    });
  }

  obtenerPorCategoria(categoria: string): ParametroGlobal[] {
    return this.parametros().filter(p => p.categoria === categoria);
  }

  formatearNombreCategoria(cat: string): string {
    const mapa: Record<string, string> = {
      QUIOSCO: 'Terminal Quiosco & Accesos',
      POLITICAS: 'Políticas de Asistencia & Tolerancias',
      SASI: 'Integración Central SASI',
      SEGURIDAD: 'Seguridad & Auditoría'
    };
    return mapa[cat] || cat;
  }

  guardarCambios(): void {
    // Solo extraer los parámetros que cambiaron respecto al original
    const modificados = Object.keys(this.valoresForm)
        .filter(clave => String(this.valoresForm[clave]) !== String(this.valoresOriginales[clave]))
        .map(clave => ({
        clave,
        valor: String(this.valoresForm[clave])
        }));

    if (modificados.length === 0) {
        this.messageService.add({ 
        severity: 'info', 
        summary: 'Sin cambios', 
        detail: 'No ha modificado ningún parámetro institucional.' 
        });
        return;
    }

    this.guardando.set(true);
    this.parametrosService.guardarBatch(modificados).subscribe({
        next: (res) => {
        this.guardando.set(false);
        this.messageService.add({ severity: 'success', summary: 'Éxito', detail: res.mensaje });
        this.cargarParametros();
        },
        error: () => {
        this.guardando.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Fallo al guardar.' });
        this.cdr.markForCheck();
        }
    });
  }
}